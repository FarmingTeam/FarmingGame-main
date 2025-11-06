using UnityEngine;

public class WaterSourceBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        //만약 물뿌리개를 들고있다면으로 수정
        if (tool.equipmentType == EquipmentType.WateringCan)
        {
            tool.equipmentExtra = tool.equipmentMaxRate;
            UIManager.Instance.GetUI<UIToolBar>().SetToolBar();
        }
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.WateringCan)
            return MinigameInteractionType.WithoutMinigame;
        return MinigameInteractionType.None;
    }
}
