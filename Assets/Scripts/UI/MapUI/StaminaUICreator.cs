
using UnityEngine;

public class StaminaUICreator: MonoBehaviour
{
    [SerializeField] private GameObject staminaUIPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private PlayerStamina playerStamina;

    private GameObject instance;

    private void Start()
    {
        if (staminaUIPrefab && parent)
        {
            instance = Instantiate(staminaUIPrefab, parent);
            instance.name = "Stamina UI";
        }
        else
        {
            return;
        }
        StaminaUI ui = instance.GetComponent<StaminaUI>();
        if (ui == null)
            return;

        playerStamina = PlayerManager.Instance.playerStamina;
        if (playerStamina == null)
            return;

        playerStamina.SetUI(ui); 
    }

}
