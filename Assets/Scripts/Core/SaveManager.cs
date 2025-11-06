using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;



public class SaveManager : Singleton<SaveManager>
{
    public const string SAVEFILEPATH = "SaveSlot";
    public const string PREVIEWNAME = "Preview.png";


    public GameInfo gameInfo = new GameInfo();

    public readonly Vector3 PlayerHouseInitPos = new Vector3(7, 2);

    public bool isLoad = false;



    //Quick Save : 0
    //Slot Save  : 1,2
    public void OnSaveSlot(int slot = 0, bool nextDayAutoSave = false)
    {
        //타일 File 경로 불러오기
        StringBuilder stringBuilder = new StringBuilder();
        //디렉토리 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SAVEFILEPATH).Append(slot);
        if (Directory.Exists(Application.persistentDataPath + "/" + SAVEFILEPATH + slot))
            DeleteDirectory(Application.persistentDataPath + "/" + SAVEFILEPATH + slot);
        Directory.CreateDirectory(stringBuilder.ToString());

        GameInfoSaveManager.Instance.SaveCacheToSlot(slot, nextDayAutoSave);
        MapSaveManager.Instance.SaveCacheToSlot(slot);
        PlayerManager.Instance.SaveCacheToSlot(slot);
        PlayerInventory.Instance.SaveInventoryStatus(slot);
        PlayerEquipment.Instance.SaveEquipmentStatus(slot);
        QuestManager.Instance.SaveQuestStatus(slot);
        NPCManager.Instance.SaveCacheToSlot(slot);


        CollectionManager.Instance.SaveObtainedItem(slot);
        CollectionManager.Instance.SaveCollection(slot);
        StartCoroutine(ScreenShot(slot));
    }

    public void OnNewGame()
    {
        GameManager gameManager = GameManager.Instance;
        PlayerManager.Instance.MakeInitialData();
        PlayerInventory.Instance.Init();
        PlayerInventory.Instance.AdditemsByID(1001, 1);
        PlayerInventory.Instance.AdditemsByID(1002, 1);
        PlayerInventory.Instance.AdditemsByID(1003, 1);
        PlayerInventory.Instance.AdditemsByID(1004, 1);
        PlayerInventory.Instance.AdditemsByID(1005, 1);
        PlayerInventory.Instance.AdditemsByID(1006, 1);
        PlayerInventory.Instance.AdditemsByID(1007, 1);
        PlayerInventory.Instance.AdditemsByID(1008, 1);
        PlayerInventory.Instance.AdditemsByID(1009, 1);
        PlayerInventory.Instance.AdditemsByID(1010, 1);

        PlayerEquipment.Instance.Init();

        SceneChangeManager.Instance.ChangeScene(
            (SceneName)GameInfoSaveManager.Instance.currentGameInfo.CurrentScene, GameInfoSaveManager.Instance.currentGameInfo.CurrentLocation);
    }

    public void OnLoadSlot(int slot)
    {
        /*Refactor : 제거
        //GameSaveData
        //타일 File 경로 불러오기
        StringBuilder stringBuilder = new StringBuilder();
        //디렉토리 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SAVEFILEPATH).Append(slot);
        string directorypath = stringBuilder.Append("/").ToString();

        if (!Directory.Exists(stringBuilder.ToString()))
            return;

        //디렉토리 데이터들을 캐싱 데이터로 저장
        StringBuilder cacheString = new StringBuilder();
        cacheString.Append(Application.persistentDataPath).Append("/").Append(CACHEPATH).Append('/');
        Directory.CreateDirectory(cacheString.ToString());
        string[] savedFiles = Directory.GetFiles(directorypath.ToString());
        foreach (string savedFile in savedFiles)
        {
            File.Copy(savedFile, cacheString.ToString() + Path.GetFileName(savedFile), true);
        }

        //현재 로드된 데이터를 기반으로 씬 전환 및 로드
        string gameInfoJson = File.ReadAllText(cacheString.Append('/').Append(GAMEINFONAME).ToString());
        gameInfo = JsonUtility.FromJson<GameInfo>(gameInfoJson);
        SceneChangeManager.Instance.ChangeScene((SceneName)gameInfo.CurrentScene, new Vector3(gameInfo.CurrentLocation.x, gameInfo.CurrentLocation.y, 0));
        */

        //타일 File 경로 불러오기
        StringBuilder stringBuilder = new StringBuilder();
        //디렉토리 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SAVEFILEPATH).Append(slot).Append('/');

        if (!Directory.Exists(stringBuilder.ToString()))
            return;
        isLoad = true;
        GameInfoSaveManager.Instance.LoadSlotToCache(slot);
        NPCManager.Instance.LoadSlotToCache(slot);
        MapSaveManager.Instance.LoadSlotToCache(slot);
        PlayerManager.Instance.LoadSlotToCache(slot);
        PlayerInventory.Instance.LoadInventoryStatus(slot);
        PlayerEquipment.Instance.LoadEquipmentStatus(slot);
        QuestManager.Instance.LoadQuestStatus(slot);


        CollectionManager.Instance.LoadObtainedItem(slot);
        CollectionManager.Instance.LoadCollection(slot);


        SceneChangeManager.Instance.ChangeScene(
            (SceneName)GameInfoSaveManager.Instance.currentGameInfo.CurrentScene, GameInfoSaveManager.Instance.currentGameInfo.CurrentLocation);
    }


    //스크린샷 기능
    private IEnumerator ScreenShot(int slot)
    {
        yield return new WaitForEndOfFrame();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SAVEFILEPATH).Append(slot).Append('/').Append(PREVIEWNAME);
        ScreenCapture.CaptureScreenshot(stringBuilder.ToString());
    }

    public void DestroyAllSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Directory.Exists(Application.persistentDataPath + "/" + SAVEFILEPATH + i))
                DeleteDirectory(Application.persistentDataPath + "/" + SAVEFILEPATH + i);
        }
    }

    void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }
}
