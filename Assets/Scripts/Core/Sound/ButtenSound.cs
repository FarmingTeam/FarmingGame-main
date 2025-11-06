using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SFXManager;

public class ButtenSound : MonoBehaviour
{
    public Button button;

    private void PlaySound()
    {
        Debug.Log("클릭했습니다");

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(SFXType.Click);
        }
    }

    private void OnEnable()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        
        button.onClick.AddListener(PlaySound);
        
        
    }

    private void OnDisable()
    {

        button.onClick.RemoveListener(PlaySound);

    }

}
