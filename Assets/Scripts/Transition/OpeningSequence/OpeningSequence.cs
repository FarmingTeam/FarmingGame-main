using System.Collections;
using UnityEngine;

public class OpeningSequence : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string animationName = "OpeningSequence";
    

    void Start()
    {
        if (animator == null)
            throw new System.Exception("Animator is Empty");
        StartCoroutine(playAnimationWhenReady());
    }

    IEnumerator playAnimationWhenReady()
    {
        while (TransitController.Instance.transitor.isTransitioning)
            yield return null;

        animator.Play(animationName);
    }

    void OnAnimationEnd()
    {
        SaveManager.Instance.OnNewGame();
    }

}
