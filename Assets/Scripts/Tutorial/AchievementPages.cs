using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_014");
        image.sprite = sprite;

        titleText.text = "[도감]";
        detailText.text = "게임의 진행상황에 따라서 획득하는 도감을 확인 할 수 있습니다";
        pageText.text = "1 / 2";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_029");
        image.sprite = sprite;

        titleText.text = "[퀘스트]";
        detailText.text = "NPC를 통해서 퀘스트를 진행 할 수 있습니다";
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
