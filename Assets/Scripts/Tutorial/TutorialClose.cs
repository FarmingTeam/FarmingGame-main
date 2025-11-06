
using UnityEngine;

public class TutorialClose : MonoBehaviour
{
    [SerializeField] private GameObject TutWindow;

    public void CloseTutorial()
    {
        //Refactor : 튜토리얼 분리해서 보이게 처리
        /*
        if (TutWindow.name.Contains("Farming"))
        {
            StartCoroutine(TutSpawner.Instance.DisplayTutorial(10));
        }
        */

        TutSpawner.Instance.InputRelease();
        TutSpawner.Instance.isTutorOpen = false;
        TimeManager.Instance.PauseTime(false);
        MapControl.Instance.player.controller.IsInteract = false;
        Destroy(TutWindow);
    }
}
