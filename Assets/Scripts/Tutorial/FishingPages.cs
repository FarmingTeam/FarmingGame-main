
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_018");
        image.sprite = sprite;

        titleText.text = "[물고기를 낚아 보세요]";
        detailText.text = "낚시대를 장비하고 물가에서 상호작용키를 누르면 낚시를 할 수 있습니다";
        pageText.text = "1 / 3";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_019");
        image.sprite = sprite;

        titleText.text = "[판정선을 움직여 주세요]";
        detailText.text = "상호작용키를 연타하면 판정선이 오른쪽으로 움직입니다";
        pageText.text = "2 / 3";
    }

    private void PageThree()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_020");
        image.sprite = sprite;

        titleText.text = "[성공범위에 맞춰주세요]";
        detailText.text = "계속 움직이는 성공범위에 판정선을 일정시간 맞춰주면 성공입니다";
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
