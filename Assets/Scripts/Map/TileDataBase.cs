using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum MinigameInteractionType
{
    None = -1,
    WithoutStaminaConsume = 0,
    WithoutMinigame = 1,
    QTE = 2,
    Fishing = 3,
    Cooking = 4
}

//타일맵 데이터 베이스
//데이터 필요할 시 TileDataBase에서 찾아서 참조
public class TileDataBase : Singleton<TileDataBase>
{
    const string TILEFLOORDATAPATH = "MapData/TileFloorData";
    const string CHUNKDATAPATH = "MapData/ChunkData";

    public TileFloor[] tileFloors;
    public ChunkData[] chunkDatas;

    protected override void Initialize()
    {
        tileFloors = Resources.LoadAll<TileFloor>(TILEFLOORDATAPATH);
        chunkDatas = Resources.LoadAll<ChunkData>(CHUNKDATAPATH);
    }

    readonly public Dictionary<FloorInteractionType, TileFloorInteraction> FLOORACTIONPAIR = new Dictionary<FloorInteractionType, TileFloorInteraction>
    {
        { FloorInteractionType.None, new ElseTileBehaviour()},
        { FloorInteractionType.Grass, new GrassTileBehaviour()},
        { FloorInteractionType.Dirt, new DirtTileBehaviour()},
        { FloorInteractionType.WetDirt, new WetDirtTileBehaviour()},
        { FloorInteractionType.Water, new WaterTileBehaviour()}
    };

    readonly public Dictionary<ChunkInteractionType, TileObjectInteraction> OBJECTACTIONPAIR = new Dictionary<ChunkInteractionType, TileObjectInteraction>
    {
        { ChunkInteractionType.None, new NoneChunkBehaviour()},
        { ChunkInteractionType.Tree, new TreeTileBehaviour()},
        { ChunkInteractionType.BigTree, new BigTreeTileBehaviour()},
        { ChunkInteractionType.Rock, new RockTileBehaviour()},
        { ChunkInteractionType.Door, new DoorTileBehaviour()},
        { ChunkInteractionType.Flower, new FlowerTypeBehaviour()},
        { ChunkInteractionType.Mushroom, new MushroomTypeBehaviour() },
        { ChunkInteractionType.Weed, new WeedTypeBehaviour() },
        { ChunkInteractionType.Bed, new BedTypeBehaviour()},
        { ChunkInteractionType.GrowTree, new TreeGrowBehaviour()},
        { ChunkInteractionType.WaterSource, new WaterSourceBehaviour()},
        { ChunkInteractionType.FoodSource, new FoodSourceBehaviour()},
        { ChunkInteractionType.BoardSource, new BoardTypeBehaviour()},
        { ChunkInteractionType.UpgradeSource, new UpgradeTypeBehaviour()}

    };

    public TileFloor GetTileFloorByType(FloorInteractionType type)
    {
        TileFloor result = tileFloors.First(state => state.floorType == type);
        return result;
    }

    public ChunkData GetChunkDataByID(int id)
    {
        ChunkData result = chunkDatas.First(data => data.chunkType == (ChunkType)id);
        return result;
    }

    public SeedData GetSeedDataByID(int id)
    {
        SeedData result = ResourceManager.Instance.GetItem(id) as SeedData;
        return result;
    }

}
