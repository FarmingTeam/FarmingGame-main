
using System.Collections.Generic;
using UnityEngine;

public class UIItemToolBar : UIBase
{
    [SerializeField] List<UISlot> itemQuickSlots = new List<UISlot>();


    private void OnEnable()
    {
        PlayerInventory.Instance.SubscribeOnItemChange(SetSlots);
        SetSlots();
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.UnsubscribeOnItemChange(SetSlots);
    }

    //임시

    public void SetSlots()
    {
        for (int i = 0; i < itemQuickSlots.Count; i++)
        {
            itemQuickSlots[i].SetSlot(PlayerInventory.Instance.slotDataList[40+i]);
        }
    }


    public void SelectSlot(SlotData slotData)
    {
        PlayerInventory.Instance.ConsumeItem(slotData.slotItem.itemData.itemID);
    }


   
}
