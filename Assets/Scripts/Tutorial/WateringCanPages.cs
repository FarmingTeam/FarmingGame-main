
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WateringCanPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_016");
        image.sprite = sprite;

        titleText.text = "[물이 부족해요]";
        detailText.text = "물뿌리개에 물이 다 떨어졌을 때는 물이 있는 곳에서 상호작용 키를 누르면 물이 채워집니다";
        pageText.text = "1 / 2";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_017");
        image.sprite = sprite;

        titleText.text = "[물이 부족해요]";
        detailText.text = "물뿌리개에 물이 다 떨어졌을 때는 물이 있는 곳에서 상호작용 키를 누르면 물이 채워집니다";
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
