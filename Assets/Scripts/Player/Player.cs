
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerController controller;
    [SerializeField] public Animator playerAnimator;
    [SerializeField] public TileReader tileReader;
    [SerializeField] public ToolPivot tool;
    [SerializeField] public PlayerInventory inventory;
    [SerializeField] public PlayerEquipment equipment;
    [SerializeField] public InteractChecker interactChecker;
    [SerializeField] public MiniGameResource miniGameResource;
    [SerializeField] public FishingMinigame fishingResource;
    [SerializeField] public PlayerAnimation playerAnimation;
    [SerializeField] public CookingModule cookingResource;


    public void InitPos(Vector3 pos)
    {
        tileReader.Init();
        interactChecker.Init();
        this.transform.position = pos + new Vector3(0.5f,0.5f);
        miniGameResource.Init();

        if (PlayerManager.Instance.ToolIdx != -1)
        {
            tool.SelectQuickslot(PlayerManager.Instance.ToolIdx);
        }
        tileReader.SetFacing(PlayerManager.Instance.Direction);
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(1));
    }

}
