using UnityEngine;
using System;






public enum ItemType
{
    Others,
    Seed,
    Present,
    Crop,
    Food,
    Resource,
    Fish,
    Potion,
    Ingredient

}

[Serializable]
public class ItemData:ScriptableObject
{
    public int itemID;
    public string itemName;
    public string itemDescription;
    public string itemPath;
    public ItemType itemType;
    public Sprite itemIcon;
    public int maxQuantity;
    public bool isStackable;
    public int itemEffect;


    public ItemData()
    {
    }

    public ItemData(int itemID, string itemName, string itemPath, string itemType, string itemDescription, int itemEffect, int maxNum)
    {
        this.itemID = itemID;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.itemPath = itemPath;
        if(itemType=="기타")
        {
            this.itemType=ItemType.Others;
        }
        else if(itemType== "NPC선물")
        {
            this.itemType = ItemType.Present;
        }
        else if(itemType== "씨앗")
        {
            this.itemType = ItemType.Seed;
        }
        else if(itemType== "물고기")
        {
            this.itemType = ItemType.Fish;
        }
        else if(itemType=="자원")
        {
            this.itemType = ItemType.Resource;
        }
        else if(itemType=="작물")
        {
            this.itemType = ItemType.Crop;
        }
        else if(itemType=="요리재료")
        {
            this.itemType = ItemType.Ingredient;
        }
        else if(itemType=="요리")
        {
            this.itemType=ItemType.Food;
        }
        else if(itemType=="포션")
        {
            this.itemType=ItemType.Potion;
        }
        this.maxQuantity = maxNum;
        isStackable=maxQuantity>1? true: false;
        this.itemEffect = itemEffect;
    }
}




[Serializable]
public class Item
{
    public ItemData itemData;
    public int currentQuantity;



    public Item( ItemData itemData)
    {
        this.itemData = itemData;
        this.currentQuantity = 1;
        
    }

    public Item(ItemData itemData, int currentQuantity)
    {
        this.itemData = itemData;
        this.currentQuantity = currentQuantity;
    }
}




