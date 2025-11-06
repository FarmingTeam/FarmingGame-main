
using UnityEngine;

public class MapChangeLocator : MonoBehaviour
{
    [SerializeField] SceneName DestSceneName;
    [SerializeField] Vector2Int Position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !TransitController.Instance.transitor.isTransitioning)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.controller.moveInput = Vector2.zero;
            player.controller.IsInteract = true;
            SceneChangeManager.Instance.ChangeScene(DestSceneName, Position);
        }
    }
}
