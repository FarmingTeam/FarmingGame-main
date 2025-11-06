using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteractable : MonoBehaviour
{
    public GameObject shopUIPrefab;
    public GameObject notifyIcon;
    private bool isInteracted = false;
    public float interactDistance = 1.5f;

    private void Update()
    {
        if (!isInteracted)
        {
            // 첫 인터렉션 전: 항상 표시
            if (notifyIcon != null)
                notifyIcon.SetActive(true);
        }
        else
        {
            if (notifyIcon != null)
                notifyIcon.SetActive(false);
        }
    }

    public static ShopInteractable FindInteractableShop(Vector2 interactPos, float distance = 1.5f)
    {
        foreach (var shop in GameObject.FindObjectsOfType<ShopInteractable>())
        {
            if (shop != null && shop.IsInteractable(interactPos))
                return shop;
        }
        return null;
    }

    public bool IsInteractable(Vector2 interactPos)
    {
        float dist = Vector2.Distance(transform.position, interactPos);
        return dist <= interactDistance;
    }

    public void SellOnInteract()
    {
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;

 
        if (playerTransform != null && !IsInteractable(playerTransform.position))
        {
            return; // 거리가 멀면 상점 안 열림
        }
        OpenShop();
        TimeManager.Instance.PauseTime(true);
        Time.timeScale = 0f;
        isInteracted = true;
    }

    private void OpenShop()
    {
        var uiSwitch = FindObjectOfType<UISwitch>();
        if (uiSwitch != null && uiSwitch.inventoryAction != null)
            uiSwitch.inventoryAction.action.Disable();

        var shopUI = FindObjectOfType<ShopUIController>();
        if (shopUI != null)
        {
            
            // 판매 모드만 열기
            shopUI.OpenShopUIWithSellTab("SellBox");
        }
        else
        {
            var canvas = UIManager.Instance.canvas;
            if (shopUIPrefab != null)
            {
                GameObject instance = Instantiate(shopUIPrefab, canvas.transform);
                var shopCtrl = instance.GetComponentInChildren<ShopUIController>(true);
                if (shopCtrl != null)
                    shopCtrl.OpenShopUIWithSellTab("SellBox");
                else
                    Debug.LogError("Prefab에 ShopUIController 컴포넌트 없어 실행 불가");
            }
        }
    }
}
