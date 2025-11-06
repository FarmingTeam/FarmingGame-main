using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text valueText;

    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color lowTextColor = Color.black;

    [SerializeField] Color normalImageColor;
    [SerializeField] Color midImageColor;
    [SerializeField] Color lowImageColor;


    public void Refresh(float current, float max)
    {
        if (fill == null)
        {
            return;
        }

        float ratio;

        if (max > 0f)
        {
            ratio = current / max;
        }

        else 
        {
            ratio = 0f;
        }

        fill.fillAmount = Mathf.Clamp01(ratio);


        if (fill.fillAmount <= 0.1f)
        {
            fill.color = lowImageColor;
        }
        else if (fill.fillAmount <= 0.5f)
        {
            fill.color = midImageColor;
        }
        else
        {
            fill.color = normalImageColor;
        }

        if (valueText != null)
        {
            valueText.text = Mathf.RoundToInt(current).ToString();

            if (ratio < 0.5f)
            {
                valueText.color = lowTextColor;
            }
            else 
            {
                valueText.color = normalTextColor;
            }
        }
    }
}
