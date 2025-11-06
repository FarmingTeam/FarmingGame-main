
using UnityEngine;

public class FailAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    const string PLAYKEY = "IsPlay";


    public void ForcePlay()
    {
        animator.ResetTrigger(PLAYKEY);
        animator.SetTrigger(PLAYKEY);
    }

}
