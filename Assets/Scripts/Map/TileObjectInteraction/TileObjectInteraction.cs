
using UnityEngine;

public abstract class TileObjectInteraction
{
    public abstract MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile);

    public abstract bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData);
}
