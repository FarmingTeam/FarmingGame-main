using UnityEngine;

public class WaterTileBehaviour : TileFloorInteraction
{
    public override void Interaction(Equipment tool, Tile tile)
    {
        //만약 물뿌리개를 들고있다면으로 수정
        if (tool.equipmentType == EquipmentType.WateringCan)
        {
            tool.equipmentExtra = tool.equipmentMaxRate;
            UIManager.Instance.GetUI<UIToolBar>().SetToolBar();
        }
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.WateringCan)
            return MinigameInteractionType.WithoutMinigame;
        else if (tool.equipmentType == EquipmentType.Rod)
            return MinigameInteractionType.Fishing;
        return MinigameInteractionType.None;
    }
}
