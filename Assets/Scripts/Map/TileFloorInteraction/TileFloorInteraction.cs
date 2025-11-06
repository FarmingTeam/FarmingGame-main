
using UnityEngine;

public abstract class TileFloorInteraction
{
    public abstract MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile);
    public abstract void Interaction(Equipment tool, Tile tile);
}
