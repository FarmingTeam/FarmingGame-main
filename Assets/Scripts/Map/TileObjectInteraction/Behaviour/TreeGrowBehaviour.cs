
using UnityEngine;

public class TreeGrowBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        int ID = (int)tile.chunkData.chunkType;
        int date = MapDataBase.Instance.currentMapData.LastUpdate;
        if (TimeManager.Instance.GetActualUpdateDate() - date == 0)
        {
            chunkData = tile.chunkData;
            return false;
        }
        while (TimeManager.Instance.GetActualUpdateDate() - date > 0)
        {
            ID++;
            date++;
            if (ID == (int)ChunkType.TreeBigComplete || ID == (int)ChunkType.TreeSmallComplete)
                break;
        }
        chunkData = TileDataBase.Instance.GetChunkDataByID(ID);
        return true;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.None;
    }
}
