using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public struct RecipeIngredients
{
    public int itemID;
    public float ratio;

    public RecipeIngredients(int itemID, float ratio)
    {
        this.itemID = itemID;
        this.ratio = ratio;
    }
}
[Serializable]
public class Recipe
{
    public RecipeIngredients[] recipeInfo;
    public int resultItemID;
}



public class CookingMinigame : MonoBehaviour
{
    //Info
    public Recipe recipe;
    int ItemCount;
    int currentIndex;

    //Parameters
    [SerializeField] CookingItemIcon[] cookingItems;
    [SerializeField] private Image progressFill;
    [SerializeField] IngredientFill[] ingredientFill;
    [SerializeField] IngredientMeasure[] ingredientMeasure;

    [SerializeField] private float fillSpeed;
    [SerializeField] private float successMargain = 0.03f;


    //Setup
    Coroutine currentCorutine = null;
    float total = 0.0f;
    float[] resultratios;
    float[] ratiotable;
    [SerializeField] CookingModule cookingModule;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] int TrashID = 3002;


    public bool CanStart(Recipe recipe)
    {
        for (int i = 0; i < recipe.recipeInfo.Length; i++)
        {
            if (!PlayerInventory.Instance.SubtractItemQuantity(recipe.recipeInfo[i].itemID, 1))
            {
                i--;
                while (i >= 0)
                {
                    PlayerInventory.Instance.AdditemsByID(recipe.recipeInfo[i].itemID, 1);
                    i--;
                }
                return false;
            }
        }
        return true;
    }


    public void Init(Recipe recipe)
    {
        this.recipe = recipe;
        if (this.recipe == null)
            return;
        ItemCount = this.recipe.recipeInfo.Length;
        currentIndex = 0;
        if (currentCorutine != null)
        {
            StopCoroutine(currentCorutine);
        }

        currentCorutine = null;
        total = 0.0f;
        resultratios = new float[ItemCount];
        ratiotable = new float[ItemCount];
        SetDefaultUI();
        cookingModule.isPlaying = true;
        gameObject.SetActive(true);
    }

    //Refcator : Move
    public void OnControl(InputAction.CallbackContext context)
    {
        if (!cookingModule.isPlaying)
            return;
        if (context.phase == InputActionPhase.Started && currentCorutine == null)
        {
            currentCorutine = StartCoroutine(OnPress());
        }
        else if (context.phase == InputActionPhase.Canceled && currentCorutine != null && currentIndex != ItemCount-1)
        {
            OnNext();
        }
    }


    private void SetDefaultUI()
    {
        float sum = 0.0f;
        for (int i = 0; i < ItemCount; i++)
        {
            sum += recipe.recipeInfo[i].ratio;
        }
        float startpos = 0.0f;
        float uisize = progressFill.rectTransform.sizeDelta.x;
        for (int i = 0; i < ItemCount; i++)
        {
            cookingItems[i].SetUI(ResourceManager.Instance.GetItem(recipe.recipeInfo[i].itemID), i);
            cookingItems[i].UpdateUI(currentIndex);
            ingredientFill[i].SetUI(currentIndex);

            ingredientMeasure[i].SetUI(startpos, uisize * (recipe.recipeInfo[i].ratio / sum));
            ratiotable[i] = recipe.recipeInfo[i].ratio / sum;
            startpos += uisize * (recipe.recipeInfo[i].ratio / sum);
        }
        ingredientFill[0].UpdateUI(0.0f);
    }

    private IEnumerator OnPress()
    {
        while (true)
        {
            ingredientFill[currentIndex].OnPress(fillSpeed);
            total += Time.deltaTime * fillSpeed;
            if (total >= progressFill.rectTransform.sizeDelta.x)
            {
                ForceBreak();
                break;
            }

            yield return null;
            if (currentCorutine == null || !cookingModule.isPlaying)
                break;
        }
    }

    private IEnumerator AutoFill()
    {
        while (total < progressFill.rectTransform.sizeDelta.x)
        {
            ingredientFill[currentIndex].OnPress(fillSpeed);
            total += Time.deltaTime * fillSpeed;
            yield return null;
        }
        currentCorutine = null;
        cookingItems[currentIndex].UpdateUI(currentIndex + 1);
        resultratios[currentIndex] = progressFill.rectTransform.sizeDelta.x;
        currentIndex++;
        OnEndGame();
    }

    private void OnNext()
    {
        StopCoroutine(currentCorutine);
        currentCorutine = null;
        cookingItems[currentIndex].UpdateUI(currentIndex+1);
        resultratios[currentIndex] = ingredientFill[currentIndex].ExitPress();
        currentIndex++;

        ingredientFill[currentIndex].UpdateUI(resultratios[currentIndex - 1]);
        cookingItems[currentIndex].UpdateUI(currentIndex);
        if (currentIndex == ItemCount - 1)
        {
            currentCorutine = StartCoroutine(AutoFill());
        }
    }

    private void ForceBreak()
    {
        StopCoroutine(currentCorutine);
        currentCorutine = null;
        total = progressFill.rectTransform.sizeDelta.x;
        while(currentIndex < ItemCount)
        {
            cookingItems[currentIndex].UpdateUI(currentIndex + 1);
            ingredientFill[currentIndex].UpdateUI(total);
            resultratios[currentIndex] = total;
            currentIndex++;
        }
        currentIndex = ItemCount;
        OnEndGame();
    }

    private bool CheckResult()
    {
        int successcount = 0;
        float start = 0.0f;
        for (int i = 0; i < ItemCount; i++)
        {
            float actualrate = (resultratios[i] - start) / progressFill.rectTransform.sizeDelta.x;
            if (actualrate >= recipe.recipeInfo[i].ratio * (1.0f - successMargain) && actualrate <= recipe.recipeInfo[i].ratio * (1.0f + successMargain))
                successcount++;
            start = resultratios[i];
        }

        return successcount >= ItemCount / 2.0f;
    }

    //Refactor : 추후 작업
    private void OnEndGame()
    {
        playerAnimation.MinigameResult = SuccessStatus.Good;
        if (CheckResult())
        {
            //요리 성공
            PlayerInventory.Instance.AdditemsByID(recipe.resultItemID, 1);
            SFXManager.Instance.PlaySFX(SFXManager.SFXType.Success);
            playerAnimation.itemResult = recipe.resultItemID;
        }
        else
        {
            //요리 실패

            PlayerInventory.Instance.AdditemsByID(TrashID, 1);
            SFXManager.Instance.PlaySFX(SFXManager.SFXType.Fail);
            playerAnimation.itemResult = TrashID;
        }

        playerAnimation.PickingAnim();
        cookingModule.EndCooking();
    }
}
