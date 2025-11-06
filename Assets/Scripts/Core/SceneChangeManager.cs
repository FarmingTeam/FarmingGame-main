using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    StartScene = 0,
    FarmScene,
    HouseScene,
    TownScene,
    MidwayScene,
    ShopScene,
    OpeningScene
}

public struct SceneInfo
{
    public string Name;
    public bool IsMap;

    public SceneInfo(string name, bool ismap)
    {
        Name = name;
        IsMap = ismap;
    }
}


public class SceneChangeManager : Singleton<SceneChangeManager>
{
    //Refactor : 플레이어 쪽으로 이동
    public const string PLAYERPREFABPATH = "PlayerData/Player";


    public SceneName currentScene;
    public bool IsSceneChange = false;

    [SerializeField] public Transitor transitor;
    [SerializeField] public LoadingText loadingText;

    private static bool FirstFadeCleared = false;

    protected override void Initialize()
    {
        currentScene = (SceneName) SceneManager.GetActiveScene().buildIndex;
        transitor = GameObject.FindAnyObjectByType<Transitor>();
        loadingText = GameObject.FindAnyObjectByType<LoadingText>();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public readonly Dictionary<SceneName, SceneInfo> SCENENAMEDICT = new Dictionary<SceneName, SceneInfo>()
    {
        {SceneName.StartScene, new SceneInfo("Start", false) },
        {SceneName.FarmScene, new SceneInfo( "TestFarm", true) },
        {SceneName.HouseScene, new SceneInfo("TestInHouse", true)},
        {SceneName.TownScene, new SceneInfo("TestTown", true)},
        {SceneName.MidwayScene, new SceneInfo("TestMidway", true)},
        {SceneName.ShopScene, new SceneInfo("TestInShop", true)},
        {SceneName.OpeningScene, new SceneInfo("OpeningSequence", false)}
    };

    public const string SCENENAMETAIL = "Scene";

    public SceneName FindSceneNameByString(string name)
    {
        for (int i = 0; i < SCENENAMEDICT.Count; i++)
        {
            if (SCENENAMEDICT.Values.ElementAt(i).Name == name)
                return SCENENAMEDICT.Keys.ElementAt(i);
        }
        throw new System.Exception($"Key {name} not found");
    }




    //How to use
    /*
    public void OnBtnClick()
    {
        SceneChangeManager.Instance.ChangeScene(SceneName.<Type>);
    }
    */

    public void ChangeScene(SceneName scenename, Vector2Int playerdest = default(Vector2Int))
    {
        transitor.TransitStart();
        StartCoroutine(ChanseSceneCoroutine(scenename, playerdest));
        
    }

    private IEnumerator ChanseSceneCoroutine(SceneName scenename, Vector2Int playerdest = default(Vector2Int))
    {
        yield return new WaitForSeconds(1);

        StringBuilder stringBuilder = new StringBuilder();
        if (!SCENENAMEDICT.ContainsKey(scenename))
        {
            Debug.LogAssertion(stringBuilder.Append(scenename).Append("Does not Exists").ToString());
            yield return ("Does not Exists");
        }
        IsSceneChange = true;

        //전처리
        if (SCENENAMEDICT[currentScene].IsMap && SCENENAMEDICT[scenename].IsMap && !SaveManager.Instance.isLoad)
        {
            MapDataBase.Instance.SaveCacheMap(currentScene);
            GameInfoSaveManager.Instance.UpdateCacheGameInfo(scenename, playerdest);
            MapControl.Instance.OnSceneEnd();
        }
        SaveManager.Instance.isLoad = false;

        //씬 로드
        stringBuilder.Append(SCENENAMEDICT[scenename].Name).Append(SCENENAMETAIL);
        SceneManager.LoadScene(stringBuilder.ToString());

        Debug.Log("씬 전환중");

        currentScene = scenename;
    }

    //후처리
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SCENENAMEDICT[currentScene].IsMap)
        {
            GameManager gameManager = GameManager.Instance;

            MapDataBase.Instance.LoadCacheMap(currentScene);

            GameObject playerObject = Instantiate(Resources.Load<MonoBehaviour>(PLAYERPREFABPATH)).gameObject;
            MapControl.Instance.player = playerObject.GetComponent<Player>();


            TimeManager.Instance.Init();
            //플레이어 위치 조정
            MapControl.Instance.player.InitPos
            (new Vector3(GameInfoSaveManager.Instance.currentGameInfo.CurrentLocation.x,
            GameInfoSaveManager.Instance.currentGameInfo.CurrentLocation.y));

            NPCManager.Instance.OnLoadMap(currentScene);
            UIManager.Instance.InitialzieUISwitch();
            UIManager.Instance.OpenUI<UIToolBar>();
            //나경: 수정
            UIManager.Instance.OpenUI<UIItemToolBar>();
        }
        else
        {
            if (currentScene == SceneName.StartScene)
                Destroy(GameManager.Instance.gameObject);
        }
        IsSceneChange = false;

        if (currentScene == SceneName.FarmScene)
            StartCoroutine(TutSpawner.Instance.DisplayTutorial(5));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
