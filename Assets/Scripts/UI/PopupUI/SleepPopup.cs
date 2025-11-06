
using UnityEngine;
using UnityEngine.UI;

public class SleepPopup : UIPopup
{
    [SerializeField] private GameObject sleepPopup;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

  
    public void OnclickYesBtn()
    {
        sleepPopup.SetActive(false);
        MapControl.Instance.player.controller.IsInteract = true;
        BGMPlayer.Instance.PlayBGM(BGMPlayer.Instance.sleepBGM);
        TimeManager.Instance.SetTomorrow();

    }
    public void OnclickNoBtn()
    {
        MapControl.Instance.player.controller.IsInteract = false;
        sleepPopup.SetActive(false);
    }

}
