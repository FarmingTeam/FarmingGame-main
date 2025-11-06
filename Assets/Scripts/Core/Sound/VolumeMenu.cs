using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISlider = UnityEngine.UI.Slider;


public class VolumeMenu : MonoBehaviour
{
    [SerializeField] private UISlider masterslider;
    [SerializeField] private UISlider bgmslider;
    [SerializeField] private UISlider sfxslider;

    private void OnEnable()
    {
        var sm = SoundManager.Instance;
        if (sm == null)
        {
            return;
        }

        masterslider.SetValueWithoutNotify(sm.GetMasterVolume());
        bgmslider.SetValueWithoutNotify(sm.GetBGMVolume());
        sfxslider.SetValueWithoutNotify(sm.GetSFXVolume());

        masterslider.onValueChanged.AddListener(sm.SetMasterVolume);
        bgmslider.onValueChanged.AddListener(sm.SetMusicVolume);
        sfxslider.onValueChanged.AddListener(sm.SetSFXVolume);
    }

    private void OnDisable()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        masterslider.onValueChanged.RemoveAllListeners();
        bgmslider.onValueChanged.RemoveAllListeners();
        sfxslider.onValueChanged.RemoveAllListeners();

    }
}
