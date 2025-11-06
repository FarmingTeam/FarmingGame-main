using UnityEngine;

public class GrassTileBehaviour : TileFloorInteraction
{
    public override void Interaction(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.Hoe && tile.chunkData.chunkType == ChunkType.None)
        {
            tile.floorInteractionType = FloorInteractionType.Dirt;
        }
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.Hoe && tile.chunkData.chunkType == ChunkType.None)
            return MinigameInteractionType.QTE;
        return MinigameInteractionType.None;
    }
}
