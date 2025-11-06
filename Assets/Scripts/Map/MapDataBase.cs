using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapDataBase : Singleton<MapDataBase>
{
    const string MAPPREFABPATH = "MapData/MapPrefabs";

    public Map[] MapPrefabs;

    public Dictionary<SceneName, MapData> mapTileData = new Dictionary<SceneName, MapData>();

    public MapData currentMapData = null;

    protected override void Initialize()
    {
        MapPrefabs = Resources.LoadAll<Map>(MAPPREFABPATH);
    }

    public void LoadCacheMap(SceneName sceneName)
    {
        if (!SceneChangeManager.Instance.SCENENAMEDICT.ContainsKey(sceneName))
            throw new System.Exception("Undefined Scene");
        if (!SceneChangeManager.Instance.SCENENAMEDICT[sceneName].IsMap)
            throw new System.Exception($"{sceneName} is not map");

        string sceneNameString = SceneChangeManager.Instance.SCENENAMEDICT[sceneName].Name;

        //맵 프리팹 불러오기
        if (MapControl.Instance.map == null)
        {
            Map mapPrefab = MapDataBase.Instance.MapPrefabs.First(data => data.sceneName == sceneNameString);
            MapControl.Instance.map = Instantiate(mapPrefab);
        }

        //만약 캐싱된 타일 데이터가 있다면, 해당 데이터를 맵에 불러온다
        if (mapTileData.ContainsKey(sceneName))
        {
            currentMapData = mapTileData[sceneName];
            MapControl.Instance.map.SetMap(mapTileData[sceneName]);
        }
        //만약 없다면, Initial 파일을 가지고 새로 생성한다
        else
        {
            MapData newData = MapSaveManager.Instance.LoadInitialMapData(sceneName);
            mapTileData.Add(sceneName, newData);
            currentMapData = newData;
            MapControl.Instance.map.SetMap(newData);
        }
    }

    public void SaveCacheMap(SceneName sceneName)
    {
        if (!SceneChangeManager.Instance.SCENENAMEDICT[sceneName].IsMap)
            return;
        if (!mapTileData.ContainsKey(sceneName))
            return;

        mapTileData[sceneName] = currentMapData;
        currentMapData = null;
    }

    public void UpdateFloor(Vector2Int pos, FloorInteractionType type)
    {
        MapFloorData result = currentMapData.TileFloor.FirstOrDefault(data => data.Pos[0] == pos.x && data.Pos[1] == pos.y);
        //좌표에 데이터가 없을 경우 추가
        if (result == null)
        {
            if (type == FloorInteractionType.None)
                return;
            MapFloorData newdata = new MapFloorData();
            newdata.FloorType = (int)type;
            newdata.Pos = new int[] { pos.x, pos.y };
            currentMapData.TileFloor.Add(newdata);
        }
        //좌표에 데이터가 있을경우 수정
        else
        {
            //타일의 종류가 None이되면 좌표 데이터 삭제
            if (type == FloorInteractionType.None)
            {
                currentMapData.TileFloor.Remove(result);
                return;
            }
            result.FloorType = (int)type;
        }
    }

    public void UpdateChunk(Vector2Int pos, ChunkData data)
    {
        MapChunkData result = currentMapData.TileObject.FirstOrDefault(data => data.Pos[0] == pos.x && data.Pos[1] == pos.y);
        //좌표에 데이터가 없을 경우 추가
        if (result == null)
        {
            if (data.chunkType == ChunkType.None)
                return;
            MapChunkData newdata = new MapChunkData();
            newdata.ChunkType = (int)data.chunkType;
            newdata.Pos = new int[] { pos.x, pos.y };
            currentMapData.TileObject.Add(newdata);
        }
        //좌표에 데이터가 있을경우 수정
        else
        {
            //오브젝트의 종류가 None이되면 좌표 데이터 삭제
            if (data.chunkType == ChunkType.None)
            {
                currentMapData.TileObject.Remove(result);
                return;
            }
            result.ChunkType = (int)data.chunkType;
        }
    }

    public void UpdateSeed(Vector2Int pos, TileSeed seed)
    {
        MapSeedData result = currentMapData.TileSeed.FirstOrDefault(data => data.Pos[0] == pos.x && data.Pos[1] == pos.y);
        //좌표에 데이터가 없을 경우 추가
        if (result == null)
        {
            if (seed.seedType == -1 && seed.isWatered == false)
                return;
            MapSeedData newdata = new MapSeedData();
            seed.UpdateRemainingDate();
            newdata.SeedType = seed.seedType;
            newdata.Pos = new int[] { pos.x, pos.y };
            newdata.RemainDate = seed.remainDate;
            newdata.RemainGrowth = seed.remainGrowth;
            newdata.IsPlanted = seed.isPlanted;
            newdata.IsGrowth = seed.isGrowth;
            newdata.LastWateredDate = seed.lastWateredDate;
            newdata.IsWatered = seed.isWatered;
            currentMapData.TileSeed.Add(newdata);
        }
        //좌표에 데이터가 있을경우 수정
        else
        {
            //씨앗의 종류가 None이되면 좌표 데이터 삭제
            if (seed.seedType == -1 && seed.isWatered == false)
            {
                currentMapData.TileSeed.Remove(result);
                return;
            }
            seed.UpdateRemainingDate();
            result.SeedType = (int)seed.seedType;
            result.RemainDate = seed.remainDate;
            result.RemainGrowth = seed.remainGrowth;
            result.IsPlanted = seed.isPlanted;
            result.IsGrowth = seed.isGrowth;
            result.LastWateredDate = seed.lastWateredDate;
            result.IsWatered = seed.isWatered;
        }
    }

}
