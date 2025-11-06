using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaminaAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI staminaText;

    void Start()
    {
        PlayerManager.Instance.playerStamina.StaminaUseActions += OnStaminaChange;
    }

    public void OnStaminaChange(int value)
    {
        if (value == 0)
            return;
        string s;
        if (value < 0)
            s = value.ToString();
        else
            s = "+" + value.ToString();
        staminaText.text = s;
        animator.SetTrigger("OnStaminaUse");
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.playerStamina.StaminaUseActions -= OnStaminaChange;
    }
}
