
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmingPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_003");
        image.sprite = sprite;

        titleText.text = "[농사를 지어 보세요]";
        detailText.text = "이장님에게서 지급받은 도구를 이용해서 농사를 지어 보세요";
        pageText.text = "1 / 7";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_004");
        image.sprite = sprite;

        titleText.text = "[농경지 정리하기]";
        detailText.text = "농경지에 자라 있는 풀, 꽃, 버섯, 나무를 치워 주세요";
        pageText.text = "2 / 7";
    }

    private void PageThree()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_005");
        image.sprite = sprite;

        titleText.text = "[농경지 경작하기]";
        detailText.text = "괭이를 들고 비어있는 땅에 상호작용 키를 누르면 땅이 갈리면서 씨앗을 심을 수 있게 됩니다";
        pageText.text = "3 / 7";
    }

    private void PageFour()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_006");
        image.sprite = sprite;

        titleText.text = "[씨앗 뿌리기]";
        detailText.text = "씨앗 바구니를 선택하고 심을 작물을 선택한 후 경작되어 있는 땅에 상호작용키를 누르면 씨앗이 심어 집니다";
        pageText.text = "4 / 7";
    }

    private void PageFive()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_007");
        image.sprite = sprite;

        titleText.text = "[작물에 물주기]";
        detailText.text = "물 뿌리개를 선택하고 씨앗이 심어져 있는 곳에 상호작용 키를 누르면 작물에 물을 줄 수 있습니다";
        pageText.text = "5 / 7";
    }

    private void PageSix()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_008");
        image.sprite = sprite;

        titleText.text = "[작물의 성장]";
        detailText.text = "심어져 있는 작물에 물을 주고 일정 날짜가 지나면 작물이 성장합니다";
        pageText.text = "6 / 7";
    }

    private void PageSeven()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_009");
        image.sprite = sprite;

        titleText.text = "[수확하기]";
        detailText.text = "낫을 선택하고 다 자란 작물이 있는 곳에 상호작용 키를 누르면 수확을 할 수 있습니다 ";
        pageText.text = "7 / 7";
    }

    public void PageUp()
    {
        switch (pageText.text)
        {
            case "1 / 7" :
                PageTwo();
                break;
            case "2 / 7" :
                PageThree();
                break;
            case "3 / 7":
                PageFour();
                break;
            case "4 / 7":
                PageFive();
                break;
            case "5 / 7":
                PageSix();
                break;
            case "6 / 7":
                PageSeven();
                break;
            case "7 / 7":
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
            case "1 / 7":
                return;
            case "2 / 7":
                PageOne();
                break;
            case "3 / 7":
                PageTwo();
                break;
            case "4 / 7":
                PageThree();
                break;
            case "5 / 7":
                PageFour();
                break;
            case "6 / 7":
                PageFive();
                break;
            case "7 / 7":
                PageSix();
                break;
            default:
                Debug.Log("코드 고쳐라");
                break;
        }
    }

    public void NextTutCall()
    {
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(10));
    }
}
