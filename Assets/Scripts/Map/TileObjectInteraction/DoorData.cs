using UnityEngine;

[CreateAssetMenu(fileName = "DoorData", menuName = "TileInfo/ChunkData/Door")]
public class DoorData : ChunkData
{
    [SerializeField] public Vector2Int Destination;
}