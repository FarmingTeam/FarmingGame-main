using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIUpgrade : UIBase
{
    [SerializeField] List<UIUpgradeSlot> upgradeSlots = new List<UIUpgradeSlot>();
    [SerializeField] GameObject checkbox;


    GameObject checkboxgo;
    UIUpgradeSlot currentSlot;


    private void OnEnable()
    {
        RefreshUI();
        PlayerEquipment.Instance.onEquipmentChange += RefreshUI;
    }


    private void OnDisable()
    {
        PlayerEquipment.Instance.onEquipmentChange -= RefreshUI;
    }


    public void RefreshUI()
    {
        upgradeSlots[0].SetUpgradeSlot(ResourceManager.Instance.GetUpgrade(10002));
        upgradeSlots[1].SetUpgradeSlot(ResourceManager.Instance.GetUpgrade(10003));
        upgradeSlots[2].SetUpgradeSlot(ResourceManager.Instance.GetUpgrade(10004));
        upgradeSlots[3].SetUpgradeSlot(ResourceManager.Instance.GetUpgrade(10006));
        Debug.Log("업그레이드 리프레시");
    }

    public void SelectSlot(UIUpgradeSlot slot)
    {
        if(slot!=currentSlot)
        {
            foreach( var upgradeslot in upgradeSlots )
            {
                upgradeslot.outline.enabled=false;
            }
            slot.outline.enabled=true;
            currentSlot = slot;
        }

        var tool = PlayerEquipment.Instance.GetPlayerEquipment(currentSlot.slotEquip.toolId);

        if (currentSlot!=null&&currentSlot.slotEquip!=null&&tool!=null&&tool.equipmentUpgrade!=2)
        {
            if(checkboxgo==null)
            {
                checkboxgo= Instantiate(checkbox,FindFirstObjectByType<UISwitch>().transform,false);
            }
            checkboxgo.SetActive(true);
            checkboxgo.transform.SetAsLastSibling();
            var ui = checkboxgo.GetComponentInChildren<ShopConfirmUI>(true);
            ui.Show("정말 업그레이드 하시겠습니까?", OnOpenPopUI, OnClosePopUI);
        }
        PlayerEquipment.Instance.onEquipmentChange?.Invoke();
        

    }

    public void OnCloseButton()
    {
        TimeManager.Instance.PauseTime(false);
        MapControl.Instance.player.controller.IsInteract=false;
    }




    void OnOpenPopUI()
    {



        var tool=PlayerEquipment.Instance.GetPlayerEquipment(currentSlot.slotEquip.toolId);
        if(tool.equipmentUpgrade==0)
        {
            PlayerEquipment.Instance.UpgradeEquipment(currentSlot.slotEquip.toolId, 1);
        }
        else if(tool.equipmentUpgrade == 1)
        {
            PlayerEquipment.Instance.UpgradeEquipment(currentSlot.slotEquip.toolId, 2);
        }
        
        
    }

    void OnClosePopUI()
    {
        checkboxgo.SetActive(false);
    }
}
