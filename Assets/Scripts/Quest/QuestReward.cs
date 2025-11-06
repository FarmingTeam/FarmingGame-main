using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RewardUtil
{
    public static void GiveQuestReward(string rewardRaw, string npcId)
    {
        if (string.IsNullOrEmpty(rewardRaw))
            return;

        var rewards = rewardRaw.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var reward in rewards)
        {
            string r = reward.Trim();
            if (string.IsNullOrEmpty(r)) continue;

            if (r.EndsWith("G")) // 골드
            {
                if (int.TryParse(r.Replace("G", ""), out int amount))
                    PlayerManager.Instance.playerMoney.Add(amount);
            }
            else if (r.Contains("Popul"))
            {
                string favorStr = r.Replace("Popul", "").Replace("+", "").Trim();
                if (favorStr == "") continue;
                if (int.TryParse(favorStr, out int favor))
                    NPCAffinity.AddFavor(npcId, favor);
                else
                    Debug.LogWarning("[RewardUtil] 호감도 파싱 실패: " + favorStr);
            }
            else if (r.StartsWith("+") && int.TryParse(r.Replace("+", "").Trim(), out int favor))
            {
                NPCAffinity.AddFavor(npcId, favor);
            }
            else if (int.TryParse(r, out int id))
            {
                if (id >= 10000) // 도구/장비라면
                {
                    Equipment equip = ResourceManager.Instance.GetEquipment(id);
                    if (equip != null)
                    {
                        PlayerEquipment.Instance.AddEquipmentByID(id);

                        // 강제 UI 갱신(확실하게 활성화된 UI에 대해서만)
                        var toolbar = GameObject.FindObjectOfType<UIItemToolBar>();
                        if (toolbar != null && toolbar.gameObject.activeInHierarchy)
                        {
                            toolbar.StartCoroutine(RefreshNextFrame(toolbar));
                        }
                        Debug.Log($"도구 지급: {id}");
                    }
                    else
                    {
                        Debug.LogWarning($"도구 정보 없음: {id}");
                    }
                }
                else // 일반 아이템
                {
                    ItemData item = ResourceManager.Instance.GetItem(id);
                    if (item != null)
                    {
                        PlayerInventory.Instance.AdditemsByID(id, 1);
                    }
                    else
                    {
                        Debug.LogWarning($"아이템 정보 없음: {id}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[RewardUtil] 알 수 없는 보상 구문: " + r);
            }
        }
    }
    private static IEnumerator RefreshNextFrame(UIItemToolBar toolbar)
    {
        yield return null; // 한 프레임 대기 후
        toolbar.SetSlots();
        Debug.Log("UIItemToolBar: 1프레임 대기 후 강제 갱신됨");
    }

    //==========================아이템 차감기능=============================

    public static bool DeductQuestItems(string target, int count)
    {
        if (string.IsNullOrEmpty(target) || count <= 0)
        {
            Debug.Log("차감할 아이템 정보가 없습니다.");
            return true;
        }
        if (target.StartsWith("ItemID:", StringComparison.OrdinalIgnoreCase))
        {
            string itemPart = target.Substring(7);

            // 범위 체크
            if (itemPart.Contains("~"))
            {
                return DeductItemRangeTotalCount(itemPart, count);
            }
            // 단일 아이템
            else
            {
                if (int.TryParse(itemPart, out int itemId))
                {
                    return DeductSingleItem(itemId, count);
                }
                else
                {
                    Debug.LogError($"잘못된 아이템 ID 형식: {itemPart}");
                    return false;
                }
            }
        }

        Debug.LogWarning($"알 수 없는 Target 형식: {target}");
        return false;
    }
    private static bool DeductSingleItem(int itemId, int count)
    {
        bool deducted = PlayerInventory.Instance.SubtractItemQuantity(itemId, count);
        if (deducted)
        {
            Debug.Log($"아이템 {itemId}번 {count}개 차감 완료");
        }
        else
        {
            Debug.LogWarning($"아이템 {itemId}번 {count}개 차감 실패 (아이템 부족)");
        }
        return deducted;
    }

    // 범위 아이템 총합에서 count만큼 제거(1101~1106 중 무엇이든 합해서 count개만!)
    private static bool DeductItemRangeTotalCount(string rangePart, int totalCount)
    {
        string[] rangeParts = rangePart.Split('~');
        if (rangeParts.Length != 2 ||
            !int.TryParse(rangeParts[0], out int startId) ||
            !int.TryParse(rangeParts[1], out int endId) ||
            startId > endId)
        {
            Debug.LogError($"잘못된 범위 형식: {rangePart}");
            return false;
        }

        int remainingCount = totalCount;
        List<(int itemId, int currentQty)> availableItems = new List<(int, int)>();

        // 현재 인벤토리에서 각 아이템 보유량 조사
        for (int itemId = startId; itemId <= endId; itemId++)
        {
            int qty = PlayerInventory.Instance.CheckItemQuantity(itemId);
            if (qty > 0)
                availableItems.Add((itemId, qty));
        }
        int totalOwned = availableItems.Sum(x => x.currentQty);
        if (totalOwned < totalCount)
        {
            Debug.LogWarning($"총 보유량 부족: {totalOwned}/{totalCount}");
            return false;
        }

        // 실제 차감 처리
        foreach (var tuple in availableItems)
        {
            int toSubtract = Mathf.Min(tuple.currentQty, remainingCount);
            if (toSubtract > 0)
            {
                // 슬롯마다 여러개 있을 수 있으니 반복적으로 호출
                PlayerInventory.Instance.SubtractItemQuantity(tuple.itemId, toSubtract);
                remainingCount -= toSubtract;
            }
            if (remainingCount == 0) break;
        }

        Debug.Log($"{rangePart} 범위에서 {totalCount}개 차감 완료");
        return true;
    }
}