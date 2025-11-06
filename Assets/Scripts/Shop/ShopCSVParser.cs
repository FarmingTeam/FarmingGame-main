
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    public int productID;
    public string itemID;
    public string itemImage;
    public string itemName;
    public bool buyLimit;
    public int limitCount;
    public int buyPrice;
    public int sellPrice;
    public string npcID;
}

public class ShopCSVParser : Singleton<ShopCSVParser>
{
    public List<ShopItemData> shopItems;

    protected override void Initialize()
    {
        TextAsset csvTextAsset = Resources.Load<TextAsset>("ShopData/ShopData");
        if (csvTextAsset != null)
        {
            shopItems = LoadShopData(csvTextAsset);
            Debug.Log($"불러온 아이템 개수: {shopItems.Count}");

            ShopUIController shopUIController = FindObjectOfType<ShopUIController>();
            if (shopUIController != null)
            {
                shopUIController.fullShopItemDataList = shopItems;  // 할당 추가
                shopUIController.ShowBuyItems(); // 구매 가능한 아이템만 보여주기
            }

            foreach (var item in shopItems)
            {
                Debug.Log($"아이템: {item.itemName}, 가격: {item.buyPrice}");
            }
        }
        else
        {
            Debug.LogError("CSV 파일을 Resources 폴더에서 찾을 수 없습니다!");
        }
    }

    public List<ShopItemData> LoadShopData(TextAsset csvTextAsset)
    {
        List<ShopItemData> itemList = new List<ShopItemData>();
        string[] lines = csvTextAsset.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            Debug.Log($"Line[{i}] : {line}");
            // 1. 완전히 빈 줄 or 콤마만 있는 줄 스킵
            if (string.IsNullOrEmpty(line) || line.Replace(",", "").Trim().Length == 0)
            {
                Debug.Log("Skipped: Empty or comma line");
                continue;
            }
            // 2. 헤더/제목줄 스킵
            if (line.Contains("상품ID"))
            {
                Debug.Log("Skipped: Header line");
                continue;
            }
            string[] cols = line.Split(',');

            // 혹시 컬럼 개수가 부족하면 건너뛰기 (에러 방지)
            if (cols.Length < 10)
            {
                Debug.Log($"Skipped: Not enough columns - {cols.Length} cols");
                continue;
            }
            ShopItemData item = new ShopItemData();
            item.productID = int.Parse(cols[1]);
            item.itemID = cols[2];
            item.itemImage = cols[3];
            item.itemName = cols[4];
            string boolStr = string.IsNullOrEmpty(cols[5]) ? "false" : cols[5].Trim().ToLower();
            item.buyLimit = (boolStr == "true");
            item.limitCount = string.IsNullOrEmpty(cols[6]) || cols[6] == "null" ? 0 : int.Parse(cols[6]);
            item.buyPrice = string.IsNullOrEmpty(cols[7]) || cols[7] == "null" ? 0 : int.Parse(cols[7]);
            item.sellPrice = string.IsNullOrEmpty(cols[8]) || cols[8] == "null" ? 0 : int.Parse(cols[8]);
            item.npcID = cols[9];

            itemList.Add(item);
        }
        return itemList;
    }
}


