using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTypeBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        TimeManager.Instance.PauseTime(true);
        UIManager.Instance.GetUI<TutorialList>().gameObject.SetActive(true);
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.WithoutStaminaConsume;
    }
}
