using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//위에 표시되는 요리 재료 아이콘
public class CookingItemIcon : MonoBehaviour
{
    int index;

    [SerializeField] Image Ingredient;
    [SerializeField] Image RedFrame;
    [SerializeField] Image CheckMark;

    //Settings
    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(48.0f / 255.0f, 48.0f / 255.0f, 48.0f / 255.0f, 1.0f);

    public void SetUI(ItemData item, int index)
    {
        Ingredient.sprite = item.itemIcon;
        Ingredient.color = inactiveColor;
        this.index = index;
        RedFrame.gameObject.SetActive(false);
        CheckMark.gameObject.SetActive(false);
    }

    public void UpdateUI(int currentIndex)
    {
        RedFrame.gameObject.SetActive(currentIndex == index);
        CheckMark.gameObject.SetActive(index < currentIndex);
        Ingredient.color = (index == currentIndex) ? activeColor : inactiveColor;
    }
}
