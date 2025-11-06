using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioMixer mixer;

    private const string MASTER = "Master";
    private const string BGM = "BGM";
    private const string SFX = "SFX";

    protected override void Awake()
    {
        base.Awake();
        ApplySavedVolumes();
    }

    private void Start()
    {
        ApplySavedVolumes();
    }

    private void OnEnable()
    {
        ApplySavedVolumes();
    }

    private float ToDecibel(float v) => Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;

    private void ApplySavedVolumes()
    {
        mixer.SetFloat(MASTER, ToDecibel(PlayerPrefs.GetFloat(MASTER, 1f)));
        mixer.SetFloat(BGM, ToDecibel(PlayerPrefs.GetFloat(BGM, 1f)));
        mixer.SetFloat(SFX, ToDecibel(PlayerPrefs.GetFloat(SFX, 1f)));
    }

    public void SetMasterVolume(float v)
    {
        mixer.SetFloat(MASTER, ToDecibel(v));
        PlayerPrefs.SetFloat(MASTER, v);
    }

    public void SetMusicVolume(float v)
    {
        mixer.SetFloat(BGM, ToDecibel(v));
        PlayerPrefs.SetFloat(BGM, v);
    }
    public void SetSFXVolume(float v)
    {
        mixer.SetFloat(SFX, ToDecibel(v));
        PlayerPrefs.SetFloat(SFX, v);
    }

    public float GetMasterVolume()
    {
        float value = PlayerPrefs.GetFloat(MASTER, 1f);
        return value;
    }

    public float GetBGMVolume()
    {
        float value = PlayerPrefs.GetFloat(BGM, 1f);
        return value;
    }
    public float GetSFXVolume()
    {
        float value = PlayerPrefs.GetFloat(SFX, 1f);
        return value;
    }

}
