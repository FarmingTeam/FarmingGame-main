using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum FloorInteractionType
{
    None = 0,
    Grass = 1,
    Dirt,
    WetDirt,
    Water

}

//레이어 1번
//타일 바닥 자체의 기능들
[System.Serializable]
[CreateAssetMenu(fileName = "TileFloor", menuName = "TileInfo/Floor")]
public class TileFloor : ScriptableObject
{
    public FloorInteractionType floorType;
    public TileBase tileBase;
    public Color tileColor;
    public TileFloorInteraction interaction;

    private void Awake()
    {
        switch (floorType)
        {
            default:
                return;
        }
    }
}
