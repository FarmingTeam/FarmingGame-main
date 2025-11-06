using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 상호작용으로 차오르는 게이지
public class IngredientFill : MonoBehaviour
{
    int index;

    [SerializeField] RectTransform fillRect;

    public void SetUI(int index)
    {
        this.index = index;
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = new Vector2(0, 80.0f);
        gameObject.SetActive(false);
    }

    public void UpdateUI(float startpos)
    {
        gameObject.SetActive(true);
        fillRect.anchoredPosition = new Vector3 (startpos, 0, 0);
    }

    public void OnPress(float fillSpeed)
    {
        fillRect.sizeDelta = new Vector2(fillRect.sizeDelta.x + fillSpeed * Time.deltaTime, fillRect.sizeDelta.y);
    }

    //퇴장 시 현재 크기리턴
    public float ExitPress()
    {
        return fillRect.anchoredPosition.x + fillRect.sizeDelta.x;
    }

}


