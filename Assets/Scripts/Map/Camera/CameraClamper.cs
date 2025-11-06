using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraClamper : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField, Range(0f, 1f)] private float followLerp = 0.2f;

    [Header("Search / Retry")]
    [Tooltip("Grid 루트를 직접 할당. 비워두면 이름/컴포넌트로 자동 탐색.")]
    [SerializeField] private Transform searchRoot;
    [Tooltip("자동 탐색시 우선 찾을 이름(, 로 여러개) ex) Grid,MapRoot")]
    [SerializeField] private string preferredRootNames = "Grid, MapRoot";
    [Tooltip("재시도 간격(초, TimeScale 무시)")]
    [SerializeField] private float retryInterval = 0.5f;
    [Tooltip("무한 재시도 여부")]
    [SerializeField] private bool retryForever = true;
    [Tooltip("무한 재시도가 아니면 최대 시도 횟수")]
    [SerializeField] private int maxRetry = 60;
    [Tooltip("찾은 뒤에도 계속 감시(씬 전환/맵 교체에 대응)")]
    [SerializeField] private bool keepWatching = false;
    [Tooltip("비활성 오브젝트까지 포함해 탐색")]
    [SerializeField] private bool includeInactive = true;

    [Header("Bounds")]
    [SerializeField] private Vector2 padding = Vector2.zero;
    [SerializeField] private bool compressEachTilemap = true;

    [Header("Edge Inset")]
    [SerializeField] private bool insetOneTile = true;       // 한 칸 안쪽에서 막기
    [SerializeField] private float insetTiles = 2f;          // 인셋 타일 수

    [Header("Pushback")]
    [SerializeField] private bool instantWhenPushing = true; // 경계 밖으로 밀면 즉시 스냅

    [SerializeField] private SceneChangeManager sceneChangeManager;

    private Camera cam;
    private Bounds worldBounds;
    private bool hasBounds;

    private Coroutine detectLoopCo;
    private readonly List<Tilemap> tilemaps = new List<Tilemap>();

    private void Awake()
    {
        cam = GetComponent<Camera>();
        sceneChangeManager = FindAnyObjectByType<SceneChangeManager>();
    }

    private void OnEnable()
    {
        StartDetectLoop();
    }

    private void OnDisable()
    {
        if (detectLoopCo != null) StopCoroutine(detectLoopCo);
        detectLoopCo = null;
    }

    private void LateUpdate()
    {
        if (!hasBounds || target == null) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        Vector3 camPos = transform.position;
        Vector2 trg = target.position;

        // ---- 한 칸 인셋 계산 ----
        Vector2 tileSize = GetTileWorldSize();
        float insetX = padding.x + (insetOneTile ? tileSize.x * insetTiles : 0f);
        float insetY = padding.y + (insetOneTile ? tileSize.y * insetTiles : 0f);

        // 카메라 프레임 + 인셋 반영한 유효 경계
        float minX = worldBounds.min.x + halfW + insetX;
        float maxX = worldBounds.max.x - halfW - insetX;
        float minY = worldBounds.min.y + halfH + insetY;
        float maxY = worldBounds.max.y - halfH - insetY;

        // 맵이 작을 때 중앙 고정
        if (minX > maxX) { float c = (minX + maxX) * 0.5f; minX = maxX = c; }
        if (minY > maxY) { float c = (minY + maxY) * 0.5f; minY = maxY = c; }

        // 하드 클램프 지점 (= 카메라가 갈 수 있는 최대 범위)
        Vector2 clamped = new Vector2(
            Mathf.Clamp(trg.x, minX, maxX),
            Mathf.Clamp(trg.y, minY, maxY)
        );

        // 경계 밖으로 '밀고' 있는지 (콜라이더처럼 즉시 스냅할 조건)
        bool pushing = (trg.x < minX || trg.x > maxX || trg.y < minY || trg.y > maxY);

        Vector3 desired = new Vector3(clamped.x, clamped.y, camPos.z);

        if (instantWhenPushing && pushing)
        {
            // 콜라이더처럼 즉시 경계까지 스냅
            transform.position = desired;
        }
        else
        {
            // 평소엔 부드럽게 추적
            transform.position = Vector3.Lerp(camPos, desired, followLerp);
        }
    }

    // ====== 탐지 루프 ======
    private void StartDetectLoop()
    {
        if (detectLoopCo != null) return;
        detectLoopCo = StartCoroutine(DetectLoop());
    }

    private IEnumerator DetectLoop()
    {
        int tries = 0;
        var wait = new WaitForSecondsRealtime(Mathf.Max(0.05f, retryInterval));

        while (true)
        {
            // 1) 루트 탐색
            if (searchRoot == null)
            {
                searchRoot = TryFindRootTransform();
            }

            // 2) 타일맵 수집 + 경계 계산
            if (searchRoot != null && CollectTilemaps(searchRoot) > 0)
            {
                CalculateWorldBounds();
                if (hasBounds)
                {
                    // 찾자마자 성공!
                    if (!keepWatching)
                    {
                        detectLoopCo = null;
                        yield break;
                    }
                    // 감시 모드면 계속 주기적으로 체크 (맵 교체 대응)
                    tries = 0; // 카운트 리셋
                }
            }
            else
            {
                tries++;
                if (!retryForever && tries >= Mathf.Max(1, maxRetry))
                {
                    Debug.LogWarning("[CameraClamper] Grid/Tilemap 탐색 실패: 최대 재시도 도달");
                    detectLoopCo = null;
                    yield break;
                }
            }

            yield return wait;
        }
    }

    // 우선 이름으로, 실패시 컴포넌트(Grid)로 검색
    private Transform TryFindRootTransform()
    {
        // 1) 이름 우선 탐색
        if (!string.IsNullOrWhiteSpace(preferredRootNames))
        {
            var names = preferredRootNames.Split(',');
            foreach (var raw in names)
            {
                var name = raw.Trim();
                if (string.IsNullOrEmpty(name)) continue;

                var go = GameObject.Find(name);
                if (go != null)
                {
                    Debug.Log($"[CameraClamper] 이름으로 Grid 루트 찾음: {name}");
                    return go.transform;
                }
            }
        }

        // 2) Grid 컴포넌트 탐색(씬 전체)
        var grids = Resources.FindObjectsOfTypeAll<Grid>();
        foreach (var g in grids)
        {
            // includeInactive를 고려해 씬 객체만 필터
            if (g == null || g.gameObject.scene.name == null) continue;
            Debug.Log($"[CameraClamper] Grid 컴포넌트로 루트 찾음: {g.name}");
            return g.transform;
        }

        return null;
    }

    private int CollectTilemaps(Transform root)
    {
        tilemaps.Clear();
        var found = root.GetComponentsInChildren<Tilemap>(includeInactive);
        foreach (var tm in found)
        {
            if (tm != null) tilemaps.Add(tm);
        }
        return tilemaps.Count;
    }

    private void CalculateWorldBounds()
    {
        if (tilemaps.Count == 0) { hasBounds = false; return; }

        bool first = true;
        Bounds combined = new Bounds();

        foreach (var tm in tilemaps)
        {
            if (tm == null) continue;

            if (compressEachTilemap) tm.CompressBounds();

            BoundsInt cb = tm.cellBounds;
            Vector3Int minCell = cb.min;
            Vector3Int maxCell = cb.max;

            // 셀 경계를 월드로 변환 (maxCell은 exclusive → 한 칸 더)
            Vector3 minWorld = tm.CellToWorld(minCell);
            Vector3 maxWorld = tm.CellToWorld(maxCell + Vector3Int.one);

            Bounds wb = new Bounds();
            wb.SetMinMax(minWorld, maxWorld);

            if (first) { combined = wb; first = false; }
            else combined.Encapsulate(wb);
        }

        combined.Expand(new Vector3(padding.x * 2f, padding.y * 2f, 0f));

        var sceneName = sceneChangeManager.currentScene;
        if (sceneName == SceneName.TownScene)
        {
            // 타일 사이즈만큼 왼쪽, 아래쪽으로 확장
            Vector2 tileSize = GetTileWorldSize();
            combined.min = new Vector3(combined.min.x - tileSize.x, combined.min.y - tileSize.y, combined.min.z);
        }

        worldBounds = combined;
        hasBounds = true;

    }

    private Vector2 GetTileWorldSize()
    {

        // 등록된 타일맵 중 첫 번째의 셀 크기를 사용
        for (int i = 0; i < tilemaps.Count; i++)
        {
            var tm = tilemaps[i];
            if (tm != null)
            {
                Vector3 cs = tm.cellSize;
                return new Vector2(
                    Mathf.Abs(cs.x) < 1e-5f ? 1f : cs.x,
                    Mathf.Abs(cs.y) < 1e-5f ? 1f : cs.y
                );
            }
        }
        return Vector2.one; // 기본값
    }

}