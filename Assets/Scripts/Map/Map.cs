using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [Header("ID")]
    public string sceneName;
    [Header("타일맵 전체 범위 참조")]
    [SerializeField] public Tilemap AllLayer;
    [Header("타일맵 0번 레이어")]
    [SerializeField] Tilemap WaterLayer;
    [field: SerializeField] public Tilemap GrassLayer { get; private set; }
    [Header("타일맵 1번 레이어")]
    [SerializeField] Tilemap TileFloor;
    [Header("타일맵 2번 레이어")]
    [SerializeField] Tilemap TileObjectLayerTwo;
    [SerializeField] Tilemap TileObjectLayerFour;
    public Dictionary<Vector2Int, List<Vector2Int>> objectGraph = new Dictionary<Vector2Int, List<Vector2Int>>();
    public RandomChunkGenerator RandomChunkGen;
    [Header("타일맵 3번 레이어")]
    [SerializeField] public Tilemap TileSeed;

    [Header("타일 정보")]
    [SerializeField] public Tile[,] tiles;
    [SerializeField] FloorInteractionType InitialFloorType;


    //상호작용이 가능하지 않다면 MinigameInteractionType.None을 리턴
    //이외에는 상호작용이 어떻게든 가능함
    //주의사항 : Update로 하기에는 좀 버거운 코드이기 때문에 실행 조건에 맞춰서 실행

    // 실행조건 1 : 보고있는 lookPos 타일 위치 변경시 + map 변경시
    // 실행조건 2 : 들고있는 장비 변경시

    public MinigameInteractionType IsInteractMinigame(Vector2Int lookPos, Equipment tool)
    {
        if (lookPos.x < 0 || lookPos.y < 0) return MinigameInteractionType.None;
        if (lookPos.x >= tiles.GetLength(0) || lookPos.y >= tiles.GetLength(1)) return MinigameInteractionType.None ;

        //지역변수 설정
        Tile currentTile = tiles[lookPos.x, lookPos.y];

        //Floor 미니게임 상호작용이 가능하다면 땅 인터렉션으로 미니게임 실행
        if (TileDataBase.Instance.FLOORACTIONPAIR.ContainsKey(currentTile.floorInteractionType))
        {
            MinigameInteractionType floorRes = TileDataBase.Instance.FLOORACTIONPAIR[currentTile.floorInteractionType].IsInteractMinigame(tool, currentTile);
            if (floorRes != MinigameInteractionType.None)
                return floorRes;
        }

        //Chunk 미니게임 상호작용이 가능하다면 Chunk인터렉션으로 미니게임 실행
        //단 Floor이 먼저 검사됨
        if (TileDataBase.Instance.OBJECTACTIONPAIR.ContainsKey(currentTile.chunkData.interactionType[currentTile.pos.y - GetChunkStartPos(currentTile.pos).y]))
        {
            MinigameInteractionType chunkRes = TileDataBase.Instance.OBJECTACTIONPAIR[currentTile.chunkData.interactionType[currentTile.pos.y - GetChunkStartPos(currentTile.pos).y]].IsInteractMinigame(tool, currentTile);
            if (chunkRes != MinigameInteractionType.None)
                return chunkRes;
        }
        //Seed 미니게임 상호작용 3순위 처리
        if (currentTile.seed.IsInteractMinigame(tool, currentTile) != MinigameInteractionType.None)
            return currentTile.seed.IsInteractMinigame(tool, currentTile);

        //해당 상호작용이 불가능하다면, None 리턴
        return MinigameInteractionType.None;
    }


    public void OnPlayerInteract(Vector2Int lookPos, Equipment tool)
    {
        Debug.Log("플레이어 아이템" + tool);
        //범위 밖 예외처리
        if(lookPos.x < 0 || lookPos.y < 0) return;
        if (lookPos.x >= tiles.GetLength(0) || lookPos.y >= tiles.GetLength(1)) return;

        //지역변수 설정
        Tile currentTile = tiles[lookPos.x, lookPos.y];

        //1번 레이어 인터렉션
        TileDataBase.Instance.FLOORACTIONPAIR[currentTile.floorInteractionType]?.Interaction(tool, currentTile);
        MapDataBase.Instance.UpdateFloor(lookPos, currentTile.floorInteractionType);
        SetTileFloor(lookPos);

        //2번 레이어 인터렉션
        TileObjectAction(currentTile, tool);

        //3번 레이어 인터렉션
        if (currentTile.seed.Interaction(tool, currentTile, out TileBase tileBase))
        {
            MapDataBase.Instance.UpdateSeed(lookPos, currentTile.seed);
            SetTileSeed(lookPos, tileBase);
        }
    }

    public void SetMap(MapData mapData)
    {
        int UpdateDiff = TimeManager.Instance.GetActualUpdateDate() - mapData.LastUpdate;
        //타일 크기 지정 및 초기화
        tiles = new Tile[mapData.MapSize[0], mapData.MapSize[1]];
        for (int i = 0; i < mapData.MapSize[0]; i++)
            for (int j = 0; j < mapData.MapSize[1]; j++)
            {
                tiles[i, j] = new Tile();
                tiles[i, j].pos = new Vector2Int(i, j);
                tiles[i, j].floorInteractionType = InitialFloorType;
            }
        
        //Floor의 물 깔기
        List<Vector2Int> tilePositions = new List<Vector2Int>();
        BoundsInt bounds = WaterLayer.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                if (WaterLayer.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    tiles[x, y].floorInteractionType = FloorInteractionType.Water;
                }
            }
        }
        



        //Floor 깔기
        for (int i = 0; i < mapData.TileFloor.Count; i++)
        {
            MapFloorData t = mapData.TileFloor[i];
            tiles[t.Pos[0], t.Pos[1]].floorInteractionType = (FloorInteractionType)t.FloorType;

            //날짜에 대비해서 씨앗 이 없는 농경지라면 녹지로 변경
            MapSeedData seedData = (from p in mapData.TileSeed where (p.Pos[0] == t.Pos[0] && p.Pos[1] == t.Pos[1]) select p).FirstOrDefault();
            if (UpdateDiff != 0 && 
                ( seedData == null || seedData.SeedType == -1) &&
                (tiles[t.Pos[0], t.Pos[1]].floorInteractionType == FloorInteractionType.Dirt || tiles[t.Pos[0], t.Pos[1]].floorInteractionType == FloorInteractionType.WetDirt))
            {
                tiles[t.Pos[0], t.Pos[1]].floorInteractionType = FloorInteractionType.Grass;
                MapDataBase.Instance.UpdateFloor(new Vector2Int(t.Pos[0], t.Pos[1]), tiles[t.Pos[0], t.Pos[1]].floorInteractionType);
            }

            //Refactor : 도로변 제외
            /*
            if (GrassLayer.GetTile(new Vector3Int(t.Pos[0], t.Pos[1])) == null)
            {
                tiles[t.Pos[0], t.Pos[1]].floorInteractionType = FloorInteractionType.None;
                MapDataBase.Instance.UpdateFloor(new Vector2Int(t.Pos[0], t.Pos[1]), tiles[t.Pos[0], t.Pos[1]].floorInteractionType);
            }
            */
            SetTileFloor(new Vector2Int(t.Pos[0], t.Pos[1]));
        }


        

        //Object 깔기
        for (int i = 0; i < mapData.TileObject.Count; i++)
        {
            MapChunkData t = mapData.TileObject[i];
            ChunkData data = TileDataBase.Instance.GetChunkDataByID(t.ChunkType);
            tiles[t.Pos[0], t.Pos[1]].chunkData = data;
            //나무 업데이트
            if (data.interactionType[0] == ChunkInteractionType.GrowTree)
            {
                TileDataBase.Instance.OBJECTACTIONPAIR[ChunkInteractionType.GrowTree].Interaction(null, tiles[t.Pos[0], t.Pos[1]], out data);
                tiles[t.Pos[0], t.Pos[1]].chunkData = data;
                MapDataBase.Instance.UpdateChunk(new Vector2Int(t.Pos[0], t.Pos[1]), data);
            }
            ChunkControl.Instance.SetTileObjectInMap(new Vector2Int(t.Pos[0], t.Pos[1]), data);
        }
        //Random Chunk Generator 깔기
        if (RandomChunkGen != null)
        {
            RandomChunkGen.MushroomType.HarvestDates = new Queue<int>(mapData.RandomChunkGenMushroomHarvest);
            RandomChunkGen.FlowerType.HarvestDates = new Queue<int>(mapData.RandomChunkGenFlowerHarvest);
            RandomChunkGen.WeedType.HarvestDates = new Queue<int>(mapData.RandomChunkGenWeedHarvest);
            RandomChunkGen.RockType.HarvestDates = new Queue<int>(mapData.RandomChunkGenRockHarvest);
            RandomChunkGen.UpdateRandomGenChunks();
        }

        //Seed 깔기
        for (int i = 0; i < mapData.TileSeed.Count; i++)
        {
            MapSeedData t = mapData.TileSeed[i];
            //Refactor : 나중에 단순화
            if(t.SeedType != -1)
                tiles[t.Pos[0], t.Pos[1]].seed.InitSeed(TileDataBase.Instance.GetSeedDataByID(t.SeedType));
            tiles[t.Pos[0], t.Pos[1]].seed.isPlanted = t.IsPlanted;
            tiles[t.Pos[0], t.Pos[1]].seed.isGrowth = t.IsGrowth;
            tiles[t.Pos[0], t.Pos[1]].seed.isWatered = t.IsWatered;
            tiles[t.Pos[0], t.Pos[1]].seed.remainDate = t.RemainDate;
            tiles[t.Pos[0], t.Pos[1]].seed.remainGrowth = t.RemainGrowth;
            tiles[t.Pos[0], t.Pos[1]].seed.lastWateredDate = t.LastWateredDate;

            SetTileSeed(new Vector2Int(t.Pos[0], t.Pos[1]), tiles[t.Pos[0], t.Pos[1]].seed.SeedState());
            //물 건조 처리
            if (!tiles[t.Pos[0], t.Pos[1]].seed.isWatered && tiles[t.Pos[0], t.Pos[1]].seed.seedType != -1)
            {
                tiles[t.Pos[0], t.Pos[1]].floorInteractionType = FloorInteractionType.Dirt;
                SetTileFloor(new Vector2Int(t.Pos[0], t.Pos[1]));
            }
        }

        //Current Map Data의 날짜 업데이트
        mapData.LastUpdate = TimeManager.Instance.GetActualUpdateDate();
    }

    public void SetTileFloor(Vector2Int index)
    {
        TileFloor.SetTile((Vector3Int)index, TileDataBase.Instance.GetTileFloorByType(tiles[index.x, index.y].floorInteractionType).tileBase);
    }

    public void SetTileObject(Vector2Int index, TileBase tile, int layer)
    {
        if (tile == null)
        {
            TileObjectLayerTwo.SetTile((Vector3Int)index, tile);
            TileObjectLayerFour.SetTile((Vector3Int)index, tile);
        }
        else if (layer == 2)
            TileObjectLayerTwo.SetTile((Vector3Int)index, tile);
        else if (layer == 4)
            TileObjectLayerFour.SetTile((Vector3Int)index, tile);
    }

    public void SetTileSeed(Vector2Int index, TileBase tile)
    {
        TileSeed.SetTile((Vector3Int)index, tile);
    }

    public void TileObjectAction(Tile tile, Equipment equipment)
    {
        if (!objectGraph.ContainsKey(tile.pos))
            return;
        Vector2Int startPos = GetChunkStartPos(tile.pos);
        ChunkData changedChunk = null;
        //시작지점 타일 오브젝트 업데이트
        bool? isChanged = TileDataBase.Instance.OBJECTACTIONPAIR[tile.chunkData.interactionType[tile.pos.y - startPos.y]]
            ?.Interaction(equipment, tile, out changedChunk);
        //만약 변경사항이 없다면 종료
        if (isChanged != true)
            return;

        //변경사항 반영
        List<Vector2Int> updatedpos = ChunkControl.Instance.SetTileObjectInMap(startPos, changedChunk);

        //시작지점 기점으로 저장정보 업데이트
        for(int i = 0;i < updatedpos.Count;i++)
            MapDataBase.Instance.UpdateChunk(updatedpos[i], changedChunk);
    }

    public bool IsTileEmpty(Vector2Int pos)
    {
        Tile tile = tiles[pos.x, pos.y];
        if (tile.floorInteractionType != FloorInteractionType.None && tile.floorInteractionType != FloorInteractionType.Grass)
            return false;
        if (TileObjectLayerTwo.GetTile((Vector3Int)pos) != null || TileObjectLayerFour.GetTile((Vector3Int)pos) != null)
            return false;
        if (tile.seed.seedType != -1)
            return false;
        return true;
    }

    //연결된 시작지점 탐색
    Vector2Int GetChunkStartPos(Vector2Int pos)
    {
        Vector2Int startPos = pos;
        if (objectGraph.ContainsKey(pos))
        {
            for (int i = 0; i < objectGraph[pos].Count; i++)
            {
                Vector2Int connectedpos = objectGraph[pos][i];

                if (connectedpos.x < startPos.x)
                    startPos.x = connectedpos.x;
                if (connectedpos.y < startPos.y)
                    startPos.y = connectedpos.y;
            }
        }
        return startPos;
    }

    public bool IsFlowerOrMushroom(Vector2Int pos)
    {
        Vector2Int startPos = GetChunkStartPos(pos);
        ChunkInteractionType interactionType = tiles[pos.x, pos.y].chunkData.interactionType[pos.y - startPos.y];
        switch (interactionType)
        {
            case ChunkInteractionType.Flower:
            case ChunkInteractionType.Mushroom:
                return true;
            default:
                return false;
        }

    }
}
