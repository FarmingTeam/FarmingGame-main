using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Transitor : MonoBehaviour
{
    [SerializeField] private Image transitor;
    [SerializeField] private LoadingText loadingText;
    [SerializeField] private GameObject ClickBlocker;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // 키고 끄는데 걸리는 시간(초)
    [SerializeField] private bool useUnscaledTime = true; // TimeScale 무시 여부

    public bool isTransitioning { get; private set; }

    public void TransitStart()
    {
        Debug.Log("트랜지션 시작");
        ClickBlocker.SetActive(true);
        if (SceneChangeManager.Instance.SCENENAMEDICT[SceneChangeManager.Instance.currentScene].IsMap)
        {
            TimeManager.Instance.PauseTime(true);
        }
        StartCoroutine(FadeInAndActiveText());
    }

    public void TransitDone()
    {
        Debug.Log("트랜지션 완료");
        ClickBlocker.SetActive(false);
        StartCoroutine(FadeOutClear());
    }

    private IEnumerator FadeInAndActiveText()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;
        yield return FadeAlpha(0f, 1f, fadeDuration);

        loadingText.ActiveTxt();

        isTransitioning = false;

        yield return new WaitForSeconds(1);

        loadingText.CloseText();
    }

    private IEnumerator FadeOutClear()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        yield return FadeAlpha(1f, 0f, fadeDuration);
        if (SceneChangeManager.Instance.SCENENAMEDICT[SceneChangeManager.Instance.currentScene].IsMap && !TutSpawner.Instance.isTutorOpen)
        {
            TimeManager.Instance.PauseTime(false);
        }

        isTransitioning = false;
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        if (transitor == null) yield break;

        float t = 0f;
        Color c = transitor.color;

        // 시작점 보정
        c.a = from;
        transitor.color = c;

        while (t < duration)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;
            float lerp = Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(from, to, lerp);
            transitor.color = c;
            yield return null;
        }

        c.a = to;
        transitor.color = c;
    }
}

