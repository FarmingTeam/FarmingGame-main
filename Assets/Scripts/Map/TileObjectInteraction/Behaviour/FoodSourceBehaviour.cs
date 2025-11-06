using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.Cooking;
    }
}
