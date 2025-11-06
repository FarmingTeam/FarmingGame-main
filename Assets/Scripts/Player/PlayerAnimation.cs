
using UnityEngine;
using static SFXManager;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    [SerializeField] private FailAnimator failAnimator;

    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileReader tileReader;

    [Header("Tool")]
    [SerializeField]private ToolAnimationController toolAnimationController;

    //Minigame Result
    public SuccessStatus MinigameResult = SuccessStatus.None;

    public int itemResult = -1;
    [SerializeField] ItemResultUI FishingSuccess;

    public void AtStartForceSleep()
    {
        playerController.IsInteract = true;
        TimeManager.Instance.PauseTime(true);
    }

    public void AtEndForceSleep()
    {
        TimeManager.Instance.forceSleep = true;
        TimeManager.Instance.PauseTime(false);
        playerController.IsInteract = false;
    }


    public void AtStartInteraction()
    {
        playerController.IsInteract = true;
    }

    public void AtEndInteraction()
    {
        if (MinigameResult == SuccessStatus.None)
        {
            playerController.IsInteract = false;
            return;
        }


        Vector2Int InteractionCell = (Vector2Int)player.tileReader.FrontCell();
        //미니게임 결과에 따른 처리
        if (MinigameResult == SuccessStatus.Fail)
        {
            //Refactor : 실패 효과음
            failAnimator.ForcePlay();
        }
        else if (MinigameResult == SuccessStatus.WithOutMinigame)
        {
            MapControl.Instance.map.OnPlayerInteract(InteractionCell, player.tool.CurrentEquip);
        }
        else
        {
            int interactionrange = player.tool.CurrentEquip.equipmentUpgrade + 1;

            if (MinigameResult == SuccessStatus.VeryGood)
                interactionrange += 2;
            Vector2Int interactionDirection = Vector2Int.zero;
            switch (player.tileReader.currentFacing)
            {
                case TileReader.Facing.Up:
                    interactionDirection = Vector2Int.up; break;
                case TileReader.Facing.Down:
                    interactionDirection = Vector2Int.down; break;
                case TileReader.Facing.Left:
                    interactionDirection = Vector2Int.left; break;
                case TileReader.Facing.Right:
                    interactionDirection = Vector2Int.right; break;

            }
            for (int i = 0; i < interactionrange; i++)
            {
                MapControl.Instance.map.OnPlayerInteract(InteractionCell + interactionDirection * i, player.tool.CurrentEquip);
            }

        }
        player.interactChecker.OnChange(player.tool.CurrentEquip);
        PlayerManager.Instance.playerStamina.Consume(playerController.StaminaCost);
        MinigameResult = SuccessStatus.None;
        if (itemResult != -1)
        {
            ItemResultUI fishingUI = Instantiate(FishingSuccess);
            fishingUI.SetPopupWithID(itemResult);
            itemResult = -1;

            return;
        }

        else if (!PlayerManager.Instance.playerStamina.Check())
            ForceSleepAnim();

        playerController.IsInteract = false;

    }

    public void PickingAnim()
    {
        var face = tileReader.currentFacing;

        switch (face)
        {
            case TileReader.Facing.Up:
                int blendIndex = 1;
                animator.SetFloat("Blend", (float)blendIndex);
                Debug.Log("뒤쪽보고 줍기동작");
                break;

            case TileReader.Facing.Down:
                blendIndex = 0;
                animator.SetFloat("Blend", (float)blendIndex);
                Debug.Log("앞쪽보고 줍기동작");
                break;

            case TileReader.Facing.Left:
                blendIndex = 2;
                animator.SetFloat("Blend", (float)blendIndex);
                Debug.Log("왼쪽보고 줍기동작");
                break;

            case TileReader.Facing.Right:
                blendIndex = 3;
                animator.SetFloat("Blend", (float)blendIndex);
                Debug.Log("오른쪽보고 줍기동작");
                break;

        }

        SFXManager.Instance.PlaySFX(SFXManager.SFXType.PickUp);

        animator.SetTrigger("IsPicking");

    }

    public void InteractAnim()
    {
        bool itemfront = MapControl.Instance.map.IsFlowerOrMushroom((Vector2Int)tileReader.FrontCell());

        if (itemfront == true)
        {
            PickingAnim();
            return;
        }
        else
        {
            var face = tileReader.currentFacing;


            switch (face)
            {
                case TileReader.Facing.Up:
                    int blendIndex = 1;
                    animator.SetFloat("Blend", (float)blendIndex);
                    Debug.Log("뒤쪽보고 상호작용");
                    break;

                case TileReader.Facing.Down:
                    blendIndex = 0;
                    animator.SetFloat("Blend", (float)blendIndex);
                    Debug.Log("앞쪽보고 상호작용");
                    break;

                case TileReader.Facing.Left:
                    blendIndex = 2;
                    animator.SetFloat("Blend", (float)blendIndex);
                    Debug.Log("왼쪽보고 상호작용");
                    break;

                case TileReader.Facing.Right:
                    blendIndex = 3;
                    animator.SetFloat("Blend", (float)blendIndex);
                    Debug.Log("오른쪽보고 상호작용");
                    break;
            }



            animator.SetTrigger("IsOnInteract");
            toolAnimationController.ToolAction();

            // 도구별 효과음 처리
            if (player.tool.CurrentEquip.equipmentType == EquipmentType.Axe)
            {
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.HitWood);
            }
            else if (player.tool.CurrentEquip.equipmentType == EquipmentType.Pickaxe)
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.HitSton);
            else if (player.tool.CurrentEquip.equipmentType == EquipmentType.Hoe)
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.BrokenSton);
            else if (player.tool.CurrentEquip.equipmentType == EquipmentType.Sickle)
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.BrokenSton);

            else if (player.tool.CurrentEquip.equipmentType == EquipmentType.WateringCan)
            {

                bool frontwater = (MapControl.Instance.map.tiles[tileReader.FrontCell().x, tileReader.FrontCell().y].floorInteractionType == FloorInteractionType.Water);

                if (frontwater == true)
                    SFXManager.Instance.PlaySFX(SFXManager.SFXType.PumpWater);
                else
                    SFXManager.Instance.PlaySFX(SFXManager.SFXType.SprayingWater);
            }
            else if (player.tool.CurrentEquip.equipmentType == EquipmentType.Rod)
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.FishingReel);
        }
    }

    public void MoveAnim()
    {
        var face = tileReader.currentFacing;
        animator.SetBool("IsMove", playerController.moveInput.x != 0 || playerController.moveInput.y != 0);

        switch (face)
        {
            case TileReader.Facing.Up:
                int blendIndex = 1;
                animator.SetFloat("Blend", (float)blendIndex);
                break;

            case TileReader.Facing.Down:
                blendIndex = 0;
                animator.SetFloat("Blend", (float)blendIndex);
                break;

            case TileReader.Facing.Left:
                blendIndex = 2;
                animator.SetFloat("Blend", (float)blendIndex);
                break;

            case TileReader.Facing.Right:
                blendIndex = 3;
                animator.SetFloat("Blend", (float)blendIndex);
                break;

        }

    }

    public void ForceSleepAnim()
    {
        animator.SetBool("IsForceSleep", true);
    }

    

}
