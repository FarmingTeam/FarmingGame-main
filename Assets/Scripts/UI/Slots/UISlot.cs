
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{

    UIInventory uiInventory;
    UISwitch uiSwitch;

    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI quantityText;

    GameObject slotDescriptionPanel;

    //퀵슬롯인지 여부
    [SerializeField] bool isQuickSlot;

   

    public SlotData SlotData { get; private set; } = null;

    public int slotIndex;

    GameObject dragObject;

    private void OnEnable()
    {

       uiInventory = GetComponentInParent<UIInventory>();
        
       uiSwitch = GetComponentInParent<UISwitch>();
    }

    
    public void SetSlot(SlotData slotData)
        {
        
        this.SlotData = slotData;
        if(SlotData.slotItem==null ||SlotData.slotItem.itemData==null)
        {
            image.sprite = null;
            var col = image.color;
            col.a = 0;
            image.color = col;
            quantityText.SetText("");
            return;
        }
        var c = image.color;
        c.a = 255;
        image.color = c;
        image.sprite=slotData.slotItem.itemData.itemIcon;
        quantityText.SetText(slotData.slotItem.currentQuantity.ToString());
        
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isQuickSlot)
        {
            if (SlotData != null && SlotData.slotItem != null && SlotData.slotItem.itemData != null)
            {

                slotDescriptionPanel = uiInventory.DescriptionPanel;
                slotDescriptionPanel.SetActive(true);
                slotDescriptionPanel.transform.SetParent(uiInventory.transform);

                slotDescriptionPanel.transform.position=  new Vector3(CheckSlotPosition().x+150f, CheckSlotPosition().y - 200f, 1);
                UIDescriptionPanel uIDescriptionPanel=slotDescriptionPanel.GetComponent<UIDescriptionPanel>();
                uIDescriptionPanel.nameText.SetText(SlotData.slotItem.itemData.itemName);
                uIDescriptionPanel.descriptionText.SetText(SlotData.slotItem.itemData.itemDescription);
                
            }

        }


    }

    Vector3 CheckSlotPosition()
    {
        return this.transform.position;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if(slotDescriptionPanel!=null)
        {
            slotDescriptionPanel.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (SlotData == null||SlotData.slotItem == null || SlotData.slotItem.itemData == null)
        {
            return;
        }
        

        dragObject = new GameObject("DragObject",typeof(Image));
        dragObject.AddComponent<LayoutElement>().ignoreLayout = true;
        Image dragImage = dragObject.GetComponent<Image>();
        dragImage.sprite=SlotData.slotItem.itemData.itemIcon;
        dragImage.raycastTarget=false;
        //일단 진짜 드래그처럼 보이기위한 꼼수로 슬롯 비어있는척
        image.sprite = null;
        var col = image.color;
        col.a = 0;
        image.color = col;
        quantityText.SetText("");

        //아이템 이미지의 복사본을 만들기 그리고 마우스 포인터 따로오게하기
        //레이캐스트 false로
        //dragObject.transform.SetParent(uiInventory.transform,false);
        dragObject.transform.SetParent(uiSwitch.transform, false);
        dragObject.transform.SetAsLastSibling();
        dragObject.transform.localPosition = Vector3.zero;
        dragObject.transform.position=eventData.position;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SlotData == null||SlotData.slotItem == null || SlotData.slotItem.itemData == null)
        {
            return;
        }
        dragObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (SlotData == null || SlotData.slotItem == null || SlotData.slotItem.itemData == null  )
        {
            return;
        }


        if(eventData.pointerCurrentRaycast.gameObject==null)
        {
            //그냥 결과가 null이면 볼것도 없이 다시 원상복구 세팅  (근데 만약 아예 바닥에 떨구기면 여기 로직 넣으면됨;
            //uiInventory.StartSettingItem();
            PlayerInventory.Instance.onItemChange?.Invoke();
            Destroy(dragObject);
            return;
        }



        UISlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<UISlot>();
        if (slot!=null)
        {
            slot.ChangeItem(slotIndex);
            Destroy(dragObject);
            return;
        }
        else
        {
            //만약 쓰레기통에 떨구면(이건 그냥 쓰레기통측이서 처리해도...
            if(eventData.pointerCurrentRaycast.gameObject.GetComponent<UITrashCan>())
            {
                Debug.Log("쓰레기통 버리기");
                PlayerInventory.Instance.EmptyOutSlot(SlotData);
            }
            Destroy(dragObject);
        }

        //uiInventory.StartSettingItem();
        
        PlayerInventory.Instance.onItemChange?.Invoke();
        //슬롯인지 확인하기
        //레이캐스트 다시 true로
        //만약 슬롯이면 
    }



    //이 슬롯위로 떨궈질때 호출될 예정
    public void ChangeItem(int slotIdx)
    {
        PlayerInventory.Instance.SwitchItemPlaces(slotIndex, slotIdx);
    }
   
}
