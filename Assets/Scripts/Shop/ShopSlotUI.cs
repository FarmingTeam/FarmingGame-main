using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum ShopMode
{
    Buy,
    Sell
}

public class SlotUI : MonoBehaviour
{
    public TMP_Text itemNameText, goldText, countText, goldNotEnoughText;
    public Button plusButton, subButton, buyButton;
    private int count = 0;
    private string itemID;
    private ShopMode currentMode;
    private int maxCount = int.MaxValue;
    private ShopUIController shopUIController;
    public Image itemImg;

    public void Init(string itemName, string itemID, int price, ShopMode mode, int availableCount = int.MaxValue, ShopUIController controller = null, string itemImageName = "")
    {
        this.itemID = itemID;
        currentMode = mode;
        maxCount = availableCount;
        if (currentMode == ShopMode.Sell)
        {
            itemNameText.text = itemName + " x" + availableCount;
            itemNameText.fontSize = 26;
        }
        else
        {
            itemNameText.text = itemName;
            itemNameText.fontSize = 30;
        }
        goldText.text = price.ToString();
        count = 0;
        UpdateCountUI();

        plusButton.onClick.RemoveAllListeners();
        subButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(OnPlus);
        subButton.onClick.AddListener(OnMinus);

        this.shopUIController = controller;

        
        int id = int.Parse(itemID);
        ItemData item = ResourceManager.Instance.GetItem(id);

        Sprite iconSprite = null;
        if (item != null)
        {
            // 1차 시도
            iconSprite = Resources.Load<Sprite>($"ItemData/ItemNewImage/{item.itemPath}");
            // 2차 시도 (없을 경우)
            if (iconSprite == null)
                iconSprite = Resources.Load<Sprite>($"ItemData/ItemImage/{item.itemPath}");
        }

        if (itemImg != null && iconSprite != null)
        {
            itemImg.sprite = iconSprite;
        }
        else
        {
            Debug.LogError($"[SlotUI] 아이템 아이콘이 없습니다: {itemID} / {item?.itemPath}");
        }


        if (currentMode == ShopMode.Buy)
        {
            goldText.color = Color.black;
            buyButton.GetComponentInChildren<TMP_Text>().text = "구매";
            buyButton.onClick.AddListener(() =>
            {
                if (count <= 0) return;

                if (shopUIController != null && shopUIController.confirmui != null)
                {
                    shopUIController.confirmui.Show(
                        $"{itemNameText.text}을 {count}개 구매하시겠습니까?",
                        () => BuyItem(itemID, price, count),
                        null // 아니요 시 아무 동작 없음
                    );
                }
                else
                {
                    BuyItem(itemID, price, count);
                }
            });
        }
        else
        {
            goldText.color = Color.red;
            buyButton.GetComponentInChildren<TMP_Text>().text = "판매";
            buyButton.onClick.AddListener(() =>
            {
                if (count <= 0) return;

                if (shopUIController != null && shopUIController.confirmui != null)
                {
                    shopUIController.confirmui.Show(
                        $"{itemNameText.text}을 {count}개 판매하시겠습니까?",
                        () => SellItem(itemID, price, count),
                        null
                    );
                }
                else
                {
                    SellItem(itemID, price, count);
                }
            });
        }
    }

    public void Clear()
    {
        itemNameText.text = "";
        goldText.text = "";
        countText.text = "";
        goldNotEnoughText.gameObject.SetActive(false);
    }

    void OnPlus() { if (count < maxCount) count++; UpdateCountUI(); }
    void OnMinus() { if (count >= 1) count--; UpdateCountUI(); }
    void UpdateCountUI()
    {
        if (count < 1)
            count = 1;

        countText.text = count.ToString();
        countText.gameObject.SetActive(true);
    }

    public void BuyItem(string itemID, int price, int count)
    {
        if (count <= 0) return;
        int totalPrice = price * count;
        if (PlayerManager.Instance.playerMoney.TrySpend(totalPrice))
        {
            PlayerInventory.Instance.AdditemsByID(int.Parse(itemID), count);
            if (shopUIController != null)
                shopUIController.UpdateMoneyDisplay();
        }
        else
            ShowGoldNotEnoughMessage();
    }

    public void SellItem(string itemID, int price, int count)
    {
        if (count <= 0) return;

        int playerCount = GetItemQuantityFromInventory(int.Parse(itemID));
        if (playerCount < count) return;

        PlayerInventory.Instance.SubtractItemQuantity(int.Parse(itemID), count);
        PlayerManager.Instance.playerMoney.Add(price * count);

        if (shopUIController != null)
            shopUIController.UpdateMoneyDisplay();

        if (shopUIController != null)
            shopUIController.ShowSellItems();
    }

    private IEnumerator FadeOutText(TMP_Text textComponent, float duration)
    {
        Color originalColor = textComponent.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        textComponent.gameObject.SetActive(false);

    }

    public void ShowGoldNotEnoughMessage()
    {
        goldNotEnoughText.gameObject.SetActive(true);
        goldNotEnoughText.color = new Color(goldNotEnoughText.color.r, goldNotEnoughText.color.g, goldNotEnoughText.color.b, 1f);

        StartCoroutine(FadeOutText(goldNotEnoughText, 1f));
    }

    private int GetItemQuantityFromInventory(int itemID)
    {
        int totalQuantity = 0;
        foreach (var slotData in PlayerInventory.Instance.slotDataList)
        {
            if (slotData.slotItem != null && slotData.slotItem.itemData != null &&
                slotData.slotItem.itemData.itemID == itemID)
            {
                totalQuantity += slotData.slotItem.currentQuantity;
            }
        }
        return totalQuantity;
    }
}
