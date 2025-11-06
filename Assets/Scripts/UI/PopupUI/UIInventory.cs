
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : UIBase
{


    public List<UISlot> uISlots = new List<UISlot>();

    public GameObject uiSlotPrefab;
    [SerializeField] Button sortButton;
    [SerializeField] GameObject DescriptionPanelPrefab;
    public GameObject DescriptionPanel;

    [SerializeField] GameObject leftPanel;
    [SerializeField] GameObject rightPanel;


    
    // Start is called before the first frame update
    void Start()
    {

        //맵컨트롤 참조
        if(DescriptionPanel==null)
        {
            DescriptionPanel = Instantiate(DescriptionPanelPrefab, transform, false);
            DescriptionPanel.SetActive(false);
        }
       
        MakeSlots();
        

        
        
    }

    private void Update()
    {
        if (uiSlotPrefab == isActiveAndEnabled)
        {
            StartCoroutine(TutSpawner.Instance.DisplayTutorial(9));
        }
    }


    void MakeSlots()
    {
        if (uISlots == null || uISlots.Count == 0)
        {
            int inventoryHalfNum = PlayerInventory.Instance.InventoryMaxNum / 2;
            for (int i = 0; i < inventoryHalfNum; i++)
            {
                var go = Instantiate(uiSlotPrefab, leftPanel.transform, false);
                var slot = go.GetComponent<UISlot>();
                uISlots.Add(slot);
                uISlots[i].slotIndex = i;
            }

            for (int i = inventoryHalfNum; i < PlayerInventory.Instance.InventoryMaxNum; i++)
            {
                var go = Instantiate(uiSlotPrefab, rightPanel.transform, false);
                var slot = go.GetComponent<UISlot>();
                uISlots.Add(slot);
                uISlots[i].slotIndex = i;
            }
        }
    }



    private void OnEnable()
    {
        base.OnOpen();

        PlayerInventory.Instance.SubscribeOnItemChange(StartSettingItem);

        sortButton.onClick.AddListener(PlayerInventory.Instance.SortInventory);
        MakeSlots() ;
        StartSettingItem();
    }


    private void OnDisable()
    {
        PlayerInventory.Instance.UnsubscribeOnItemChange(StartSettingItem);
        sortButton.onClick.RemoveListener(PlayerInventory.Instance.SortInventory);
    }
    

    public void StartSettingItem()
    {
        for (int i = 0; i < uISlots.Count; i++)
        {

            SetItemsUI(PlayerInventory.Instance.slotDataList[i], i);
        }
    }



    public void SetItemsUI(SlotData slotData,int slotIndex)
    {
        var slot=uISlots[slotIndex];
        slot.SetSlot(slotData);
            
    }

    



}
