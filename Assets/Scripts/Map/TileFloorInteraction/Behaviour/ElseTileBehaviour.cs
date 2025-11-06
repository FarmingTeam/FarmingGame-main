using UnityEngine;

public class ElseTileBehaviour : TileFloorInteraction
{
    public override void Interaction(Equipment tool, Tile tile)
    {
        return;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.None;
    }
}
