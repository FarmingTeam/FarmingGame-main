using System.Collections;
using UnityEngine;

public enum SuccessStatus
{
    None,
    Fail,
    Good,
    VeryGood,
    WithOutMinigame

}

public class MiniGameResource : MonoBehaviour
{
    //캐릭터한테 일단 월드 캔버스 달기
    [Header("설정")]
    [SerializeField] float speed = 3f;
    const float half = 0.5f;
    float midpoint = 0.0f;
    public bool isMiniGameOn = false;
    [HideInInspector] Coroutine currentCorutine = null;
    [SerializeField] Player player;
    [SerializeField] PlayerAnimation playerAnimation;


    [Header("실패")]
    [SerializeField] RectTransform barRect;

    [Header("성공")]
    [SerializeField] RectTransform goodRect;
    [SerializeField] float goodRectRate;

    [Header("대성공")]
    [SerializeField] RectTransform perfectRect;
    [SerializeField] float perfectRectRate;

    [Header("피봇")]
    [SerializeField] RectTransform movingNeedle;
    [SerializeField] float needleWidth;
    float needleX;

    public void PressOn()
    {
        gameObject.SetActive(true);
        if(currentCorutine != null)
            return;
        SetRectByPercent();
        isMiniGameOn = true;
        StartCoroutine(TutSpawner.Instance.DisplayTutorial(4));
        currentCorutine = StartCoroutine(StartMiniGame());
    }

    public void Init()
    {
        isMiniGameOn = false;

        if (currentCorutine != null)
        {
            StopCoroutine(currentCorutine);
            currentCorutine = null;
        }
        gameObject.SetActive (false);
    }

    public void PressStop()
    {
        isMiniGameOn = false;
        if (currentCorutine != null)
        {
            StopCoroutine(currentCorutine);
            currentCorutine = null;
        }
        playerAnimation.MinigameResult = CheckSuccessStatus();



        var result = playerAnimation.MinigameResult;
        if (player.tool.CurrentEquip.equipmentType == EquipmentType.None)
        {
            if (result == SuccessStatus.VeryGood || result == SuccessStatus.Good)
                playerAnimation.PickingAnim();
            else
                playerAnimation.AtEndInteraction();
        }
        else
        {
            if (result == SuccessStatus.VeryGood || result == SuccessStatus.Good)
                playerAnimation.InteractAnim();
            else
                playerAnimation.AtEndInteraction();

        }
            
        gameObject.SetActive(false);
    }

    //Refactor : 나중에 각 도구에 따른 설정 범위 변수
    void SetRectByPercent()
    {
        midpoint = Random.Range(goodRectRate / 2.0f, 1.0f - goodRectRate / 2.0f);

        goodRect.anchorMin = new Vector2(AnchorCalculator(goodRectRate, true), 0);
        goodRect.anchorMax = new Vector2(AnchorCalculator(goodRectRate, false), 1);

        perfectRect.anchorMin = new Vector2(AnchorCalculator(perfectRectRate, true), 0);
        perfectRect.anchorMax = new Vector2(AnchorCalculator(perfectRectRate, false), 1);

        movingNeedle.sizeDelta = new Vector2(needleWidth, movingNeedle.sizeDelta.y);
    }

    float AnchorCalculator(float percent, bool isLeft)
    {
        if (isLeft)
        {
            return midpoint - percent / 2.0f;
        }
        else
        {
            return midpoint + percent / 2.0f;
        }
    }

    bool SuccessCalculator(float percent)
    {
        float left = AnchorCalculator(percent, true);
        float right = AnchorCalculator(percent, false);

        //판정로직 : 중심점 기준으로만 판단
        if (left <= needleX + half && needleX + half <= right)
            return true;

        return false;
    }


    IEnumerator StartMiniGame()
    {
        float time = 0f;
        float width = barRect.rect.width;
        while (isMiniGameOn)
        {
            time += Time.deltaTime * speed;
            needleX = Mathf.PingPong(time, 1.0f) - half;
            movingNeedle.localPosition = new Vector3(needleX * width, 0, 0);
            yield return null;
        }
    }

    SuccessStatus CheckSuccessStatus()
    {
        //만약 멈춘 범위가 verygood미면 
        if(SuccessCalculator(perfectRectRate))
        {
            Debug.Log("대성공");
            return SuccessStatus.VeryGood;
        }
        else if(SuccessCalculator(goodRectRate))
        {
            Debug.Log("성공");
            return SuccessStatus.Good;
        }
        else
        {
            Debug.Log("실패");
            return SuccessStatus.Fail;
        }
    }


}
