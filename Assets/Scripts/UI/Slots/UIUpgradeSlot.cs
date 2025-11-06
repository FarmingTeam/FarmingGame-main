using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUpgradeSlot : MonoBehaviour,IPointerClickHandler
{
    public Upgrade slotEquip;
    public Image ItemImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI needEquipmentText;
    public Outline outline;

    UIUpgrade uIUpgrade;
    public void SetUpgradeSlot(Upgrade upgrade)
    {
        slotEquip = upgrade;
        if(upgrade == null)
        {
            needEquipmentText.gameObject.SetActive(true);
            needEquipmentText.SetText(" NPC에게서 아이템 획득 필요");
        }
        else
        {
            needEquipmentText.gameObject.SetActive(false);
            var tool = PlayerEquipment.Instance.GetPlayerEquipment(upgrade.toolId);
            if(tool!=null)
            {
                ItemImage.gameObject.SetActive(true);
                nameText.gameObject.SetActive(true);
                moneyText.gameObject.SetActive(true);
                ItemImage.sprite = tool.equipmentIcon;
                nameText.SetText($"{tool.equipmentName} lv.{tool.equipmentUpgrade+1}");
                if(tool.equipmentUpgrade==0)
                {
                    moneyText.SetText($"필요 금액:{upgrade.requireGold}");
                }
                else if(tool.equipmentUpgrade==1)
                {
                    moneyText.SetText($"필요 금액:{upgrade.nextrequireGold}");
                }
                else if (tool.equipmentUpgrade==2)
                {
                    needEquipmentText.gameObject.SetActive(true);
                    ItemImage.gameObject.SetActive(false);
                    nameText.gameObject.SetActive(false);
                    moneyText.gameObject.SetActive(false);
                    needEquipmentText.SetText("업그레이드 완료");
                }
                
            }
            else
            {
                needEquipmentText.gameObject.SetActive(true);
                ItemImage.gameObject.SetActive(false);
                nameText.gameObject.SetActive(false);
                moneyText.gameObject.SetActive(false);
            }
            
        }
            
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(uIUpgrade==null)
        {
           uIUpgrade = GetComponentInParent<UIUpgrade>();
        }
        uIUpgrade.SelectSlot(this);
       
    }
}
