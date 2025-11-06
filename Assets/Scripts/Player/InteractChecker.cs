
using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractChecker : MonoBehaviour
{
    [SerializeField] private TileReader tileReader;
    [SerializeField] private InteractableSign interactableSign;
    private Tilemap tilemap;
    

    public void Init()
    {
        tilemap = MapControl.Instance.map.AllLayer;
    }
    private void OnEnable()
    {
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(2));
    }


    void Update()
    {
        var inter = tileReader.FrontCell();
        Vector2 pos = tilemap.GetCellCenterWorld(inter);
        transform.position = inter;
    }

    public void OnChange(Equipment tool)
    {
        if (MapControl.Instance.map.IsInteractMinigame((Vector2Int)tileReader.FrontCell(), tool) == MinigameInteractionType.None)
        {
            gameObject.SetActive(false);
            interactableSign.SignOff();
        }
        else
        {
            gameObject.SetActive(true);
            interactableSign.SignOn();
        }
    }
}
