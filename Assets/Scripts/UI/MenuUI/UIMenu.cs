
using UnityEngine;

public class UIMenu : UIPopup
{
    private void OnEnable()
    {
        UIManager.Instance.currentUISwitch.isOpen = true;
        TimeManager.Instance.PauseTime(true);
    }

    private void OnDisable()
    {
        UIManager.Instance.currentUISwitch.isOpen = false;
        if (!TutSpawner.Instance.isTutorOpen)
            TimeManager.Instance.PauseTime(false);
    }
}
