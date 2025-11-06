
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISeedSlot : MonoBehaviour,IPointerClickHandler
{
    public SeedData SlotSeedItem { get; private set; }
    public Image image;
    TextMeshProUGUI quantityText;
    public Outline outline;
    UISeedBasket seedBasket;
   

    public void SetSeedSlot(SeedData seedData, int quantity)
    {
        if(image==null)
        {
            image=GetComponent<Image>();
        }
        if(quantityText==null)
        {
            quantityText=GetComponentInChildren<TextMeshProUGUI>();
        }

        SlotSeedItem = seedData;

        if(SlotSeedItem != null)
        {
            image.sprite = SlotSeedItem.itemIcon;
            quantityText.SetText(quantity.ToString());
            var col = image.color;
            col.a = 255;
            image.color = col;
        }
        else
        {
            
            quantityText.SetText(quantity.ToString());
            var col = image.color;
            col.a = 0;
            image.color = col;
            quantityText.SetText("");
        }
        

    }


    public void EmptyOutSlot()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (quantityText == null)
        {
            quantityText = GetComponentInChildren<TextMeshProUGUI>();
        }

        SlotSeedItem = null;
        
        var col = image.color;
        col.a = 0;
        image.color = col;
        quantityText.SetText("");
        
    }

    private void OnEnable()
    {
        
        outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(seedBasket==null)
        {
            seedBasket=GetComponentInParent<UISeedBasket>();
        }
        if(SlotSeedItem!=null)
        {
            seedBasket.SelectSlot(this);
        }
        else
        {
            //빈곳 클릭시 그냥 닫기
            UIManager.Instance.CloseUI<UISeedBasket>();
        }
        
    }
}
