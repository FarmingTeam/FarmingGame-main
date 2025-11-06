using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


[Serializable]
public class MapData
{
    public int[] MapSize;
    public List<MapFloorData> TileFloor;
    public List<MapChunkData> TileObject;
    public List<MapSeedData> TileSeed;

    public List<int> RandomChunkGenMushroomHarvest;
    public List<int> RandomChunkGenFlowerHarvest;
    public List<int> RandomChunkGenWeedHarvest;
    public List<int> RandomChunkGenRockHarvest;
    public int LastUpdate;
}

[Serializable]
public class MapFloorData
{
    public int[] Pos;
    public int FloorType;
}
[Serializable]
public class MapChunkData
{
    public int[] Pos;
    public int ChunkType;
}

[Serializable]
public class MapSeedData
{
    public int[] Pos;
    public int SeedType;
    public bool IsPlanted;
    public bool IsGrowth;
    public bool IsWatered;
    public int RemainDate;
    public int RemainGrowth;
    public int LastWateredDate;
}

public class MapSaveManager : Singleton<MapSaveManager>
{
    public const string TILEDATA = "TileData";
    public const string MAPDATA = "MapData";
    public const string RESOURCEINITIALPATH = "MapData/InitialMapJson/";

    public void SaveCacheToSlot(int slotNumber)
    {
        // 파일 디렉토리 경로 설정
        StringBuilder stringBuilderDirectory = new StringBuilder().Append(Application.persistentDataPath).Append("/")
            .Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(MAPDATA);
        //디렉토리 생성
        Directory.CreateDirectory(stringBuilderDirectory.ToString());

        //맵 정보 저장
        for (int i = 0; i < MapDataBase.Instance.mapTileData.Count; i++)
        {
            string sceneName = SceneChangeManager.Instance.SCENENAMEDICT[MapDataBase.Instance.mapTileData.Keys.ElementAt(i)].Name;
            //파일 이름 지정
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
                .Append("/").Append(MAPDATA).Append("/").Append(sceneName).Append(TILEDATA).Append(".json");
            //위에서 만든 mapData를 Json으로 변경
            string mapDataString = JsonUtility.ToJson(MapDataBase.Instance.mapTileData.Values.ElementAt(i), true);
            //파일에 해당 Json 텍스트를 파일에다 적기
            File.WriteAllText(stringBuilder.ToString(), mapDataString);
        }
    }

    public void LoadSlotToCache(int slotNumber)
    {
        if(MapDataBase.Instance.mapTileData.Count != 0)
            MapDataBase.Instance.mapTileData.Clear();
        StringBuilder stringBuilder = new StringBuilder();
        //디렉토리 경로
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(MAPDATA);

        if (!Directory.Exists(stringBuilder.ToString()))
            return;

        string directorypath = stringBuilder.ToString();

        string[] savedfiles = Directory.GetFiles(directorypath);
        //맵로드
        for (int i = 0; i < savedfiles.Length; i++)
        {
            StringBuilder mapStringBuilder = new StringBuilder();
            mapStringBuilder.Append(Path.GetFileName(savedfiles[i])).Replace(TILEDATA, "").Replace(".json", "");

            //Key
            string mapName = mapStringBuilder.ToString();
            SceneName type = SceneChangeManager.Instance.FindSceneNameByString(mapName);

            //Value
            string mapInfo = File.ReadAllText(savedfiles[i]);
            MapData mapData = JsonUtility.FromJson<MapData>(mapInfo);

            MapDataBase.Instance.mapTileData.Add(type, mapData);
        }
    }

    public MapData LoadInitialMapData(SceneName sceneName)
    {
        StringBuilder InitialFileString = new StringBuilder();
        var initialMap = Resources.Load(InitialFileString.Append(RESOURCEINITIALPATH)
            .Append(SceneChangeManager.Instance.SCENENAMEDICT[sceneName].Name).Append(TILEDATA).ToString()) as TextAsset;

        if(sceneName == SceneName.FarmScene)
        {
            AnalyticsManager.SendFunnelStepEvent(2);
        }
        return JsonUtility.FromJson<MapData>(initialMap.text);
    }
}
