using UnityEngine;

public class ItemDropTest : MonoBehaviour
{
    [Header("DropItem")]
    // 드롭할 아이템 프리팹 (ItemToPlayer가 붙어있는 오브젝트)
    public GameObject itemPrefab;
    public int runtimeItemID = 9001;
    public string runtimeItemName = "Test Item";
    [TextArea] public string runtimeItemDescription = "For runtime test";
    public string runtimeItemPath = "Items/Test";
    public ItemType runtimeItemType = ItemType.Others;
    public Sprite runtimeItemIcon;
    public int runtimeMaxQuantity = 99;


    [Header("PickItem")]
    // 줍기용 아이템 프리팹
    public GameObject itemPrefab2;
    public int runtimeItemID2 = 3;
    public string runtimeItemName2 = "초록이";
    [TextArea] public string runtimeItemDescription2 = "초록이이다";
    public string runtimeItemPath2 = "Green";
    public ItemType runtimeItemType2 = ItemType.Others;
    public Sprite runtimeItemIcon2;
    public int runtimeMaxQuantity2 = 1;

    public void DropItem(TileReader tileReader)
    {
        // 드롭 위치
        Vector2Int dropPoint = tileReader.FrontPoint;

        if (itemPrefab == null)
        {
            Debug.LogWarning("ItemPrefab이 할당되지 않았습니다.");
            return;
        }

        // 맵 내 좌표 생성
        Vector3Int dropPos = (Vector3Int)dropPoint;

        var go = Instantiate(itemPrefab, dropPos, Quaternion.identity);
        var itp = go.GetComponent<ItemToPlayer>();

        var runtimeData = ScriptableObject.CreateInstance<ItemData>();
        runtimeData.itemID = runtimeItemID;
        runtimeData.itemName = runtimeItemName;
        runtimeData.itemDescription = runtimeItemDescription;
        runtimeData.itemPath = runtimeItemPath;
        runtimeData.itemType = runtimeItemType;
        runtimeData.itemIcon = runtimeItemIcon;
        runtimeData.maxQuantity = Mathf.Max(1, runtimeMaxQuantity);
        runtimeData.isStackable = runtimeData.maxQuantity > 1;

        itp.data = runtimeData;

        // 아이템 생성
        Instantiate(itemPrefab, dropPos, Quaternion.identity);
        Debug.Log($"아이템 드롭됨: {dropPos}");

        // 좌표를 지정받아 출력할것. 1. 아이템 ID 2. 아이템 생성 위치(좌표) - 기능 구현 필요(재활용 원활하게)
    }

    public void PickableItem(TileReader tileReader)
    {
        Vector2Int dropPoint = (Vector2Int)tileReader.CurrentCell;

        if (itemPrefab2 == null)
        {
            Debug.LogWarning("ItemPrefab이 할당되지 않았습니다.");
            return;
        }

        Vector3Int dropPos = (Vector3Int)dropPoint;

        var go = Instantiate(itemPrefab2, dropPos, Quaternion.identity);
        var itp = go.GetComponent<ItemToPlayer>();

        var runtimeData = ScriptableObject.CreateInstance<ItemData>();
        runtimeData.itemID = runtimeItemID2;
        runtimeData.itemName = runtimeItemName2;
        runtimeData.itemDescription = runtimeItemDescription2;
        runtimeData.itemPath = runtimeItemPath2;
        runtimeData.itemType = runtimeItemType2;
        runtimeData.itemIcon = runtimeItemIcon2;
        runtimeData.maxQuantity = Mathf.Max(1, runtimeMaxQuantity2);
        runtimeData.isStackable = runtimeData.maxQuantity > 1;

        itp.data = runtimeData;

        // 아이템 생성
        Instantiate(itemPrefab, dropPos, Quaternion.identity);
        Debug.Log($"아이템 드롭됨: {dropPos}");
    }

}
