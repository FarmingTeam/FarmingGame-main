
using UnityEngine;
using UnityEngine.EventSystems;

public class StaminaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject currentlyStamina;
    [SerializeField] private RectTransform staminaBG;
    void Start()
    {
        currentlyStamina.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        currentlyStamina.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        currentlyStamina.SetActive(false);
    }
}
