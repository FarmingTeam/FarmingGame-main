
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_021");
        image.sprite = sprite;

        titleText.text = "[수확물을 판매하세요]";
        detailText.text = "수확한 작물은 상점에서 판매할 수 있습니다";
        pageText.text = "1 / 3";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_022");
        image.sprite = sprite;

        titleText.text = "[상점]";
        detailText.text = "상점은 마을의 남서쪽에 있습니다";
        pageText.text = "2 / 3";
    }

    private void PageThree()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_023");
        image.sprite = sprite;

        titleText.text = "[상점 메뉴]";
        detailText.text = "상점에서는 획득한 아이템을 판매하고 씨앗을 구입할 수 있습니다";
        pageText.text = "3 / 3";
    }

    public void PageUp()
    {
        switch (pageText.text)
        {
            case "1 / 3":
                PageTwo();
                break;
            case "2 / 3":
                PageThree();
                break;
            case "3 / 3":
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
            case "1 / 3":
                return;
            case "2 / 3":
                PageOne();
                break;
            case "3 / 3":
                PageTwo();
                break;
            default:
                Debug.Log("코드 고쳐라");
                break;
        }
    }
}
