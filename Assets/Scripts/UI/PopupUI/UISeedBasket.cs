
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISeedBasket : UIPopup
{

    PlayerInventory inventory;

    List<UISeedSlot> slots=new List<UISeedSlot>();
    Dictionary<int, int> seedInventoryDic=new Dictionary<int, int>();


    [SerializeField] GameObject seedSlotPrefab;
    [SerializeField] Button closeButton;
    int SeedInventorySlotNum { get; } = 14;
 

    //여기 SelectedSlot.SlotSeedItem 으로 접근하면 현재 선택된 씨앗을 알 수 있음( 단 SelectedSlot이 null일수도 있음을 주의
    public UISeedSlot SelectedSlot { get; private set; }

    protected override void OnOpen()
    {
        base.OnOpen();

        //아래 슬롯들 소환
        if(slots.Count == 0)
        {
            for(int i = 0;i<SeedInventorySlotNum;i++)
            {
                Instantiate(seedSlotPrefab,transform,false);
            }
            slots = GetComponentsInChildren<UISeedSlot>(true).ToList();
        }

        inventory = PlayerInventory.Instance;

        //인벤토리에서 씨앗인거만 가져옴
        
        inventory.SubscribeOnItemChange(SetSeedSlotUI);
        SetSeedSlotUI();
        closeButton.onClick.AddListener(CloseBasket);
        
    }
    //플레이어 인벤토리쪽을 이어두고

    protected override void OnClose()
    {
        inventory.UnsubscribeOnItemChange(SetSeedSlotUI);
        closeButton.onClick.RemoveListener(CloseBasket);
    }


    public void SetSeedSlotUI()
    {
        seedInventoryDic.Clear();
        if (slots.Count == 0 || slots[0] == null)
        {
            slots.Clear();
            for (int i = 0; i < SeedInventorySlotNum; i++)
            {
                Instantiate(seedSlotPrefab, transform, false);
            }
            slots = GetComponentsInChildren<UISeedSlot>(true).ToList();
        }
        foreach (var slot in inventory.slotDataList)
        {

            if(slot.slotItem==null)
            {
                continue;
            }
            if (slot.slotItem.itemData is SeedData)
            {
                

                //만약 씨앗이면 딕셔너리에 그 아이템 아이디가 있으면 quantity value만 더해서 다시넣어주기
                if (seedInventoryDic.TryGetValue(slot.slotItem.itemData.itemID, out int quantity))
                {
                    seedInventoryDic[slot.slotItem.itemData.itemID] = quantity + slot.slotItem.currentQuantity;
                }
                else
                {
                    //만약 딕셔너리에 없었으면 새로 등록
                    seedInventoryDic.Add(slot.slotItem.itemData.itemID, slot.slotItem.currentQuantity);
                }



            }
        }

        var list=seedInventoryDic.OrderBy(kv => kv.Key).ToList();
        
        for(int i=0;i<list.Count;i++)
        {
            slots[i].SetSeedSlot((SeedData) ResourceManager.Instance.GetItem(list[i].Key), list[i].Value);
        }
        for(int i=list.Count; i<slots.Count;i++)
        {
            slots[i].EmptyOutSlot();
        }

        OutLineSelectedSlot();
        

    }

    UISeedSlot FindEmptySeedSlot()
    {
        foreach (var slot in slots)
        {
            if(slot.SlotSeedItem == null)
            {
                return slot;
            }
        }
        Debug.Log("칸이 없습니다");
        return null;
    }
    




    public void SelectSlot(UISeedSlot uISeedSlot)
    {
        SelectedSlot = uISeedSlot;

        foreach (var slot in slots)
        {
            slot.outline.enabled = false;
        }
        if (SelectedSlot != null)
        {
            SelectedSlot.outline.enabled = true;
        }
        PlayerEquipment.Instance.ChangeEquipmentExtra(EquipmentType.SeedBasket,SelectedSlot.SlotSeedItem.itemID);
        


        Debug.Log($"선택 씨앗: {SelectedSlot.SlotSeedItem.itemName}");
    }

    void OutLineSelectedSlot()
    {
       
        if(SelectedSlot != null&&SelectedSlot.SlotSeedItem!=null)
        {
            foreach (var slot in slots)
            {
                slot.outline.enabled = false;
                if (slot.SlotSeedItem!=null)
                {
                    
                    if (slot.SlotSeedItem.itemID == PlayerEquipment.Instance.CheckEquipmentExtra(EquipmentType.SeedBasket))
                    {
                        slot.outline.enabled = true;
                        break;

                    }
                }
                
            }
        }

        
    }



    void CloseBasket()
    {
        UIManager.Instance.CloseUI<UISeedBasket>();
    }

    private void OnDestroy()
    {
        OnClose();
    }

}

