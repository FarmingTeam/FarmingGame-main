using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTypeBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        if ((Vector2Int)MapControl.Instance.player.tileReader.FrontCell() == tile.pos)
        {
            TimeManager.Instance.PauseTime(true);
            UIManager.Instance.OpenUI<UIUpgrade>();
        }

        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.WithoutStaminaConsume;
    }
}
