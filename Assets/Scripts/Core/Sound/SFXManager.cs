using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : Singleton<SFXManager>
{
    public enum SFXType
    { 
        Click,
        Footstep,
        Success,
        Fail,
        Dialogue,
        FishingReel,
        ThrowingFishingBait,
        Splashing,
        HitWood,
        FallingTree,
        HitSton,
        BrokenSton,
        PumpWater,
        SprayingWater,
        PickUp,
        Absorb
    }

    
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip footStep;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip dialogueSound;
    [SerializeField] private AudioClip fishingReelSound;
    [SerializeField] private AudioClip throwingFishingBaitSound;
    [SerializeField] private AudioClip splashingSound;
    [SerializeField] private AudioClip hitWoodSound;
    [SerializeField] private AudioClip fallingTreeSound;
    [SerializeField] private AudioClip hitStonSound;
    [SerializeField] private AudioClip brokenStonSound;
    [SerializeField] private AudioClip pumpWaterSound;
    [SerializeField] private AudioClip sprayingWaterSound;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip absorbSound;

    [SerializeField] private AudioMixerGroup sfxGroup;

    private AudioSource src;
    private bool initialized;

    private void OnEnable()
    {
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (initialized)
        {
            return;
        }

        src = GetComponent<AudioSource>();
        if (sfxGroup)
        { 
            src.outputAudioMixerGroup = sfxGroup;
        }

        src.playOnAwake = false;
        src.loop = false;
        initialized = true;
    }

    public void PlaySFX(SFXType type)
    {
        TryInitialize();

        AudioClip clip = null;
        switch (type)
        {
            case SFXType.Click:
                clip = clickSound;
                break;

            case SFXType.Footstep:
                clip = footStep;
                break;

            case SFXType.Success:
                clip = successSound;
                break;

            case SFXType.Fail:
                clip = failSound;
                break;

            case SFXType.Dialogue:
                clip = dialogueSound;
                break;

            case SFXType.FishingReel:
                clip = fishingReelSound;
                break;

            case SFXType.ThrowingFishingBait:
                clip = throwingFishingBaitSound;
                break;

            case SFXType.Splashing:
                clip = splashingSound;
                break;

            case SFXType.HitWood:
                clip = hitWoodSound;
                break;

            case SFXType.FallingTree:
                clip = fallingTreeSound;
                break;

            case SFXType.HitSton:
                clip = hitStonSound;
                break;

            case SFXType.BrokenSton:
                clip = brokenStonSound;
                break;

            case SFXType.PumpWater:
                clip = pumpWaterSound;
                break;

            case SFXType.SprayingWater:
                clip = sprayingWaterSound;
                break;

            case SFXType.PickUp:
                clip = pickUpSound;
                break;

            case SFXType.Absorb:
                clip = absorbSound;
                break;

            default:
                clip = null;
                break;

        }

        if (clip == null)
        {
            return;
        }

        src.PlayOneShot(clip);
    }

    public void StopSFX(SFXType type)
    {
        if (type == SFXType.Dialogue && src.isPlaying)
        {
            src.Stop();
        }
    }


}
