
using System.Collections.Generic;

using UnityEngine;




public class UIToolBar : UIBase
{
    [SerializeField] GameObject uiQuickSlotPrefab;
    List<UIQuickSlot> quickSlotList=new List<UIQuickSlot>();
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < 7; i++)
        {
            GameObject go = Instantiate(uiQuickSlotPrefab, this.transform, false);
            quickSlotList.Add(go.GetComponent<UIQuickSlot>());
        }

        //여기에서 슬롯에 아이템 넣어주기
        for (int i = 0; i < quickSlotList.Count; i++)
        {
            quickSlotList[i].slotNumber = i + 1;
            quickSlotList[i].Init();
            quickSlotList[i].SetQuickSlot(PlayerEquipment.Instance.equipList[i]);
        }
        // 저장된 슬롯 로드
        if (PlayerManager.Instance.ToolIdx != -1)
        {
            InitialSelectSlot(PlayerManager.Instance.ToolIdx);
        }
    }

    public void SetToolBar()
    {

        for (int i = 0; i < quickSlotList.Count; i++)
        {
            quickSlotList[i].SetQuickSlot(PlayerEquipment.Instance.equipList[i]);
        }
    }





    private void OnEnable()
    {
        MapControl.Instance.player.tool.SubscribeToSelectionChange(SelectSlot);
        PlayerEquipment.Instance.SubscribeEquipmentChange(SetToolBar);
        SetToolBar();
    }
    private void OnDisable()
    {
        if (PlayerEquipment.Instance != null)
            PlayerEquipment.Instance.UnsubscribeEquipmentChange(SetToolBar);

        if (MapControl.Instance == null || MapControl.Instance.player == null)
            return;
        MapControl.Instance.player.tool.UnsubscribeToSelectionChange(SelectSlot);
    }




    public void SelectSlot()
    {

        
        foreach(var slot in quickSlotList)
        {
            slot.outline.enabled = false;
            if(slot.slotEquipment==MapControl.Instance.player.tool.CurrentEquip&&slot.slotEquipment.equipmentID!=0)
            {
                if(slot.slotEquipment.equipmentType==EquipmentType.SeedBasket)
                {
                    UIManager.Instance.OpenUI<UISeedBasket>();
                }
                else
                {
                    UIManager.Instance.CloseUI<UISeedBasket>();
                }
                slot.outline.enabled = true;
            }
            
        }
        
    }

    public void InitialSelectSlot(int slotnumber)
    {
        for (int i = 0; i < quickSlotList.Count; i++)
        {

            var slot = quickSlotList[i];
            if (i == slotnumber - 1)
            {
                slot.outline.enabled = true;
                if (slot.slotEquipment.equipmentType == EquipmentType.SeedBasket)
                {
                    UIManager.Instance.OpenUI<UISeedBasket>();
                }
                else
                {
                    UIManager.Instance.CloseUI<UISeedBasket>();
                }
            }
            else
                slot.outline.enabled=false;
        }
    }




    
    

    
}
