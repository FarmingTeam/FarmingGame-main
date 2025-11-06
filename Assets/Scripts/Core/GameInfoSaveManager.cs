using System;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public struct GameInfo
{
    public int CurrentScene;
    public GameTime CurrentTime;
    public Vector2Int CurrentLocation;
}

public class GameInfoSaveManager : Singleton<GameInfoSaveManager>
{
    public const string GAMEINFONAME = "GameInfo.Json";
    public readonly Vector2Int PLAYERHOUSEINITPOS = new Vector2Int(7, 2);

    public GameInfo currentGameInfo = new GameInfo();

    protected override void Initialize()
    {
        currentGameInfo.CurrentScene = (int)SceneName.HouseScene;
        currentGameInfo.CurrentTime.date = 1;
        currentGameInfo.CurrentTime.hour = 6;
        currentGameInfo.CurrentTime.minute = 0;
        currentGameInfo.CurrentLocation = new Vector2Int(6, 8);
        PlayerManager.Instance.Direction = TileReader.Facing.Up;
    }
    public void UpdateCacheGameInfo(SceneName nextScene, Vector2Int nextLocation)
    {
        currentGameInfo.CurrentScene = (int)nextScene;
        currentGameInfo.CurrentTime = TimeManager.Instance.currentTime;
        currentGameInfo.CurrentLocation = nextLocation;
    }


    public void SaveCacheGameInfo(bool nextDayAutoSave)
    {
        currentGameInfo.CurrentTime = TimeManager.Instance.currentTime;
        
        if (nextDayAutoSave)
        {
            currentGameInfo.CurrentScene = (int)SceneName.HouseScene;
            currentGameInfo.CurrentLocation = PLAYERHOUSEINITPOS;
        }
        else
        {
            currentGameInfo.CurrentScene = (int)SceneChangeManager.Instance.currentScene;
            currentGameInfo.CurrentLocation = (Vector2Int)MapControl.Instance?.player.tileReader.CurrentCell;
        }
    }

    public void SaveCacheToSlot(int slotNumber, bool nextDayAutoSave = false)
    {
        SaveCacheGameInfo(nextDayAutoSave);
        StringBuilder stringBuilder = new StringBuilder();
        //디렉토리 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber);
        Directory.CreateDirectory(stringBuilder.ToString());

        string gameInfoString = JsonUtility.ToJson(currentGameInfo, true);
        File.WriteAllText(stringBuilder.Append('/').Append(GAMEINFONAME).ToString(), gameInfoString);
    }

    public void LoadSlotToCache(int slotNumber)
    {
        StringBuilder stringBuilder = new StringBuilder();
        //파일 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(GAMEINFONAME);
        if (!File.Exists(stringBuilder.ToString()))
        {
            throw new Exception("File Does not Exists");
        }
        string gameInfoString = File.ReadAllText(stringBuilder.ToString());
        currentGameInfo = JsonUtility.FromJson<GameInfo>(gameInfoString);
    }
}
