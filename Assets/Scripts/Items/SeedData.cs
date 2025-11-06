using System;
using UnityEngine.Tilemaps;

[Serializable]
public class SeedData : ItemData
{
    public int growTime;

    public string seedTilePath;
    public string growthTilePath;
    public string cropTilePath;

    public TileBase seedTileBase;
    public TileBase growthTileBase;
    public TileBase cropTileBase;
    public SeedData(int itemID, string itemName, string itemPath, string itemType, string itemDescription, int itemEffect,int maxNum, int growTime,string seedTilePath, string growthTilePath, string cropTilePath) : base(itemID, itemName, itemPath,itemType,itemDescription, itemEffect, maxNum)
    {
        this.itemID = itemID;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.itemPath = itemPath;
        if (itemType == "기타")
        {
            this.itemType = ItemType.Others;
        }
        else if (itemType == "NPC선물")
        {
            this.itemType = ItemType.Present;
        }
        else if (itemType == "씨앗")
        {
            this.itemType = ItemType.Seed;
        }
        else if (itemType == "물고기")
        {
            this.itemType = ItemType.Fish;
        }
        else if (itemType == "자원")
        {
            this.itemType = ItemType.Resource;
        }
        else if (itemType == "작물")
        {
            this.itemType = ItemType.Crop;
        }
        else if (itemType == "요리재료")
        {
            this.itemType = ItemType.Ingredient;
        }
        else if (itemType == "요리")
        {
            this.itemType = ItemType.Food;
        }
        else if (itemType == "포션")
        {
            this.itemType = ItemType.Potion;
        }
        this.maxQuantity = maxNum;
        isStackable = maxQuantity > 1 ? true : false;
        this.itemEffect = itemEffect;



        this.growTime = growTime;
        this.seedTilePath = seedTilePath;
        this.growthTilePath = growthTilePath;
        this.cropTilePath = cropTilePath;


    }
    ///이 생성자는 처음에 씨앗 데이터를 로드할때 채워넣는것, 후에 위에있는 생성자로 병합예정
    
}
