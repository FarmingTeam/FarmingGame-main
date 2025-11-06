using UnityEngine;
using UnityEngine.InputSystem;

public class CookingDebugModule : MonoBehaviour
{
    [SerializeField] CookingModule cookingModule;


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !cookingModule.isActive)
        {
            cookingModule.StartCooking();
        }
        else if (cookingModule.isPlaying)
        {
            cookingModule.WhileCooking(context);
        }
    }
}
