
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    [SerializeField] private Transitor transitor;
    [SerializeField] private TextMeshProUGUI textComponent;

    public void ActiveTxt()
    {
        Color c = textComponent.color;
        c.a = 1f; // 알파 1 = 255와 동일 (0~1 범위)
        textComponent.color = c;
        gameObject.SetActive(true);
    }

    public void CloseText()
    {
        Color c = textComponent.color;
        c.a = 0f;
        textComponent.color = c;
        transitor.TransitDone();
        gameObject.SetActive(false);
    }

}
