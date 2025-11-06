using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingIngredientListPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] ingredientsText;
    [SerializeField] Button cookingConfirmButton;
    public void Init(Recipe recipe = null)
    {
        if (recipe == null)
        {
            for (int i = 0; i < ingredientsText.Length; i++)
            {
                ingredientsText[i].text = "";
            }
            cookingConfirmButton.interactable = false;

            return;
        }
        for (int i = 0; i < recipe.recipeInfo.Length; i++)
        {
            ingredientsText[i].text = ResourceManager.Instance.GetItem(recipe.recipeInfo[i].itemID).itemName;
        }
        cookingConfirmButton.interactable = true;
    }

}
