using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    [SerializeField] CookingModule cookingModule;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI ItemName;
    Recipe recipe;

    public void Init(Recipe recipe)
    {
        this.recipe = recipe;
        ItemData res = ResourceManager.Instance.GetItem(recipe.resultItemID);
        itemImage.sprite = res.itemIcon;
        ItemName.text = res.itemName;
    }

    public void OnRecipeSelect()
    {
        cookingModule.SetRecipe(recipe);
    }

    public void OnCancel()
    {
        cookingModule.EndCooking();
    }

}
