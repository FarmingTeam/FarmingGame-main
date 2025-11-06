
using UnityEngine;
using System;

public class ToolPivot : MonoBehaviour
{
    [Header("Tool")]
    public GameObject ToolSprite;
    public Equipment CurrentEquip;

    public Equipment[] QuickSlots = new Equipment[7];

    private Action onSelectionChange;
    //만약 특정 번호를 누를경우 여기로 신호가 감
    //그냥 그럼 퀵슬롯 배열에 미리 도구를 할당해주면 되겠네
    int currentSlotIdx = -1;
    [SerializeField] private Player player;

    private void Start()
    {
        if (CurrentEquip == null)
        {
            CurrentEquip = ResourceManager.Instance.GetEquipment(0);
        }
    }

    public void SelectQuickslot(int slot)
    {
        int idx = slot - 1;
        //플레이어의 현재 장비 리스트를 받아와서 이 번호에 맞는걸 ApplyEquip한다
        
        QuickSlots=PlayerEquipment.Instance.equipList.ToArray();


        if(currentSlotIdx==idx)
        {
            //만약 같은거를 선택했다면
            Debug.Log("같은 아이템 선택");
            UnEquipItem();
            onSelectionChange?.Invoke();
            currentSlotIdx = -1;
            PlayerManager.Instance.ToolIdx = -1;
            return;
        }


        PlayerManager.Instance.ToolIdx = slot;
        currentSlotIdx = idx;
        ApplyEquip(QuickSlots[idx]);
        player.interactChecker.OnChange(player.tool.CurrentEquip);
    }

    private void ApplyEquip(Equipment eq)
    {
        CurrentEquip = eq;

        if (ToolSprite) ToolSprite.SetActive(CurrentEquip != null); // 빈손이면 ToolSprite 비활성화
        onSelectionChange?.Invoke();
        Debug.Log($"장착템: {CurrentEquip.equipmentName}");
    }


    private void UnEquipItem()
    {

        UIManager.Instance.CloseUI<UISeedBasket>();
        CurrentEquip = ResourceManager.Instance.GetEquipment(0);
    }

    public void SubscribeToSelectionChange(Action action)
    {
        onSelectionChange += action;
    }

    public void UnsubscribeToSelectionChange(Action action)
    {
        onSelectionChange -= action;    
    }

}
