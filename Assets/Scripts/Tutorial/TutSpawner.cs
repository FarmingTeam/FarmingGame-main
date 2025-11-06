using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutSpawner : Singleton<TutSpawner>
{
    [SerializeField] private GameObject interactTut1;
    [SerializeField] private GameObject interactTut2;
    [SerializeField] private GameObject farmingTut;
    [SerializeField] private GameObject qTETut;
    [SerializeField] private GameObject inventoryTut;
    [SerializeField] private GameObject wateringTut;
    [SerializeField] private GameObject fishingTut;
    [SerializeField] private GameObject shopTut;
    [SerializeField] private GameObject quickSlotTut;
    [SerializeField] private GameObject stamina1Tut;
    [SerializeField] private GameObject stamina2Tut;
    [SerializeField] private GameObject achievementTut;
    [SerializeField] private GameObject systemTut;
    [SerializeField] private GameObject mapTut;
    [SerializeField] private GameObject sellBoxTut;
    [SerializeField] private GameObject runTut;
    [SerializeField] private GameObject cookTut;
    [SerializeField] private GameObject informationTut;
    [SerializeField] private GameObject oClockTut;

    private PlayerInput playerInput;

    private bool interact1Shown = false;
    private bool interact2Shown = false;
    private bool farmingShown = false;
    private bool qTEShown = false;
    private bool inventoryShown = false;
    private bool wateringShown = false;
    private bool fishingShown = false;
    private bool shopShown = false;
    private bool quickSlotShown = false;
    private bool stamina1Shown = false;
    private bool stamina2Shown = false;
    private bool achievementShown = false;
    private bool systemShown = false;
    private bool mapShown = false;
    private bool sellBoxShown = false;
    private bool runShown = false;
    private bool cookShown = false;
    private bool informationShown = false;
    private bool oClockShown = false;

    public bool isTutorOpen = false;

    private void Start()
    {
        // 불러오기
        interact1Shown = PlayerPrefs.GetInt("interact1Shown", 0) == 1;
        interact2Shown = PlayerPrefs.GetInt("interact2Shown", 0) == 1;
        farmingShown = PlayerPrefs.GetInt("farmingShown", 0) == 1;
        qTEShown = PlayerPrefs.GetInt("qTEShown", 0) == 1;
        inventoryShown = PlayerPrefs.GetInt("inventoryShown", 0) == 1;
        wateringShown = PlayerPrefs.GetInt("wateringShown", 0) == 1;
        fishingShown = PlayerPrefs.GetInt("fishingShown", 0) == 1;
        shopShown = PlayerPrefs.GetInt("shopShown", 0) == 1;
        quickSlotShown = PlayerPrefs.GetInt("quickSlot", 0) == 1;
        stamina1Shown = PlayerPrefs.GetInt("stamina1", 0) == 1;
        stamina2Shown = PlayerPrefs.GetInt("stamina2", 0) == 1;
        achievementShown = PlayerPrefs.GetInt("achievement", 0) == 1;
        systemShown = PlayerPrefs.GetInt("system", 0) == 1;
        mapShown = PlayerPrefs.GetInt("map", 0) == 1;
        sellBoxShown = PlayerPrefs.GetInt("sellBox", 0) == 1;
        runShown = PlayerPrefs.GetInt("run", 0) == 1;
        cookShown = PlayerPrefs.GetInt("cook", 0) == 1;
        informationShown = PlayerPrefs.GetInt("information", 0) == 1;
        oClockShown = PlayerPrefs.GetInt("oClock", 0) == 1;
    }

    public void SaveTutorialStates()
    {
        // 저장하기
        PlayerPrefs.SetInt("interact1Shown", interact1Shown ? 1 : 0);
        PlayerPrefs.SetInt("interact2Shown", interact2Shown ? 1 : 0);
        PlayerPrefs.SetInt("farmingShown", farmingShown ? 1 : 0);
        PlayerPrefs.SetInt("qTEShown", qTEShown ? 1 : 0);
        PlayerPrefs.SetInt("inventoryShown", inventoryShown ? 1 : 0);
        PlayerPrefs.SetInt("wateringShown", wateringShown ? 1 : 0);
        PlayerPrefs.SetInt("fishingShown", fishingShown ? 1 : 0);
        PlayerPrefs.SetInt("shopShown", shopShown ? 1 : 0);
        PlayerPrefs.SetInt("quickSlot", quickSlotShown ? 1 : 0);
        PlayerPrefs.SetInt("stamina1", stamina1Shown ? 1 : 0);
        PlayerPrefs.SetInt("stamina2", stamina2Shown ? 1 : 0);
        PlayerPrefs.SetInt("achievement", achievementShown ? 1 : 0);
        PlayerPrefs.SetInt("system", systemShown ? 1 : 0);
        PlayerPrefs.SetInt("map", mapShown ? 1 : 0);
        PlayerPrefs.SetInt("sellBox", sellBoxShown ? 1 : 0);
        PlayerPrefs.SetInt("run", runShown ? 1 : 0);
        PlayerPrefs.SetInt("cook", cookShown ? 1 : 0);
        PlayerPrefs.SetInt("information", informationShown ? 1 : 0);
        PlayerPrefs.SetInt("oClock", oClockShown ? 1 : 0);

        PlayerPrefs.Save(); // 즉시 저장
    }

    public IEnumerator DisplayTutorial(int tutornumber)
    {
        while (TransitController.Instance.transitor.isTransitioning)
        {
            yield return null;
        }
        switch (tutornumber)
        {
            case 1:
                Interact1Tutor(); break;
            case 2:
                Interact2Tutor(); break;
            case 3:
                FarmingTutor(); break;
            case 4:
                QTETutor(); break;
            case 5:
                InventoryTutor(); break;
            case 6:
                WateringTutor(); break;
            case 7:
                FishingTutor(); break;
            case 8:
                ShopTutor(); break;
            case 9:
                QuickSlotTutor(); break;
            case 10:
                Stamina1Tutor(); break;
            case 11:
                Stamina2Tutor(); break;
            case 12:
                AchivementTutor(); break;
            case 13:
                SystemTutor(); break;
            case 14:
                MapTutor(); break;
            case 15:
                SellBoxTutor(); break;
            case 16:
                RunTutor(); break;
            case 17:
                CookTutor(); break;
            case 18:
                InfoTutor(); break;
            case 19:
                OClockTutor(); break;
            default:
                throw new System.Exception("Undefined Tutorial Key");
        }
    }


    public void Interact1Tutor(bool forceOpen = false)
    {
        if (!interact1Shown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(interactTut1);
            interact1Shown = true;
            isTutorOpen = true;
            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void Interact2Tutor(bool forceOpen = false)
    {
        if (!interact2Shown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(interactTut2);
            interact2Shown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void FarmingTutor(bool forceOpen = false)
    {
        if (!farmingShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(farmingTut);
            farmingShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void QTETutor(bool forceOpen = false)
    {
        if (!qTEShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(qTETut);
            qTEShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void InventoryTutor(bool forceOpen = false)
    {
        if (!inventoryShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(inventoryTut);
            inventoryShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void WateringTutor(bool forceOpen = false)
    {
        if (!wateringShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(wateringTut);
            wateringShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void FishingTutor(bool forceOpen = false)
    {
        if (!fishingShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(fishingTut);
            fishingShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void ShopTutor(bool forceOpen = false)
    {
        if (!shopShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(shopTut);
            shopShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void QuickSlotTutor(bool forceOpen = false)
    {
        if (!quickSlotShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(quickSlotTut);
            quickSlotShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void Stamina1Tutor(bool forceOpen = false)
    {
        if (!stamina1Shown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(stamina1Tut);
            stamina1Shown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void Stamina2Tutor(bool forceOpen = false)
    {
        if (!stamina2Shown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(stamina2Tut);
            stamina2Shown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void AchivementTutor(bool forceOpen = false)
    {
        if (!achievementShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(achievementTut);
            achievementShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void SystemTutor(bool forceOpen = false)
    {
        if (!systemShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(systemTut);
            systemShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void MapTutor(bool forceOpen = false)
    {
        if (!mapShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(mapTut);
            mapShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void SellBoxTutor(bool forceOpen = false)
    {
        if (!sellBoxShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(sellBoxTut);
            sellBoxShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void RunTutor(bool forceOpen = false)
    {
        if (!runShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(runTut);
            runShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void CookTutor(bool forceOpen = false)
    {
        if (!cookShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(cookTut);
            cookShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void InfoTutor(bool forceOpen = false)
    {
        if (!informationShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(informationTut);
            informationShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void OClockTutor(bool forceOpen = false)
    {
        if (!oClockShown || forceOpen)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            playerInput.enabled = false;
            GameObject tutorial = Instantiate(oClockTut);
            oClockShown = true;
            isTutorOpen = true;

            TimeManager.Instance.PauseTime(true);
        }
        else
            return;

        SaveTutorialStates();
    }

    public void ResetTutor()
    {
        interact1Shown = false;
        interact2Shown = false;
        farmingShown = false;
        qTEShown = false;
        inventoryShown = false;
        wateringShown = false;
        fishingShown = false;
        shopShown = false;
        quickSlotShown = false;
        stamina1Shown = false;
        stamina2Shown = false;
        achievementShown = false;
        systemShown = false;
        mapShown = false;
        sellBoxShown = false;
        runShown = false;
        cookShown = false;
        informationShown = false;
        oClockShown = false;

        SaveTutorialStates();
    }

    public void InputRelease()
    {
        playerInput.enabled = true;
    }
}
