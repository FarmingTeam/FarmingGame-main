
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_010");
        image.sprite = sprite;

        titleText.text = "[QTE 미니게임]";
        detailText.text = "좌우로 움직이는 판정선이 성공범위에 정확한 타이밍에 상호작용을 하면 자원을 획득할 수 있습니다";
        pageText.text = "1 / 3";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_011");
        image.sprite = sprite;

        titleText.text = "[미니게임 대성공]";
        detailText.text = "판정선이 한 가운데의 대성공 판정선에서 정확하게 성공하면 주위 3X3 범위의 모든 범위의 자원이 획득됩니다";
        pageText.text = "2 / 3";
    }

    private void PageThree()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_012");
        image.sprite = sprite;

        titleText.text = "[미니게임 대성공]";
        detailText.text = "판정선이 한 가운데의 대성공 판정선에서 정확하게 성공하면 주위 3X3 범위의 모든 범위의 자원이 획득됩니다";
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
