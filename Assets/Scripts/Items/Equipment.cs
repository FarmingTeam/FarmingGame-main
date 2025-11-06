using System;
using UnityEngine;


public enum EquipmentType
{
    None,
    Hoe,
    WateringCan,
    Axe,
    Pickaxe,
    SeedBasket,
    Sickle,
    Rod
}


[Serializable]
public class Equipment : ScriptableObject
{
    public int equipmentID;
    public string equipmentName;
    public string equipmentDescription;
    public EquipmentType equipmentType;
    public string equipmentPath;
    public Sprite equipmentIcon;
    public int equipmentUpgrade;


    public int equipmentExtra;

    //이 부분은 최대치( 물뿌리개의 경우 최대 물 양, 다른것의경우 필요시 내구도 등 최대치가 필요한 부분에 대한 필드)
    public int equipmentMaxRate;
    public Equipment(int equipmentID, string equipmentName, string equipmentDescription, string equipmentType, string equipmentPath)
    {
        this.equipmentID = equipmentID;
        this.equipmentName = equipmentName;
        this.equipmentDescription = equipmentDescription;
        this.equipmentPath = equipmentPath;

        if (equipmentType == "Hoe")
        {
            this.equipmentType = EquipmentType.Hoe;
        }
        else if (equipmentType == "WateringCan")
        {
            this.equipmentType = EquipmentType.WateringCan;
            this.equipmentExtra = 10;
            this.equipmentMaxRate = 10;
        }
        else if (equipmentType == "Axe")
        {
            this.equipmentType = EquipmentType.Axe;
        }
        else if (equipmentType == "Pickaxe")
        {
            this.equipmentType = EquipmentType.Pickaxe;
        }
        else if (equipmentType == "SeedBasket")
        {
            this.equipmentType = EquipmentType.SeedBasket;

            //이거에 대한 equipmentExtra는 seedData

        }
        else if(equipmentType=="Sickle")
        {
            this.equipmentType = EquipmentType.Sickle;
        }
        else if (equipmentType =="Empty")
        {
            this.equipmentType = EquipmentType.None;
        }
        else if( equipmentType =="Fishingrod")
        {
            this.equipmentType = EquipmentType.Rod;
        }
    }
}
