
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{

    public int toolId;
    public int toolLevel;
    public int firstIngredientId;
    public int firstIngredientAmount;


    public int secondIngredientId;
    public int secondIngredientAmount;

    public int requireGold;
    public int nextrequireGold;



    public Upgrade(string toolId, string toollevel, string firstIngredientId, string firstIngredientAmount, string secondIngredientId, string secondIngredientAmount, string requireGold, string nextGold)
    {
        this.toolId = int.Parse(toolId);
        this.toolLevel = int.Parse(toollevel);
        this.firstIngredientId = int.Parse(firstIngredientId);
        this.firstIngredientAmount = int.Parse(firstIngredientAmount);
        if(secondIngredientId!="null")
        {
            this.secondIngredientId = int.Parse(secondIngredientId);
        }
        if(secondIngredientAmount!="null")
        {
            this.secondIngredientAmount = int.Parse(secondIngredientAmount);
        }
        this.requireGold = int.Parse(requireGold);
        this.nextrequireGold = int.Parse(nextGold);
        
    }
}
