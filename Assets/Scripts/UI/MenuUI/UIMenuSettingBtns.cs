using System.Collections;
using UnityEngine;

public class UIMenuSettings : MonoBehaviour
{
    int currentSelect = -1;
    [SerializeField] UISaveDataBox[] saveSlots = new UISaveDataBox[3];

    [SerializeField] RectTransform SaveConfirmUI;
    [SerializeField] RectTransform LoadConfirmUI;
    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        currentSelect = -1;
        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].UpdateUI(i);
        }
        SaveConfirmUI.gameObject.SetActive(false);
        LoadConfirmUI.gameObject.SetActive(false);
    }

    public void OnPressSlot(int slotnumber)
    {
        if (saveSlots[slotnumber].toggle.isOn)
            currentSelect = slotnumber;
        else
            currentSelect = -1;
    }

    public void OnPressMainMenu()
    {
        SceneChangeManager.Instance.ChangeScene(SceneName.StartScene);
    }

    public void OnPressSave()
    {
        if (currentSelect == -1)
            return;
        SaveConfirmUI.gameObject.SetActive(true);
    }

    public void OnPressLoad()
    {
        if (currentSelect == -1)
            return;
        if (!saveSlots[currentSelect].HasSaveData)
            return;
        LoadConfirmUI.gameObject.SetActive(true);
    }

    public void OnPressSaveConfirm()
    {
        if (currentSelect == -1)
            throw new System.Exception("Not Valid SaveSlot");
        SaveManager.Instance.OnSaveSlot(currentSelect);

        StartCoroutine(Init());
    }

    public void OnPressLoadConfirm()
    {
        if (currentSelect == -1)
            throw new System.Exception("Not Valid SaveSlot");
        SaveManager.Instance.OnLoadSlot(currentSelect);
    }

    public void OnPressCancel()
    {
        SaveConfirmUI.gameObject.SetActive(false);
        LoadConfirmUI.gameObject.SetActive(false);
    }
}
