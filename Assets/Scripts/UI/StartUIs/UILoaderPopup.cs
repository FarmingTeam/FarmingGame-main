
using UnityEngine;

public class UILoaderPopup : MonoBehaviour
{
    public int selectNum = -1;

    public void OnLoadConfirm()
    {
        if (selectNum == -1)
            return;
        gameObject.SetActive(false);
        SaveManager.Instance.OnLoadSlot(selectNum);
    }

    public void OnLoadCanceld()
    {
        selectNum = -1;
        gameObject.SetActive(false);
    }
}
