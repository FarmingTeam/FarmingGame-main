using UnityEngine;
using UnityEngine.InputSystem;

public class CookingModule : MonoBehaviour
{
    [SerializeField] public CookingMinigame cookingMinigame;
    [SerializeField] RecipePopup recipePopup;
    [SerializeField] CookingIngredientListPanel cookingIngredientListPanel;
    [SerializeField] Animator unSufficientAnimatior;
    [SerializeField] bool cheatCode = false;
    Recipe recipe = null;

    public bool isActive = false;
    public bool isPlaying = false;

    public void StartCooking()
    {
        gameObject.SetActive(true);
        TimeManager.Instance.PauseTime(true);
        isActive = true;
        recipe = null;
        cookingIngredientListPanel.Init(recipe);
        recipePopup.gameObject.SetActive(true);

        StartCoroutine(TutSpawner.Instance.DisplayTutorial(17));
    }

    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
        cookingIngredientListPanel.Init(recipe);
    }

    public void WhileCooking(InputAction.CallbackContext context)
    {
        if (isPlaying)
        {
            cookingMinigame.OnControl(context);
        }
    }

    public void CheckRecipeAndStartMinigame()
    {
        if (recipe == null)
            return;
#if UNITY_EDITOR
        //Refactor : 제거
        if (cheatCode)
        {
            for (int i = 0; i < recipe.recipeInfo.Length; i++)
            {
                PlayerInventory.Instance.AdditemsByID(recipe.recipeInfo[i].itemID, 1);
            }
        }
#endif
        //Refactor : end
        if (cookingMinigame.CanStart(recipe))
        {
            cookingMinigame.Init(recipe);
            recipePopup.gameObject.SetActive(false);
        }
        else
        {
            if(unSufficientAnimatior.GetCurrentAnimatorStateInfo(0).IsName("Reset"))
                unSufficientAnimatior.SetTrigger("UnSufficient");
        }
    }

    public void EndCooking()
    {
        isActive = false;
        isPlaying = false;
        recipe = null;
        recipePopup.gameObject.SetActive(false);
        cookingMinigame.gameObject.SetActive(false);
        TimeManager.Instance.PauseTime(false);
        gameObject.SetActive(false);
    }
}
