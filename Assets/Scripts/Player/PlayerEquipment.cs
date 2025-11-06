using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerEquipment : Singleton<PlayerEquipment>
{
    public List<Equipment> equipList=new List<Equipment>();


    public Action onEquipmentChange;


    private string JsonPath => Path.Combine(Application.persistentDataPath);
    private const string FILENAME = "equipment.Json";

    int equipMaxNum = 7;


    public void Init()
    {
        if (equipList.Count > 0)
        {
            return;
        }
        else
        {
            for(int i = 0; i < equipMaxNum; i++)
            {
                equipList.Add(ResourceManager.Instance.GetEquipment(0));
            }
            

        }
    }


   
    public void AddEquipmentByID(int id)
    {
        CollectionManager.Instance.ObtainedItem(id, 1);
        var emptySlotNum=FindEmptySlotNum();
        if (emptySlotNum == -1) 
        {
            Debug.Log("빈 장비슬롯이 없습니다");
        }
        else
        {
            equipList[emptySlotNum]=ResourceManager.Instance.GetEquipment(id);
        }
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(3));
    }

    int FindEmptySlotNum()
    {
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList[i].equipmentType == EquipmentType.None)
            {
                return i;
            }
        }
        return -1;
    }

    public void SwitchEquipmentPlaces(int firstIndex, int secondIndex)
    {
        int CurrentSelectedIndex = -1;
        //그냥 정말 쉽게 두번째꺼랑 첫번쨰 자리를 바꾸면 된다.
        for(int i=0; i<equipList.Count; i++)
        {
            if (equipList[i]==MapControl.Instance.player.tool.CurrentEquip)
            {
                CurrentSelectedIndex = i; //현재 골라진 슬롯 번호 저장
                break;
            }
        }
        var temp=equipList[firstIndex];
        equipList[firstIndex]=equipList[secondIndex];
        equipList[secondIndex]=temp;
        onEquipmentChange?.Invoke();
        if(firstIndex==CurrentSelectedIndex)
        {
            MapControl.Instance.player.tool.SelectQuickslot(firstIndex+1);
        }
        else if(secondIndex==CurrentSelectedIndex)
        {
            MapControl.Instance.player.tool.SelectQuickslot(secondIndex+1);
        }
    }



 
    public void ChangeEquipmentExtra(EquipmentType equipmentType,int equipmentExtra)
    {
        foreach(Equipment equip in equipList)
        {
            if(equip.equipmentType == equipmentType)
            {
                if(equipmentType==EquipmentType.SeedBasket)
                {
                    //현재 SeedID를 알려줌
                    equip.equipmentExtra = equipmentExtra;
                }
                else if(equipmentType == EquipmentType.WateringCan)
                {
                    //현재 물 게이지를 알려줌
                    equip.equipmentExtra += equipmentExtra;
                    equip.equipmentExtra=Mathf.Clamp(equip.equipmentExtra,0,equip.equipmentMaxRate);
                }
                
            }
        }
    }

    public int CheckEquipmentExtra(EquipmentType equipmentType)
    {
        foreach (Equipment equip in equipList)
        {
            if(equip.equipmentType==equipmentType)
            {
                return equip.equipmentExtra;
            }
            
        }
        return -1;
    }
    int CheckEquipmentUpgradeStatus(EquipmentType equipmentType)
    {
        foreach (Equipment equip in equipList)
        {
            if (equip.equipmentType == equipmentType)
            {
                return equip.equipmentUpgrade;
            }

        }
        return -1;
    }

    Equipment CheckEquipment(EquipmentType equipmentType)
    {
        foreach (Equipment equip in equipList)
        {
            if (equip.equipmentType == equipmentType)
            {
                return equip;
            }

        }
        return null;
    }


    //이걸로 업그레이드 처리
    public void UpgradeEquipment(EquipmentType type,int upgradeLevel)
    {
        var equip = CheckEquipment(type);
        if(equip != null)
        {
            equip.equipmentUpgrade = upgradeLevel;
        }
        onEquipmentChange?.Invoke();
        
    }

    //이건 id사용
    public void UpgradeEquipment(int id, int upgradeLevel)
    {
        var equip = GetPlayerEquipment(id);
        if (equip != null)
        {
            
            if(equip.equipmentUpgrade==0)
            {
                if(PlayerManager.Instance.playerMoney.TrySpend(ResourceManager.Instance.GetUpgrade(id).requireGold))
                {
                    equip.equipmentUpgrade = upgradeLevel;
                }
            }
            else if(equip.equipmentUpgrade==1)
            {
                if (PlayerManager.Instance.playerMoney.TrySpend(ResourceManager.Instance.GetUpgrade(id).nextrequireGold))
                {
                    equip.equipmentUpgrade = upgradeLevel;
                }
            }
            
        }
        Debug.Log("업글");

        onEquipmentChange?.Invoke();

    }


    public Equipment GetPlayerEquipment(int id)
    {
        
        foreach (var equip in equipList)
        {
            if(equip.equipmentID==id)
            {
                return equip;
            }

        }
        return null;
        
    }



    public void SubscribeEquipmentChange(Action action)
    {
        onEquipmentChange += action;
    }

    public void UnsubscribeEquipmentChange(Action action)
    {
        onEquipmentChange-= action; 
    }

    public void SaveEquipmentStatus(int slotNumber)
    {
        EquipmentListWrapper equipmentListWrapper = new EquipmentListWrapper();
        foreach (var equip in equipList)
        {
            equipmentListWrapper.equipmentWrapperList.Add(new KeyValueParsing(equip.equipmentID,equip.equipmentUpgrade));
        }

        string json = JsonUtility.ToJson(equipmentListWrapper, true);

        StringBuilder savePath = new StringBuilder();
        savePath.Append(JsonPath).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME);

        File.WriteAllText(savePath.ToString(), json);

        Debug.Log("장비 저장 완료: " + JsonPath);
    }

    public void LoadEquipmentStatus(int slotNumber)
    {
        StringBuilder loadPath = new StringBuilder();
        loadPath.Append(JsonPath).Append('/').Append(SaveManager.SAVEFILEPATH).Append(slotNumber).Append('/').Append(FILENAME);
        if (File.Exists(loadPath.ToString()))
        {

            Debug.Log("장비 파일 있음");
            string json = File.ReadAllText(loadPath.ToString());

            EquipmentListWrapper equipmentListWrapper = JsonUtility.FromJson<EquipmentListWrapper>(json);
            
            for(int i = 0;i< equipMaxNum;i++)
            {
                Equipment eq=(ResourceManager.Instance.GetEquipment(equipmentListWrapper.equipmentWrapperList[i].key));
                equipList.Add(eq);
                eq.equipmentUpgrade= equipmentListWrapper.equipmentWrapperList[i].value;
            }

        }
        onEquipmentChange?.Invoke();
    }
}


[Serializable]
public class EquipmentListWrapper
{
    public List<KeyValueParsing> equipmentWrapperList = new List<KeyValueParsing> ();
}
