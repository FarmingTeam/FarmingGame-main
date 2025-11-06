using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientMeasure : MonoBehaviour
{
    [SerializeField] RectTransform fillRect;
    public void SetUI(float startpos, float fill)
    {
        gameObject.SetActive(true);
        fillRect.anchoredPosition = new Vector2(startpos, 0);
        fillRect.sizeDelta = new Vector2(fill, fillRect.sizeDelta.y);
    }
}
