using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct RandomChunkInfo
{
    public int MaxQuantity;
    public int GenDelay;
    public Queue<int> HarvestDates;
    public ChunkData[] ChunkDatas;
}

public class RandomChunkGenerator : MonoBehaviour
{
    [SerializeField] Map currentMap;
    [SerializeField] public RandomChunkInfo MushroomType;
    [SerializeField] public RandomChunkInfo FlowerType;
    [SerializeField] public RandomChunkInfo WeedType;
    [SerializeField] public RandomChunkInfo RockType;
    BoundsInt grassBounds;

    public void Awake()
    {
        currentMap.GrassLayer.CompressBounds();
        grassBounds = currentMap.GrassLayer.cellBounds;
    }
    
    public Vector2Int PickRandomTile()
    {
        while (true)
        {
            int randomx = UnityEngine.Random.Range(grassBounds.x, grassBounds.xMax + 1);
            int randomy = UnityEngine.Random.Range(grassBounds.y, grassBounds.yMax + 1);
            Vector2Int startpos = new Vector2Int(randomx, randomy);
            if ( startpos.x < 0 || startpos.x >= currentMap.tiles.GetLength(0)
                || startpos.y <0 || startpos.y >= currentMap.tiles.GetLength(1)
                || currentMap.GrassLayer.GetTile((Vector3Int)startpos) == null
                || !currentMap.IsTileEmpty(startpos))
            {
                continue;
            }
            return startpos;
        }
    }

    void PlaceChunk(int index)
    {
        ChunkData randomChunk = GetInfoByIndex(index).ChunkDatas[UnityEngine.Random.Range(0, GetInfoByIndex(index).ChunkDatas.Length)];

        Vector2Int randomPos = PickRandomTile();
        currentMap.tiles[randomPos.x, randomPos.y].chunkData = randomChunk;
        ChunkControl.Instance.SetTileObjectInMap(randomPos, randomChunk);
        MapDataBase.Instance.UpdateChunk(randomPos, randomChunk);
    }

    public void UpdateRandomGenChunks()
    {
        for (int i = 0; i < 4; i++)
        {
            while (GetInfoByIndex(i).HarvestDates.Count != 0)
            {
                if (GetInfoByIndex(i).HarvestDates.Peek() == -1
                    || TimeManager.Instance.GetActualUpdateDate() - GetInfoByIndex(i).HarvestDates.Peek() >= GetInfoByIndex(i).GenDelay)
                {
                    PlaceChunk(i);
                    GetInfoByIndex(i).HarvestDates.Dequeue();
                }
                else
                    break;
            }
        }


        UpdateMapDataBase();
    }

    RandomChunkInfo GetInfoByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return MushroomType;
            case 1:
                return FlowerType;
            case 2:
                return WeedType;
            case 3:
                return RockType;
            default:
                throw new System.Exception("Undefined Index");
        }
    }
    public void UpdateMapDataBase()
    {
        MapDataBase.Instance.currentMapData.RandomChunkGenMushroomHarvest = MushroomType.HarvestDates.ToList();
        MapDataBase.Instance.currentMapData.RandomChunkGenFlowerHarvest = FlowerType.HarvestDates.ToList();
        MapDataBase.Instance.currentMapData.RandomChunkGenWeedHarvest = WeedType.HarvestDates.ToList();
        MapDataBase.Instance.currentMapData.RandomChunkGenRockHarvest = RockType.HarvestDates.ToList();
    }
}


