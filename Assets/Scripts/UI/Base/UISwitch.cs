
using UnityEngine;
using UnityEngine.InputSystem;

public class UISwitch : MonoBehaviour
{

    // 탭키 인풋 따로 참조
    public InputActionReference inventoryAction;
    public InputActionReference escAction;
    public PlayerInput playerInput;

    // 액션맵 생성
    public string uiActionMap = "UI";
    public string playerActionMap = "Player";

    public PlayerController playerController;

    [SerializeField] public UIMenu uiMenu;

    public bool isOpen;
    private InputActionMap _uiMap;
    private InputActionMap _playerMap;



    private void OnEnable()
    {
        //플레이어 컨트롤러 참조
        if (!playerController)
        {
            playerController = MapControl.Instance.player.GetComponent<PlayerController>();
        }
        // 인벤토리 활성화
        if (inventoryAction != null && inventoryAction.action != null)
        {
            if (!inventoryAction.action.enabled)
            {
                inventoryAction.action.Enable();
            }
            
            inventoryAction.action.performed += ActiveInventory;
        }
        if (escAction != null && escAction.action != null)
        {
            if (!escAction.action.enabled)
            {
                escAction.action.Enable();
            }

            escAction.action.performed += CloseTopUI;
        }


    }

    private void OnDisable()
    {
        // 인벤토리 비활성화
        if (inventoryAction != null && inventoryAction.action != null)
        {
            inventoryAction.action.performed -= ActiveInventory;
        }

        if (escAction != null && escAction.action != null)
        {
            escAction.action.performed -= CloseTopUI;
        }
    }

    private void Start()
    {
        if (!playerInput)
        {
            playerInput = MapControl.Instance.player.GetComponent<PlayerInput>();
        }

        if (playerInput && playerInput.actions)
        {
            _uiMap = playerInput.actions.FindActionMap(uiActionMap, false);
            _playerMap = playerInput.actions.FindActionMap(playerActionMap, false);

            if (_uiMap != null && !_uiMap.enabled) _uiMap.Enable();
        }
        EnsureInventoryRef();
    }


    private void EnsureInventoryRef()
    {
        if (uiMenu == null)
        {
            uiMenu = UIManager.Instance.GetUI<UIMenu>();
            uiMenu.gameObject.SetActive(false);
        }
    }

    private void ActiveInventory(InputAction.CallbackContext ctx)
    {
        // 0) 현재 UI를 열 수 있는 상황인지 판단
        if (!playerController.CanInput())
            return;
        // 1) 먼저 레퍼런스 확보
        EnsureInventoryRef();

        // 2) 토글 수행 (UIManager가 있으면 그것 사용, 없으면 직접 토글)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleUI<UIMenu>();
        }
        else if (uiMenu != null)
        {
            uiMenu.gameObject.SetActive(!uiMenu.gameObject.activeSelf);
        }

        // 3) 현재 열림 상태 갱신
        if (uiMenu != null)
            isOpen = uiMenu.gameObject.activeInHierarchy;
        else
            isOpen = false; // 못 찾으면 안전하게 닫힌 걸로 처리

        // 4) 플레이어 액션맵 토글
        if (_playerMap != null)
        {
            if (isOpen) _playerMap.Disable();
            else _playerMap.Enable();
        }

        // 5) UI 액션맵은 항상 켜두기(혹시 꺼져있으면 복구)
        if (_uiMap != null && !_uiMap.enabled) _uiMap.Enable();

        // 6) 플레이어 스크립트 동작/정지
        if (playerController)
        {
            if (isOpen)
            {
                // 이동 정지
                if (playerController.rigidbody) playerController.rigidbody.velocity = Vector2.zero;
                playerController.enabled = false;
            }
            else
            {
                playerController.enabled = true;
            }
        }
    }

    // Inventory 인풋을 받아서 인벤토리 창을 끄고 킵니다.
    // 인벤토리 창이 켜져있는 동안 플레이어의 동작은 멈춥니다.



    private void CloseTopUI(InputAction.CallbackContext callbackContext)
    {
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(16));
        UIManager.Instance.CloseTopPopUpUI();

        _playerMap.Enable();
        playerController.enabled = true;
    }
}


