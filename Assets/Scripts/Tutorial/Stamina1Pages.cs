using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stamina1Pages : MonoBehaviour
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
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_025");
        image.sprite = sprite;

        titleText.text = "[스태미나]";
        detailText.text = "플레이어가 자원을 채집하거나 농사를 지을 때 스태미나가 감소합니다";
        pageText.text = "1 / 2";
    }

    private void PageTwo()
    {
        Sprite sprite = Resources.Load<Sprite>("TutImg/popimg_026");
        image.sprite = sprite;

        titleText.text = "[스태미나 회복]";
        detailText.text = "집에서 잠을 자거나 요리를 먹으면 스태미나를 회복할 수 있습니다";
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
