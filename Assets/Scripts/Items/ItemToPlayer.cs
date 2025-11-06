using UnityEngine;

public class ItemToPlayer : MonoBehaviour
{
    public ItemData data;
    public int amount = 1;

    // 플레이어에게 드롭 아이템이 끌려오기 시작하는 거리
    public float attractDistance = 1.5f;
    // 끌려오는 속도
    public float attractSpeed = 8f;
    // 인벤토리로 들어오는 거리
    public float pickUpDistance = 0.3f;

    // 플레이어 지정
    public Transform player;

    private DroppedItem cachedItem;
    private float zFixed;

    public void Init(ItemData data, int amount = 1)
    {
        this.data = data;
        this.amount = Mathf.Max(1, amount);
    }

    private void Awake()
    {
        if (player == null)
        {
            var pl = GameObject.FindGameObjectWithTag("Player");
            if (pl != null) player = pl.transform;
        }
        if (player != null)
        {
            cachedItem = player.GetComponentInParent<DroppedItem>();
        }

    }

    private void Update()
    {
        if (player == null) return;

        Vector2 pos = transform.position;
        Vector2 target = player.position;
        float dist = Vector2.Distance(pos, target);

        if (dist <= attractDistance)
        {
            // 가까운 거리에 있으면 당겨짐
            transform.position = Vector2.MoveTowards(pos, target, attractSpeed * Time.deltaTime);
            // 더 가까워지면 수집 시도
            if (dist <= pickUpDistance)
            {
                TryCollect();
                Destroy(gameObject);
            }
        }
    }

    private void TryCollect()
    {
        if (data == null || amount <= 0) return;

        // 수집 대상 찾기
        if (cachedItem == null && player != null)
            cachedItem = player.GetComponentInParent<DroppedItem>();

        if (cachedItem != null)
        {
            bool collected = cachedItem.Collect(data, amount);
            if (collected)
            {
                Destroy(gameObject);
            }
        }
    }
}
