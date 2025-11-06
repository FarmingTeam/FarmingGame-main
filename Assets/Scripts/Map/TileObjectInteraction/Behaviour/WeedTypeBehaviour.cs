using UnityEngine;

public class WeedTypeBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        if (tool.equipmentType == EquipmentType.None || tool.equipmentType == EquipmentType.Sickle)
        {
            MapControl.Instance.map.RandomChunkGen.WeedType.HarvestDates.Enqueue(TimeManager.Instance.GetActualUpdateDate());
            MapControl.Instance.map.RandomChunkGen.UpdateMapDataBase();
            PlayerInventory.Instance.AdditemsByID(1, 1);
            chunkData = TileDataBase.Instance.GetChunkDataByID((int)ChunkType.None);
            return true;
        }
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.None || tool.equipmentType == EquipmentType.Sickle)
            return MinigameInteractionType.QTE;
        return MinigameInteractionType.None;
    }
}
