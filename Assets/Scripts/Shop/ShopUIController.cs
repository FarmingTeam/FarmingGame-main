
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : UIBase
{
    [SerializeField] private Image shopBackgroundImage;
    [SerializeField] private ScrollRect shopScrollRect;
    public GameObject slotPrefab;
    public Transform contentParent;
    public List<ShopItemData> fullShopItemDataList = new List<ShopItemData>();
    private PlayerInventory playerInventory;
    public string activeNpcID;
    public ShopConfirmUI confirmui;
    public Image buyBtnBG;
    public Image sellBtnBG;
    public Sprite selectedSprite;
    public Sprite normalSprite;
    public TMP_Text playerMoneyText;
    private Color buyTabBackgroundColor = new Color(1f, 1f, 1f, 0.8431f); 
    private Color sellTabBackgroundColor = new Color(0.325f, 1.0f, 0.0f, 0.8431f);

    private void Start()
    {
        playerInventory = PlayerInventory.Instance;
    }

    public void OpenShopUIWithBuyTab(string npcID)
    {
        activeNpcID = npcID;
        OpenUI();
        fullShopItemDataList = ShopCSVParser.Instance.shopItems;
        playerInventory = PlayerInventory.Instance;

        ShowBuyItems();
        UpdateShopTabHighlight(ShopMode.Buy);
        UpdateMoneyDisplay();
        if (shopBackgroundImage != null)
            shopBackgroundImage.color = buyTabBackgroundColor;
        if (shopScrollRect != null)
            StartCoroutine(ResetScrollNextFrame());
    }

    public void OpenShopUIWithSellTab(string npcID)
    {
        activeNpcID = npcID;
        OpenUI();
        fullShopItemDataList = ShopCSVParser.Instance.shopItems;
        ShowSellItems();
        UpdateShopTabHighlight(ShopMode.Sell);
        UpdateMoneyDisplay();
        if (buyBtnBG != null)
            buyBtnBG.gameObject.SetActive(false);
        if (shopBackgroundImage != null)
            shopBackgroundImage.color = sellTabBackgroundColor;
        if (shopScrollRect != null)
            StartCoroutine(ResetScrollNextFrame());
    }

    public void CloseShopUI()
    {
        var uiSwitch = FindObjectOfType<UISwitch>();
        if (uiSwitch != null && uiSwitch.inventoryAction != null)
            uiSwitch.inventoryAction.action.Enable();

        ShopCSVParser.Instance.shopItems = fullShopItemDataList;
        Destroy(transform.parent.gameObject);
        
        NPCManager.Instance.IsInteracting = false;
        Time.timeScale = 1f;
        TimeManager.Instance.PauseTime(false);
    }

    // 구매 가능한 아이템만 UI에 보여주기
    public void ShowBuyItems()
    {
        Debug.Log($"[DEBUG] activeNpcID: {activeNpcID}");
        //List<ShopItemData> buyableItems = fullShopItemDataList.FindAll(item => item.buyPrice > 0);
        List<ShopItemData> buyableItems = fullShopItemDataList.FindAll(
        item => item.buyPrice > 0 && item.npcID == activeNpcID);
        foreach (var item in buyableItems)
        {
            Debug.Log($"[DEBUG] itemName: {item.itemName}, item.npcID: {item.npcID} / activeNpcID: {activeNpcID}");

        }
        CreateShopUI(buyableItems, ShopMode.Buy, null);
        UpdateShopTabHighlight(ShopMode.Buy);
    }

    // 플레이어 인벤토리와 상점 데이터 비교해서 판매 가능한 아이템만 UI에 보여주기
    public void ShowSellItems()
    {
        List<ItemData> playerItems = GetPlayerOwnedItemData();
        List<ShopItemData> sellItems = new List<ShopItemData>();
        List<int> playerItemCounts = new List<int>();

        foreach (var pItem in playerItems)
        {
            ShopItemData shopData = fullShopItemDataList.Find(x => int.Parse(x.itemID) == pItem.itemID);
            if (shopData != null && shopData.sellPrice > 0)
            {
                sellItems.Add(shopData);

                int count = 0;
                foreach (var slot in playerInventory.slotDataList)
                {
                    if (slot.slotItem == null) continue; //null 체크
                    if (slot.slotItem.itemData == null) continue; //itemData null체크
                    if (slot.slotItem.itemData.itemID == pItem.itemID)
                        count += slot.slotItem.currentQuantity;
                }
                playerItemCounts.Add(count);
            }
        }
        CreateShopUI(sellItems, ShopMode.Sell, playerItemCounts);
        UpdateShopTabHighlight(ShopMode.Sell);
    }

    // 인벤토리에서 플레이어 소유 아이템 데이터만 추출 (중복 제외)
    private List<ItemData> GetPlayerOwnedItemData()
    {
        playerInventory = PlayerInventory.Instance;
        List<ItemData> ownedItems = new List<ItemData>();

        if (playerInventory == null)
        {
            Debug.LogError("playerInventory가 null입니다.");
            return ownedItems;
        }
        if (playerInventory.slotDataList == null)
        {
            Debug.LogError("playerInventory.slotDataList가 null입니다.");
            return ownedItems;
        }

        foreach (var slot in playerInventory.slotDataList)
        {
            if (slot.slotItem != null && slot.slotItem.itemData != null)
            {
                if (!ownedItems.Contains(slot.slotItem.itemData))
                    ownedItems.Add(slot.slotItem.itemData);
            }
        }
        return ownedItems;
    }

    // 슬롯 UI 생성 및 초기화
    public void CreateShopUI(List<ShopItemData> shopItems, ShopMode mode, List<int> itemCounts = null)
    {
        List<Transform> existingSlots = new List<Transform>();
        foreach (Transform child in contentParent)
            existingSlots.Add(child);

        foreach (var slot in existingSlots)
        {
            var slotUI = slot.GetComponent<SlotUI>();
            if (slotUI != null)
                slotUI.Clear();
        }

        for (int i = 0; i < shopItems.Count; i++)
        {
            Transform slotTransform;
            if (i < existingSlots.Count)
            {
                slotTransform = existingSlots[i];
                slotTransform.gameObject.SetActive(true);
            }
            else
            {
                GameObject newSlot = Instantiate(slotPrefab, contentParent);
                slotTransform = newSlot.transform;
            }

            var slotUI = slotTransform.GetComponent<SlotUI>();
            if (slotUI != null)
            {
                if (mode == ShopMode.Buy)
                {
                    slotUI.Init(
                        itemName: shopItems[i].itemName,
                        itemID: shopItems[i].itemID,
                        price: shopItems[i].buyPrice,
                        mode: ShopMode.Buy,
                        controller: this,
                        itemImageName: shopItems[i].itemImage
                    );
                }
                else
                {
                    int inventoryCount = (itemCounts != null && itemCounts.Count > i) ? itemCounts[i] : 1;
                    slotUI.Init(
                        itemName: shopItems[i].itemName,
                        itemID: shopItems[i].itemID,
                        price: shopItems[i].sellPrice,
                        mode: ShopMode.Sell,
                        availableCount: inventoryCount,
                        controller: this
                    );
                }
            }
        }

        for (int i = shopItems.Count; i < existingSlots.Count; i++)
        {
            existingSlots[i].gameObject.SetActive(false);
        }

        if (shopScrollRect != null)
            StartCoroutine(ResetScrollNextFrame());
    }

    // 구매, 판매 버튼 인스펙터 OnClick 연결
    public void OnClickBuyButton()
    {
      
        ShowBuyItems();
        UpdateShopTabHighlight(ShopMode.Buy);
        UpdateMoneyDisplay();
        if (shopBackgroundImage != null)
        {
            shopBackgroundImage.color = buyTabBackgroundColor;
        }
    }

    public void OnClickSellButton()
    {
    
        ShowSellItems();
        UpdateShopTabHighlight(ShopMode.Sell);
        UpdateMoneyDisplay();
        if (shopBackgroundImage != null)
        {
            shopBackgroundImage.color = sellTabBackgroundColor;
        }

    }

    public void UpdateShopTabHighlight(ShopMode mode)
    {
        if (buyBtnBG != null)
            buyBtnBG.sprite = (mode == ShopMode.Buy) ? selectedSprite : normalSprite;
        if (sellBtnBG != null)
            sellBtnBG.sprite = (mode == ShopMode.Sell) ? selectedSprite : normalSprite;
    }

    public void UpdateMoneyDisplay()
    {
        if (playerMoneyText != null)
            playerMoneyText.text = PlayerManager.Instance.playerMoney.Amount.ToString();
    }

    private IEnumerator ResetScrollNextFrame()
    {
        yield return null; // 한 프레임 대기
        shopScrollRect.verticalNormalizedPosition = 1.0f;
    }
}
