using System;
using UnityEngine;

[Serializable]
public class Tile
{
    public Vector2Int pos;
    public FloorInteractionType floorInteractionType { get; set; }
    public ChunkData chunkData { get; set; }
    public TileSeed seed { get; set; } = new TileSeed ();

    public Tile()
    {
        this.floorInteractionType = FloorInteractionType.None;
        this.chunkData = TileDataBase.Instance.GetChunkDataByID(0);
    }
}
