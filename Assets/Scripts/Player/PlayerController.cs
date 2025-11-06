
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    [Header("Common")]
    public float moveSpeed = 5f; // 플레이어 이동속도
    [SerializeField] private float runPower = 2.0f;

    public new Rigidbody2D rigidbody;

    [SerializeField] private TileReader tileReader;
    [SerializeField] private Player player;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private SFXManager sFXManager;
    [SerializeField] private AudioSource audio;
    [SerializeField] public int StaminaCost;

    private Vector3Int previousLook = Vector3Int.zero;

    public Vector2 moveInput;
    public bool IsInteract = false;
    public bool IsRun = false;

    [SerializeField] private FailAnimator failAnimator;
    private void Awake()
    {
        // 인스펙터에 비어 있으면 Player 컴포넌트에서 가져오기
        if (!player) player = GetComponentInParent<Player>();

        if (!tileReader)
        {
            var player = GetComponentInParent<Player>();
            tileReader = player ? player.tileReader : null;
        }

        GameObject sfxManager = GameObject.Find("SFXManager");
        audio = sfxManager.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {

        OnMove();
        
        UpdateFacingFromInput();
    }

    private void Update()
    {
        playerAnimation.MoveAnim();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!CanInput() || UIManager.Instance.currentUISwitch.isOpen)
        {
            moveInput = Vector2.zero;
            return;
        }
        if (context.phase == InputActionPhase.Performed)
        {
            moveInput = context.ReadValue<Vector2>(); // 이동 구현
        }
        else if (context.phase == InputActionPhase.Canceled)
            moveInput = Vector2.zero;
    }

    public void OnRunInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            moveSpeed *= runPower;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            moveSpeed /= runPower;
        }
    }

    public void OnInteractionAction(InputAction.CallbackContext context)
    {
        if (player.cookingResource.isPlaying)
        {
            player.cookingResource.WhileCooking(context);
            return;
        }
        if (context.phase == InputActionPhase.Started)
        {
            if (IsInteract || SceneChangeManager.Instance.IsSceneChange || NPCManager.Instance.IsInteracting || UIManager.Instance.currentUISwitch.isOpen)
                return;
            List<RaycastResult> UIRaycast = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            EventSystem.current.RaycastAll(eventData, UIRaycast);
            foreach (var uiGameObejct in UIRaycast)
            {
                if (uiGameObejct.gameObject.GetComponent<UIQuickSlot>() != null 
                    || uiGameObejct.gameObject.GetComponent<UIToolBar>()!= null
                    || uiGameObejct.gameObject.GetComponent<UISeedBasket>() != null)
                    return;
            }
            //NPC 대화 처리
            NPC InteractingNPC = NPCManager.Instance.CanNPCInteract((Vector2Int)player.tileReader.FrontCell());
            if (InteractingNPC != null)
            {
                moveInput = Vector2.zero;
                InteractingNPC.OnInteract();
                return;
            }

            Vector2Int cell = (Vector2Int)player.tileReader.FrontCell();
            Vector2 interactPos = new Vector2(cell.x, cell.y);

            ShopInteractable shop = ShopInteractable.FindInteractableShop(interactPos);
            if (shop != null)
            {
                moveInput = Vector2.zero;
                StartCoroutine(TutSpawner.Instance.DisplayTutorial(15));
                shop.SellOnInteract();
                return;
            }

            //미니게임이 켜져있으면 미니게임 처리
            else if (player.miniGameResource.isMiniGameOn)
            {
                moveInput = Vector2.zero;
                player.miniGameResource.PressStop();
                return;
            }

            else if (player.fishingResource.IsFishing)
            {
                player.fishingResource.onPressed();
                return;
            }

            //상호작용이 가능하다면 상호작용 실행
            switch (MapControl.Instance.map.IsInteractMinigame((Vector2Int)player.tileReader.FrontCell(), player.tool.CurrentEquip))
            {
                //스테미나를 사용하지 않는 행동
                case MinigameInteractionType.WithoutStaminaConsume:
                    moveInput = Vector2.zero;
                    IsInteract = true;
                    MapControl.Instance.map.OnPlayerInteract((Vector2Int)player.tileReader.FrontCell(), player.tool.CurrentEquip);
                    break;
                //상호작용 행동 없음
                case MinigameInteractionType.None:
                    if((player.tool.CurrentEquip.equipmentType == EquipmentType.WateringCan || player.tool.CurrentEquip.equipmentType == EquipmentType.SeedBasket)
                        && player.tool.CurrentEquip.equipmentExtra <= 0)
                        failAnimator.ForcePlay();
                    break;
                //스테미나만 사용하는 행동
                case MinigameInteractionType.WithoutMinigame:
                    if (PlayerManager.Instance.playerStamina.Check(StaminaCost))
                    {
                        IsInteract = true;
                        moveInput = Vector2.zero;
                        playerAnimation.MinigameResult = SuccessStatus.WithOutMinigame;

                        if (player.tool.CurrentEquip.equipmentType == EquipmentType.None)
                            playerAnimation.PickingAnim();
                        else
                            playerAnimation.InteractAnim();
                    }
                    else
                        failAnimator.ForcePlay();
                    break;
                //낚시 미니게임 시작
                case MinigameInteractionType.Fishing:
                    if (!player.fishingResource.IsFishing && PlayerManager.Instance.playerStamina.Check(StaminaCost))
                    {
                        moveInput = Vector2.zero;
                        player.fishingResource.IsFishing = true;
                        player.fishingResource.Init();
                    }
                    else
                        failAnimator.ForcePlay();
                    break;

                case MinigameInteractionType.Cooking:
                    if (!player.cookingResource.isActive && PlayerManager.Instance.playerStamina.Check(StaminaCost))
                    {
                        moveInput = Vector2.zero;
                        player.cookingResource.StartCooking();
                    }
                    break;

                //QTE미니게임을 사용하는 행동
                default:
                    if (PlayerManager.Instance.playerStamina.Check(StaminaCost))
                    {
                        moveInput = Vector2.zero;
                        player.miniGameResource.PressOn();
                    }
                    else
                        failAnimator.ForcePlay();
                    break;
            }
        }
    }

    public void OnQuickSlot(InputAction.CallbackContext context)
    {
        if (!CanInput()) return;
        if (!context.started) return;

        int slot = ResolveQuickslotFromContext(context);
        if (slot < 1 || slot > 7) return;

        HandleQuickslot(slot);
    }

    public void OnItemQuickSlot(InputAction.CallbackContext context)
    {
        
        
        if (!CanInput()) return;
        if (!context.started) return;

        int slot = ResolveQuickslotFromContext(context);
        if (slot==10||slot==8||slot==9)
        {
            HandleItemQuickSlot(slot);
        }

        
    }

    public void OnInventoryToggle(InputAction.CallbackContext context)
    {
        if (!CanInput()) return;

        if (!context.started) return;

        UIManager.Instance.ToggleUI<UIMenu>();
    }

    private int ResolveQuickslotFromContext(InputAction.CallbackContext context)
    {
        var action = context.action;
        int binding = action?.GetBindingIndexForControl(context.control) ?? -1;
        if (binding >= 0 && binding < action.bindings.Count)
        {
            var number = action.bindings[binding].name;
            if (int.TryParse(number, out int slotByNumber)) return slotByNumber;
        }

        if (context.control is KeyControl key)
        {
            switch (key.keyCode)
            {
                case Key.Digit1: return 1;
                case Key.Digit2: return 2;
                case Key.Digit3: return 3;
                case Key.Digit4: return 4;
                case Key.Digit5: return 5;
                case Key.Digit6: return 6;
                case Key.Digit7: return 7;
                case Key.Digit8: return 8;
                case Key.Digit9: return 9;
                case Key.Digit0: return 10;

                    
            }
        }

        return -1;
    }

    protected virtual void HandleQuickslot(int slot)
    {
        player.tool.SelectQuickslot(slot);
    }

    void HandleItemQuickSlot(int slot)
    {
        PlayerInventory.Instance.SelectSlot(slot);
    }

    /*
    private bool OnCheck()
    {
        if (tileReader == null || moveInput == Vector2.zero)
            return true;

        // 현재 위치
        Vector3Int curCell = tileReader.CurrentCell;

        // 이동할 예정의 타일
        Vector3Int willCell = curCell + new Vector3Int(
            Mathf.RoundToInt(moveInput.x),
            Mathf.RoundToInt(moveInput.y),
            0);

        var map = MapControl.Instance.map;
    }
    */

    private void OnMove()
    {
        if (rigidbody != null)
        {
            Vector2 v = moveInput;
            if (v.sqrMagnitude > 1f) v = v.normalized; // 대각선 이동 구현
            rigidbody.velocity = v * moveSpeed;
        }

        
        // 입력이 있을 때
        if (moveInput.sqrMagnitude > 0.01f)
        {
            
            // 재생 중이 아니면 시작
            if (!audio.isPlaying)
            {
                audio.loop = true; // 반복 재생
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.Footstep);
            }
        }
        else
        {
            // 입력이 없으면 정지
            if (audio.isPlaying)
            {
                return;
            }
        }
        
    }

    private void UpdateFacingFromInput()
    {
        if (!tileReader) return;

        // 입력이 없으면 마지막 방향 유지
        if (moveInput.sqrMagnitude == 0) return;

        TileReader.Facing facing;
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            facing = moveInput.x > 0 ? TileReader.Facing.Right : TileReader.Facing.Left;
        else
            facing = moveInput.y > 0 ? TileReader.Facing.Up : TileReader.Facing.Down;

        tileReader.SetFacing(facing);
        if (previousLook != tileReader.FrontCell())
        {
            previousLook = tileReader.FrontCell();
            player.interactChecker.OnChange(player.tool.CurrentEquip);
        }
    }

    public bool CanInput()
    {
        if (IsInteract || SceneChangeManager.Instance.IsSceneChange || NPCManager.Instance.IsInteracting 
            || player.miniGameResource.isMiniGameOn || player.fishingResource.IsFishing || player.cookingResource.isActive
            || TutSpawner.Instance.isTutorOpen)
        {
            moveInput = Vector2.zero;
            return false;
        }
        return true;
    }

}
