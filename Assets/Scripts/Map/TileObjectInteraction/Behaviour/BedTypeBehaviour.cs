
using UnityEngine;

public class BedTypeBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        MapControl.Instance.player.controller.IsInteract = true;
        UIManager.Instance.GetUI<SleepPopup>().gameObject.SetActive(true);
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.WithoutStaminaConsume;
    }
}
