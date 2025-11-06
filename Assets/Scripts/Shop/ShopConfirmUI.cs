
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopConfirmUI : MonoBehaviour
{
    public GameObject blockPanel;
    public TextMeshProUGUI messageText;
    public Button yesButton;
    public Button noButton;
    public ScrollRect scrollRect;
    public Scrollbar scrollbar;

    public void Show(string message, System.Action onYes, System.Action onNo)
    {
        messageText.text = message;
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { onYes?.Invoke(); Hide(); });
        noButton.onClick.AddListener(() => { onNo?.Invoke(); Hide(); });
        blockPanel.SetActive(true);
        gameObject.SetActive(true);

        if (scrollRect != null) scrollRect.enabled = false;
        if (scrollbar != null) scrollbar.interactable = false;
    }

    public void Hide()
    {
        blockPanel.SetActive(false);  
        gameObject.SetActive(false);

        if (scrollRect != null) scrollRect.enabled = true;
        if (scrollbar != null) scrollbar.interactable = true;
    }
}
