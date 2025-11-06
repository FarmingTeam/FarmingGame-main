using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_035");
        image.sprite = sprite;

        titleText.text = "[요리하기]";
        detailText.text = "플레이어가 레시피를 선택하면 자동으로 재료가 선택되고 1번재료부터 상호작용을 누르는 시간에 따라 프로그레스바에 1f에 1%씩 차오릅니다";
        pageText.text = "1 / 5";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_036");
        image.sprite = sprite;

        titleText.text = "[요리방법]";
        detailText.text = "상호작용 키를 떼면 재료가 자동으로 다음 재료로 넘어갑니다";
        pageText.text = "2 / 5";
    }

    private void PageThree()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_037");
        image.sprite = sprite;

        titleText.text = "[성공조건]";
        detailText.text = "재료 필요량에서 ±3% 안으로 재료량이 들어오면 성공입니다";
        pageText.text = "3 / 5";
    }

    private void PageFour()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_038");
        image.sprite = sprite;

        titleText.text = "[요리 완성]";
        detailText.text = "재료마다 성공판정을 매기고 3개 중 2개가 성공판정일 때 요리가 완성됩니다";
        pageText.text = "4 / 5";
    }

    private void PageFive()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_039");
        image.sprite = sprite;

        titleText.text = "[실패]";
        detailText.text = "2개 이상이 실패 판정이면 음식물 쓰레기가 완성됩니다";
        pageText.text = "5 / 5";
    }

    public void PageUp()
    {
        switch (pageText.text)
        {
            case "1 / 5":
                PageTwo();
                break;
            case "2 / 5":
                PageThree();
                break;
            case "3 / 5":
                PageFour();
                break;
            case "4 / 5":
                PageFive();
                break;
            case "5 / 5":
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
            case "1 / 5":
                return;
            case "2 / 5":
                PageOne();
                break;
            case "3 / 5":
                PageTwo();
                break;
            case "4 / 5":
                PageThree();
                break;
            case "5 / 5":
                PageFour();
                break;
            default:
                Debug.Log("코드 고쳐라");
                break;
        }
    }
}
