using UnityEngine;

public class RockTileBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        if (tool.equipmentType == EquipmentType.Pickaxe)
        {
            //Drop Rock Item here
            ItemData rockItem = ResourceManager.Instance.GetItem(2);
            GameObject drop = new GameObject($"{rockItem.itemName} Drop");
            drop.AddComponent<ItemToPlayer>().Init(rockItem);
            drop.transform.localPosition = MapControl.Instance.map.TileSeed.CellToLocal((Vector3Int)tile.pos);

            // 드랍 아이템의 생김새는 아이콘과 같음.
            var spr = drop.AddComponent<SpriteRenderer>();
            spr.sprite = rockItem.itemIcon;
            spr.sortingOrder = 1000;

            MapControl.Instance.map.RandomChunkGen.RockType.HarvestDates.Enqueue(TimeManager.Instance.GetActualUpdateDate());
            MapControl.Instance.map.RandomChunkGen.UpdateMapDataBase();
            chunkData = TileDataBase.Instance.GetChunkDataByID((int)ChunkType.None);
            return true;
        }
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.Pickaxe)
            return MinigameInteractionType.QTE;
        return MinigameInteractionType.None;
    }
}
