
using UnityEngine;

public class MushroomTypeBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        if ((Vector2Int)MapControl.Instance.player.tileReader.FrontCell() != tile.pos)
        {
            chunkData = null;
            return false;
        }
        PlayerInventory.Instance.AdditemsByID(ItemIDCalculator((int)tile.chunkData.chunkType),1);
        MapControl.Instance.map.RandomChunkGen.MushroomType.HarvestDates.Enqueue(TimeManager.Instance.GetActualUpdateDate());
        MapControl.Instance.map.RandomChunkGen.UpdateMapDataBase();
        chunkData = TileDataBase.Instance.GetChunkDataByID((int)ChunkType.None);
        return true;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.WithoutMinigame;
    }

    int ItemIDCalculator(int chunkID)
    {
        return chunkID - 70;
    }
}
