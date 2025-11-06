using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stamina2Pages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_027");
        image.sprite = sprite;

        titleText.text = "[스태미나 저하]";
        detailText.text = "스태미나가 0이 되면 플레이어는 강제로 취침 모드로 들어갑니다";
        pageText.text = "1 / 2";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_028");
        image.sprite = sprite;

        titleText.text = "[강제 취침]";
        detailText.text = "강제 취침 모드시에는 스태미나가 절반밖에 회복되지 않으니 주의하세요";
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
