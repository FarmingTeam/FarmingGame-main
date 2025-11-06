using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum ChunkInteractionType
{
    None = 0,
    Tree,
    BigTree,
    Rock,
    Weed, 
    Branch,
    Flower,
    Mushroom,
    Door,
    Bed ,
    GrowTree,
    WaterSource,
    FoodSource,
    BoardSource,
    UpgradeSource
}

[System.Serializable]
public enum ChunkType
{
    None = 0,
    Stonetype1 = 10,
    Stonetype2 = 11,
    Branch = 20,
    Weed = 30,
    TreeSmallZero =  40,
    TreeSmallOne = 41,
    TreeSmallTwo = 42,
    TreeSmallComplete = 43,
    TreeBigZero = 50,
    TreeBigOne = 51,
    TreeBigTwo = 52,
    TreeBigThree = 53,
    TreeBigFour = 54,
    TreeBigComplete = 55,
    TreeBigBroken = 59,
    Flowertype1 = 61,
    Flowertype2 = 62,
    Flowertype3 = 63,
    Flowertype4 = 64,
    Mushroomtype1 = 81,
    Mushroomtype2 = 82,
    Mushroomtype3 = 83,
    Mushroomtype4 = 84,
    Mushroomtype5 = 85,
    Doortype1 = 100,
    Doortype2 = 101,
    Doortype3 = 102,
    Doortype4 = 103,
    Bed = 150,
    Well = 110,
    FoodProcessor = 200,
    BulletinBoard = 210,
    Anvil = 220
}

[System.Serializable]
public struct OneDimensionTileBase
{
    public TileBase[] Tiles;
}

[System.Serializable]
[CreateAssetMenu(fileName = "ChunkData", menuName = "TileInfo/ChunkData/Chunk")]
public class ChunkData : ScriptableObject
{
    [SerializeField] public ChunkType chunkType;
    [SerializeField] public ChunkInteractionType[] interactionType;
    [SerializeField] public OneDimensionTileBase[] tileBases;
}


