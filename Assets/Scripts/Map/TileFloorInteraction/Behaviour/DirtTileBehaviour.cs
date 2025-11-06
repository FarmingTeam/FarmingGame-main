using UnityEngine;

public class DirtTileBehaviour : TileFloorInteraction
{
    public override void Interaction(Equipment tool, Tile tile)
    {
        //만약 도구가 물뿌리개이고, 물이 차 있다면 물뿌려진타일로 변경
        if (tool.equipmentType == EquipmentType.WateringCan && tool.equipmentExtra > 0)
        {
            tile.floorInteractionType = FloorInteractionType.WetDirt;
            tile.seed.isWatered = true;
            tile.seed.lastWateredDate = TimeManager.Instance.GetActualUpdateDate();
            MapDataBase.Instance.UpdateSeed(tile.pos, tile.seed);
            tool.equipmentExtra--;
            UIManager.Instance.GetUI<UIToolBar>().SetToolBar();
        }
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.WateringCan && tool.equipmentExtra > 0 && tile.seed.seedType != -1)
            return MinigameInteractionType.WithoutMinigame;
        return MinigameInteractionType.None;
    }
}
