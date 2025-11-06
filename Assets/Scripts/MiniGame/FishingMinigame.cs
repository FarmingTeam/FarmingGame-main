
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private RectTransform targetZone;
    [SerializeField] private RectTransform gameBar;
    [SerializeField] private RectTransform playerBox;
    [SerializeField] private Image successGauage;
    [SerializeField] private PlayerAnimation playerAnimation;

    [SerializeField] private float targetHoldTime = 1f;

    [SerializeField] private float speed = 200f;
    [SerializeField] private float returnSpeed = 200f;

    [SerializeField] private float timeLimit = 5f;

    private float startTime;
    private float targetCenter;
    private float targetSpeed = 0.0f;

    [SerializeField] private float RandomMoveDelay = 0.5f;
    private float RandomMoveTime = 0.0f;

    [SerializeField] private float targetZoneSpeed = 20f;
    [SerializeField] private float offcenterv = 0.05f;

    private float direction = 1f;

    private float x;
    private float v;
    private float barWidth;
    private float maxX;

    float holdTimerCount;
    int tickcount;

    private readonly int[] FISHINGRESULT = new int[] { 1101, 1102, 1103, 1104, 1105, 1106 };

    public bool IsFishing = false;

    public void Init()
    {
        InitializeState();
        gameObject.SetActive(true);
        enabled = true;
        IsFishing = true;
        startTime = Time.time;
    }


    private void InitializeState()
    {
        barWidth = gameBar.rect.width;
        maxX = Mathf.Max(0f, barWidth - playerBox.rect.width);
        successGauage.fillAmount = 0.0f;

        holdTimerCount = 0f;
        tickcount = 0;

        x = 0f;
        v = 0f;
        ProgressBar(x);
        RandomTargetPosition();
    }


    private void Update()
    {
        //Refactor : Remove
        if (Input.GetKeyDown(KeyCode.Space))
            onPressed();

        v -= returnSpeed * Time.deltaTime;

        x += v * Time.deltaTime;

        x = Mathf.Clamp(x, 0f, maxX);

        ProgressBar(x);

        if (IsInsideTarget())
        {
            holdTimerCount += Time.deltaTime;
            successGauage.fillAmount = Mathf.Min(holdTimerCount / targetHoldTime, 1.0f);

            if (holdTimerCount >= targetHoldTime)
            {
                EndGame(true);
            }
        }
        else
        {
            holdTimerCount = Mathf.Max(0.0f, holdTimerCount - Time.deltaTime);
            successGauage.fillAmount = Mathf.Max(holdTimerCount / targetHoldTime, 0.0f);
        }

        if (Time.time - startTime >= timeLimit)
        {
            EndGame(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopGame();
        }
        if (RandomMoveTime >= RandomMoveDelay)
        {
            SelecteTargetMove();
            RandomMoveTime = 0.0f;
        }
        RandomMoveTime += Time.deltaTime;
        MoveTargetZone();

    }

    private void SelecteTargetMove()
    {
        float barW = gameBar.rect.width;
        float targetW = targetZone.rect.width;

        float leftmost = Mathf.Max(targetCenter - targetW / 2.0f, 0.0f);
        float rightmost = Mathf.Min(targetCenter + targetW / 2.0f, barW);

        float targetX = UnityEngine.Random.Range(leftmost, rightmost);

        float curpos = targetZone.anchoredPosition.x;

        targetSpeed = (targetX - curpos) / RandomMoveDelay;
    }

    private void MoveTargetZone()
    {
        Vector2 pos = targetZone.anchoredPosition;
        pos.x += targetSpeed * Time.deltaTime;
        targetZone.anchoredPosition = pos;
    }

    private void StopGame()
    {
        Debug.Log("미니게임 실행 취소");
       
        gameObject.SetActive(false);

        enabled = false;

        IsFishing = false;
    }


    private void ProgressBar(float barLocalX)
    {
        var pos = playerBox.anchoredPosition;
        pos.x = barLocalX;
        pos.y = 0f;
        playerBox.anchoredPosition = pos;
    }

    private void RandomTargetPosition()
    {
        float barW = gameBar.rect.width;
        float targetW = targetZone.rect.width;

        float minpos = targetW;
        float maxpos = barW - targetW;

        targetCenter = UnityEngine.Random.Range(minpos, maxpos);

        Vector2 pos = targetZone.anchoredPosition;
        pos.x = targetCenter;
        targetZone.anchoredPosition = pos;
        SelecteTargetMove();
    }

    private bool IsInsideTarget()
    {
        float cursorCenterX = playerBox.TransformPoint(playerBox.rect.center).x;

        Vector3[] corners = new Vector3[4];
        targetZone.GetWorldCorners(corners);
        float leftx = corners[0].x;
        float rightx = corners[2].x;

        return cursorCenterX >= leftx && cursorCenterX <= rightx;
    }


    void EndGame(bool success)
    {
        if (success)
        {
            SFXManager.Instance.PlaySFX(SFXManager.SFXType.Success);
            playerAnimation.MinigameResult = SuccessStatus.Good;
            playerAnimation.itemResult = GiveFish();
            playerAnimation.PickingAnim();
            Debug.Log("성공!");
        }
        else
        {
            SFXManager.Instance.PlaySFX(SFXManager.SFXType.Fail);
            playerAnimation.MinigameResult = SuccessStatus.Fail;
            playerAnimation.AtEndInteraction();
            Debug.Log("실패!");
        }
        gameObject.SetActive(false);
        IsFishing = false;

    }

    int GiveFish()
    {
        int randomfishID = UnityEngine.Random.Range(0, FISHINGRESULT.Length);
        PlayerInventory.Instance.AdditemsByID(FISHINGRESULT[randomfishID], 1);
        return FISHINGRESULT[randomfishID];
    }

    public void onPressed()
    {
        v = speed;
    }



}



