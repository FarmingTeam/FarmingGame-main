
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_013");
        image.sprite = sprite;

        titleText.text = "[인벤토리]";
        detailText.text = "키보드의 'TAB'키를 누르면 인벤토리를 열 수 있습니다";
        pageText.text = "1 / 2";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_031");
        image.sprite = sprite;

        titleText.text = "[인벤토리]";
        detailText.text = "왼쪽에 있는 탭을 누르면 해당 메뉴를 전환 할 수 있습니다";
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
