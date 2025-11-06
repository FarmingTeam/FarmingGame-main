
using UnityEngine;

public class BigTreeTileBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        if (tool.equipmentType == EquipmentType.Axe)
        {
            //Drop Rock Item here
            ItemData woodItem = ResourceManager.Instance.GetItem(3);
            GameObject drop = new GameObject($"{woodItem.itemName} Drop");
            drop.AddComponent<ItemToPlayer>().Init(woodItem);
            drop.transform.localPosition = MapControl.Instance.map.TileSeed.CellToLocal((Vector3Int)tile.pos);

            // 드랍 아이템의 생김새는 아이콘과 같음.
            var spr = drop.AddComponent<SpriteRenderer>();
            spr.sprite = woodItem.itemIcon;
            spr.sortingOrder = 1000;

            chunkData = TileDataBase.Instance.GetChunkDataByID((int)ChunkType.TreeBigBroken);
            return true;
        }
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if (tool.equipmentType == EquipmentType.Axe)
            return MinigameInteractionType.QTE;
        return MinigameInteractionType.None;
    }
}
