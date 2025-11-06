using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void OnGameQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#else
        Application.Quit();
#endif
    }
}
