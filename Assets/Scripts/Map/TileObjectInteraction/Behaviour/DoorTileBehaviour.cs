
using UnityEngine;

public class DoorTileBehaviour : TileObjectInteraction
{
    public override bool Interaction(Equipment tool, Tile tile, out ChunkData chunkData)
    {
        DoorData door = tile.chunkData as DoorData;
        switch ((int)tile.chunkData.chunkType % 10)
        {
            //Farm -> House
            case 0:
                SceneChangeManager.Instance.ChangeScene(SceneName.HouseScene, door.Destination);
                break;
            //House -> Farm
            case 1:
                SceneChangeManager.Instance.ChangeScene(SceneName.FarmScene, door.Destination);
                break;
            //Town -> Shop
            case 2:
                SceneChangeManager.Instance.ChangeScene(SceneName.ShopScene, door.Destination); break;
            //Shop -> Town
            case 3:
                SceneChangeManager.Instance.ChangeScene(SceneName.TownScene, door.Destination); break;
            default:
                break;
            
        }
        chunkData = null;
        return false;
    }

    public override MinigameInteractionType IsInteractMinigame(Equipment tool, Tile tile)
    {
        return MinigameInteractionType.WithoutStaminaConsume;
    }
}
