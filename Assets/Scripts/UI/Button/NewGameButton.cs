
using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] Canvas NameInputCanvas;
    public void OnNewGameButtonClicked()
    {
        NameInputCanvas.gameObject.SetActive(true);
    }
}
