using UnityEngine;

public class WetDirtTileBehaviour : TileFloorInteraction
{
    public override void Interaction(Equipment tool, Tile tile)
    {

    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.None;
    }
}
