
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIQuickSlot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerClickHandler
{
    
    public Equipment slotEquipment;
    public int slotNumber;
    [SerializeField] TextMeshProUGUI slotNumberText;
    [SerializeField] Image image;
    [SerializeField] Image medalGameImage;
    [SerializeField] Sprite bronzeMedal;
    [SerializeField] Sprite silverMedal;
    [SerializeField] Sprite goldMedal;


    UIToolBar toolbar;

    public Outline outline;
    public Image waterBar;

     //이건 기존 이미지
    GameObject dragObject;


    public void Init()
    {
        slotNumberText.SetText(slotNumber.ToString());
        
    }
    public void SetQuickSlot(Equipment equipment)
    {
        Color col;
        Color medalcol;
        //만약 빈손이면
        if(equipment.equipmentID==0)
        {
            col = image.color;
            col.a = 0;
            image.color = col;

            medalcol = image.color;
            medalcol.a = 0;
            medalGameImage.color = medalcol;

            slotEquipment=equipment;
            waterBar.gameObject.SetActive(false);
            
            return;
        }

        col = image.color;
        col.a = 255;
        image.color = col;

        medalcol = image.color;
        medalcol.a = 255;
        medalGameImage.color = medalcol;

        image.sprite=equipment.equipmentIcon;
        slotEquipment=equipment;
        if(slotEquipment.equipmentType==EquipmentType.WateringCan)
        {
            waterBar.gameObject.SetActive(true);
            waterBar.fillAmount = (float)equipment.equipmentExtra / equipment.equipmentMaxRate;
            if(waterBar.fillAmount <= 0.0f)
                StartCoroutine(TutSpawner.Instance.DisplayTutorial(6));
        }
        else
        {
            waterBar.gameObject.SetActive(false);
        }

        if(equipment.equipmentUpgrade==0)
        {
            medalGameImage.sprite=bronzeMedal;
        }
        else if(equipment.equipmentUpgrade==1)
        {
            medalGameImage.sprite=silverMedal;
        }
        else if( equipment.equipmentUpgrade==2)
        {
            medalGameImage.sprite=goldMedal;
        }
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerController controller = MapControl.Instance.player.GetComponent<PlayerController>();
        if (!controller.CanInput() || UIManager.Instance.currentUISwitch.isOpen)
            return;
        MapControl.Instance.player.tool.SelectQuickslot(slotNumber);
        Debug.Log("클릭");
    }

    private void OnEnable()
    {
        toolbar= GetComponentInParent<UIToolBar>();
    }




    public void OnBeginDrag(PointerEventData eventData)
    {

        if (slotEquipment == null|| slotEquipment?.equipmentType == EquipmentType.None)
        {
            return;
        }
        PlayerController controller = MapControl.Instance.player.GetComponent<PlayerController>();
        if (!controller.CanInput() || UIManager.Instance.currentUISwitch.isOpen)
            return;



        dragObject = new GameObject("DragObject", typeof(Image));
        Image dragImage = dragObject.GetComponent<Image>();
        dragImage.sprite = slotEquipment.equipmentIcon;
        dragImage.raycastTarget = false;
        //일단 진짜 드래그처럼 보이기위한 꼼수로 슬롯 비어있는척
        image.gameObject.SetActive(false);


        //아이템 이미지의 복사본을 만들기 그리고 마우스 포인터 따로오게하기
        //레이캐스트 false로
        dragObject.transform.SetParent(toolbar.transform, false);
        dragObject.transform.SetAsLastSibling();
        dragObject.transform.localPosition = Vector3.zero;
        dragObject.transform.position = eventData.position;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotEquipment == null|| slotEquipment?.equipmentType == EquipmentType.None)
        {
            return;
        }
        PlayerController controller = MapControl.Instance.player.GetComponent<PlayerController>();
        if (!controller.CanInput() || UIManager.Instance.currentUISwitch.isOpen)
            return;

        dragObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotEquipment == null || slotEquipment?.equipmentType == EquipmentType.None)
        {
            return;
        }
        PlayerController controller = MapControl.Instance.player.GetComponent<PlayerController>();
        if (!controller.CanInput() || UIManager.Instance.currentUISwitch.isOpen)
            return;

        image.gameObject.SetActive(true);

        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            //그냥 결과가 null이면 볼것도 없이 다시 원상복구 세팅  (근데 만약 아예 바닥에 떨구기면 여기 로직 넣으면됨;
            //장비 동가화
            Destroy(dragObject);
            return;
        }

        UIQuickSlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIQuickSlot>();
        Debug.Log(slotNumber);
        if (slot != null)
        {
            slot.ChangeItem(slotNumber-1);
            Destroy(dragObject);
            return;
        }
        else
        {
            
            Destroy(dragObject);
        }

        
        //동기화
        //슬롯인지 확인하기
        //레이캐스트 다시 true로
        //만약 슬롯이면 
    }



    //이 슬롯위로 떨궈질때 호출될 예정
    public void ChangeItem(int slotIdx)
    {
        Debug.Log("아이템바꾸기");

        PlayerEquipment.Instance.SwitchEquipmentPlaces(slotNumber-1,slotIdx);
    }

}









    
