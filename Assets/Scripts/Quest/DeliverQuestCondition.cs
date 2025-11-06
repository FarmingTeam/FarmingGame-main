using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliverQuestCheckParam
{
    public string NpcID;                   // 말 건 NPC
    public PlayerInventory Inventory;      // 현재 인벤토리
}

[Serializable]
public class DeliverQuestCondition : QuestConditionBase
{
    public List<int> ItemIDs { get; private set; }
    public int RequiredCount { get; private set; }
    public string TargetNPCID { get; private set; }

    public ItemType? TypeFilter { get; private set; } // 타입필터

    private bool _turnedIn; // 실제 NPC에게 전달 되었는지 판별

    public DeliverQuestCondition(List<int> itemIDs, int requiredCount, string npcID)
    {
        Action = QuestAction.Deliver;
        ItemIDs = itemIDs ?? new List<int>();
        RequiredCount = Mathf.Max(1, requiredCount);
        TargetNPCID = npcID ?? "";
        CurrentCount = 0;

        Count = RequiredCount;
        Target = TargetNPCID;
    }

    public DeliverQuestCondition(ItemType typeFilter, int requiredCount, string npcID)
    {
        Action = QuestAction.Deliver;
        ItemIDs = new List<int>();
        TypeFilter = typeFilter;
        RequiredCount = Mathf.Max(1, requiredCount);
        TargetNPCID = npcID ?? "";
        CurrentCount = 0;

        Count = RequiredCount;
        Target = TargetNPCID;
    }


    public override bool IsCompleted() => _turnedIn;

    public override void OnProgress(object parameter)
    {
        if (parameter is not DeliverQuestCheckParam param)
        {
            Debug.Log("[DeliverProgress] 피라미터 타입이 올바르지 않음");
            return;
        }

        int totalCount = TypeFilter.HasValue
            ? GetQuantityByType(param.Inventory, TypeFilter.Value)
            : GetQuantityByItemIDs(param.Inventory, ItemIDs);

        CurrentCount = Mathf.Clamp(totalCount, 0, RequiredCount);

        if (!_turnedIn && param.NpcID == TargetNPCID && totalCount >= RequiredCount)
        {
            _turnedIn = true;
            CurrentCount = RequiredCount;
            Debug.Log("[DeliverProgress] 딜리버 조건 달성(제출완료)");
        }
        else 
        {
            Debug.Log($"[DeliverProgress] 진행 : {CurrentCount}/{RequiredCount}, 제출 완료 여부 : {_turnedIn}");
        }
    }

    /*public override bool IsCompleted() => CurrentCount >= RequiredCount;

    public override void OnProgress(object parameter)
    {
         if (parameter is DeliverQuestCheckParam param)
        {
            Debug.Log($"[DeliverProgress] 입력된 NpcID: {param.NpcID}, 목표 TargetNPCID: {TargetNPCID}");
            Debug.Log($"[DeliverProgress] 인벤토리 슬롯 개수: {param.Inventory?.slotDataList?.Count ?? -1}");
            int totalCount = GetQuantityByItemIDs(param.Inventory, ItemIDs);
            Debug.Log($"[DeliverProgress] 총합 count: {totalCount}, 필요 수량: {RequiredCount}");
            if (param.NpcID == TargetNPCID && totalCount >= RequiredCount)
            {
                CurrentCount = RequiredCount;
                Debug.Log("[DeliverProgress] 딜리버 조건 달성!");
            }
            else
            {
                Debug.Log("[DeliverProgress] 딜리버 조건 미달성!");
            }
        }
        else
        {
            Debug.Log("[DeliverProgress] 파라미터 타입이 올바르지 않음!");
        }*/

    // 인벤토리에서 아이템ID 리스트의 합계 수량을 체크
    private int GetQuantityByItemIDs(PlayerInventory inv, List<int> itemIDs)
    {
        int sum = 0;
        foreach (int itemId in itemIDs)
        {
            foreach (var slot in inv.slotDataList)
            {
                if (slot.slotItem != null && slot.slotItem.itemData != null && slot.slotItem.itemData.itemID == itemId)
                    sum += slot.slotItem.currentQuantity;
            }
        }
        return sum;
    }

    private int GetQuantityByType(PlayerInventory inv, ItemType type)
    {
        if (inv?.slotDataList == null)
        { 
            return 0;
        }

        int sum = 0;
        for (int i = 0; i < inv.slotDataList.Count; i++)
        { 
            var slot = inv.slotDataList[i];
            var data = slot?.slotItem?.itemData;
            if (data != null && data.itemType == type)
            {
                sum += slot.slotItem.currentQuantity;
            }
        }
        return sum;
    }

}




