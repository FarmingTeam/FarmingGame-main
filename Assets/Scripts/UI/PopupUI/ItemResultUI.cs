using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemResultUI : UIBase
{
    [SerializeField] Image itemimage;
    [SerializeField] TextMeshProUGUI context;
    [SerializeField] TextMeshProUGUI description;

    public void OnEnable()
    {
        TimeManager.Instance.PauseTime(true);
    }

    public void SetPopupWithID(int ID)
    {
        ItemData item = ResourceManager.Instance.GetItem(ID);
        itemimage.sprite = item.itemIcon;
        if (IsItemFish(ID))
            context.text = item.itemName + "을 낚았습니다";
        else
            context.text = item.itemName + "을 얻었습니다";
        description.text = item.itemDescription;
    }

    public void OnCloseFishingResult()
    {
        TimeManager.Instance.PauseTime(false);
        if (!PlayerManager.Instance.playerStamina.Check())
            MapControl.Instance.player.playerAnimation.ForceSleepAnim();

        MapControl.Instance.player.controller.IsInteract = false;
        Destroy(gameObject);
    }

    bool IsItemFish(int ID)
    {
        if(ID / 100 == 11)
            return true;
        return false;

    }
}
