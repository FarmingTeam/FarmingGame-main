using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipePopup : MonoBehaviour
{
    //Refactor : csv파일로 불러오기
    [SerializeField] public List<Recipe> recipes;
    [SerializeField] public List<RecipeButton> recipebuttons;

    public void Start()
    {
        for (int i = 0; i < recipebuttons.Count; i++)
        {
            if (i < recipes.Count)
            {
                recipebuttons[i].Init(recipes[i]);
                recipebuttons[i].gameObject.SetActive(true);
            }
            else
            {
                recipebuttons[i].gameObject.SetActive(false);
            }
        }
    }
}
