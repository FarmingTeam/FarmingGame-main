using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellBoxPages : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text detailText;
    [SerializeField] private TMP_Text pageText;

    private void Start()
    {
        PageOne();
    }

    private void PageOne()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_032");
        image.sprite = sprite;

        titleText.text = "[판매상자]";
        detailText.text = "마을의 상점까지 가지 않아도 여기에서 소지품을 판매 할 수 있습니다";
        pageText.text = "1 / 2";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_033");
        image.sprite = sprite;

        titleText.text = "[판매만 가능]";
        detailText.text = "판매박스에서는 판매만 가능 합니다 물품 구입은 마을의 상점을 이용해 주세요";
        pageText.text = "2 / 2";
    }

    public void PageUp()
    {
        switch (pageText.text)
        {
            case "1 / 2":
                PageTwo();
                break;
            case "2 / 2":
                return;
            default:
                Debug.Log("코드 고쳐라");
                break;
        }
    }

    public void PageDown()
    {
        switch (pageText.text)
        {
            case "1 / 2":
                return;
            case "2 / 2":
                PageOne();
                break;
            default:
                Debug.Log("코드 고쳐라");
                break;
        }
    }
}
