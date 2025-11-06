using UnityEngine;

public class SampleItemScript : MonoBehaviour
{
   


    //================== 아이템 관련 =======================================
    void 아이템데이터가져오기(int 아이템ID) //씨앗도 당연히 가능
    {
        ItemData itemdata=  ResourceManager.Instance.GetItem(아이템ID);
    }

    void 인벤토리아이템추가(int 아이템ID, int 갯수) //씨앗도 동일하게 이거사용
    {
        PlayerInventory.Instance.AdditemsByID(아이템ID,갯수);
    }

    void 인벤토리아이템갯수차감(int 아이템ID, int 갯수) //씨앗도 동일하게 이거 사용
    {
        PlayerInventory.Instance.SubtractItemQuantity(아이템ID, 갯수);
    }

    void 도구관련기능()
    {
        //현재 플레이어가 장착한 장비를 알고싶어
        Equipment playerEquipment= MapControl.Instance.player.tool.CurrentEquip;

        //====씨앗바구니=====
        //현재 씨앗바구니에 들어있는 씨앗을 알고싶어(씨앗바구니에서 씨앗을 고르는 순간 설정이 되며, 아무것도 안골랐다면 iD가 기본값인 0으로 나올듯)
        int seedID = PlayerEquipment.Instance.CheckEquipmentExtra(EquipmentType.SeedBasket);

        //=====물뿌리개======
        //물뿌리개에 대해 남은 물양을 알고싶어
        int waterleft= PlayerEquipment.Instance.CheckEquipmentExtra(EquipmentType.WateringCan);

        //물뿌리개의 물을 쓰고싶어 (0이면 물을 못주는 로직은 상호작용쪽에서 추가 필요)
        PlayerEquipment.Instance.ChangeEquipmentExtra(EquipmentType.WateringCan, -1);
        //물뿌리개의 물을 끝까지 채우고싶어
        PlayerEquipment.Instance.ChangeEquipmentExtra(EquipmentType.WateringCan, 10);



    }




    //================== UI 관련 =======================================

    
    void UI열기() //예를들어 툴바
    {
        UIManager.Instance.OpenUI<UIToolBar>();
    }

    void UI닫기() 
    {
        UIManager.Instance.CloseUI<UIToolBar>();
    }

    void 토글기능()
    {
        UIManager.Instance.ToggleUI<UIToolBar>();
    }

    
   
}
