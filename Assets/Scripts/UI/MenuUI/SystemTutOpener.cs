using UnityEngine;

public class SystemTutOpener : MonoBehaviour
{
    public void TutOpen()
    {
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(13));
    }
}
