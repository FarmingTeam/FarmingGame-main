
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSeed
{
    public int seedType = -1;
    public bool isPlanted = false;
    public bool isGrowth = false;
    public bool isWatered = false;
    public int remainGrowth = -1;
    public int remainDate = -1;
    public int lastWateredDate = -1;

    public ItemData itemData;

    TileBase seedTileBase;
    TileBase growthTileBase;
    TileBase cropTileBase;


    public void UpdateRemainingDate()
    {
        if (isWatered && lastWateredDate != TimeManager.Instance.GetActualUpdateDate())
        {
            isWatered = false;
            lastWateredDate = -1;

            if (seedType == -1)
                return;
            remainGrowth = remainGrowth - 1;
            remainDate = remainDate - 1;

            if (remainDate <= 0)
            {
                isPlanted = true;
            }
            if (remainGrowth <= 0)
            {
                isGrowth = true;
            }
        }
    }

    public TileBase SeedState()
    {
        UpdateRemainingDate();
        if (isPlanted)
            return cropTileBase;
        if (isGrowth)
        {
            return growthTileBase;
        }
        return seedTileBase;
    }

    public void InitSeed(SeedData seed)
    {
        seedType = seed.itemID;
        if (seedType == -1)
        {
            return;
        }

        int itemID = seedType + 1000;
        itemData = ResourceManager.Instance.GetItem(itemID);
        remainGrowth = (seed.growTime+1) / 2;
        remainDate = seed.growTime;
        seedTileBase = seed.seedTileBase;
        growthTileBase = seed.growthTileBase;
        cropTileBase = seed.cropTileBase;
    }
    public MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        if ((tile.floorInteractionType == FloorInteractionType.Dirt || tile.floorInteractionType == FloorInteractionType.WetDirt)
                && tool.equipmentType == EquipmentType.SeedBasket && tool.equipmentExtra > 0)
        {
            if (seedType != -1)
                return MinigameInteractionType.None;
            return MinigameInteractionType.WithoutMinigame;
        }
        else if (isPlanted && tool.equipmentType == EquipmentType.Sickle)
            return MinigameInteractionType.QTE;
        return MinigameInteractionType.None;
    }


    public bool Interaction(Equipment tool, Tile tile, out TileBase tileBase)
    {
        if (seedType == -1 && (tile.floorInteractionType == FloorInteractionType.Dirt || tile.floorInteractionType == FloorInteractionType.WetDirt)
                && tool.equipmentType == EquipmentType.SeedBasket && tool.equipmentExtra > 0)
        { 
            try
            {
                SeedData seed = ResourceManager.Instance.GetItem(tool.equipmentExtra) as SeedData;
                InitSeed(seed);
                PlayerInventory.Instance.SubtractItemQuantity(tool.equipmentExtra, 1);

                tileBase = SeedState();
                return true;
            }
            catch
            {
                Debug.LogError("올바르지 않은 씨앗 데이터입니다. : " + tool.equipmentExtra);
                tileBase = null;
                return false;
            }
        }

        //씨앗 수확 로직
        else if (isPlanted && tool.equipmentType == EquipmentType.Sickle)
        {
            // 드랍 오브젝트 생성(ItemData에서 작물 아이템의 데이터를 끌어와 타일 위에 띄우기)
            int cropId = seedType + 1000;
            ItemData cropItem = ResourceManager.Instance.GetItem(cropId);
            GameObject drop = new GameObject($"{cropItem.itemName} Drop");
            drop.AddComponent<ItemToPlayer>().Init(cropItem);
            drop.transform.localPosition = MapControl.Instance.map.TileSeed.CellToLocal((Vector3Int)tile.pos);

            // 드랍 아이템의 생김새는 아이콘과 같음.
            var spr = drop.AddComponent<SpriteRenderer>();
            spr.sprite = cropItem.itemIcon;
            spr.sortingOrder = 1000;

            tile.seed.ResetSeed();
            tileBase = null;
            return true;
        }

        tileBase = null;
        return false;
    }

    void ResetSeed()
    {
        seedType = -1;
        itemData = null;
        isPlanted = false;
        isWatered = false;
        isGrowth = false;
        remainGrowth = -1;
        remainDate = -1;
        lastWateredDate = -1;
    }
}
