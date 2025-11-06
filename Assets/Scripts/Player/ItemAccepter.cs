
using UnityEngine;

public class ItemAccepter : MonoBehaviour, DroppedItem
{
    public bool Collect(ItemData data, int amount)
    {
        if (data == null || amount <= 0) return false;
        return AddInventory((int)data.itemID, amount);
    }

    private bool AddInventory(int id, int amount)
    {
        PlayerInventory.Instance.AdditemsByID(id, amount);
        return true;
    }
}
