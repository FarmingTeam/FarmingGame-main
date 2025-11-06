using System;
using System.Collections;
using UnityEngine;


[Serializable]
public struct GameTime
{
    public int date;

    public int hour;
    public int minute;
}


public class TimeManager : Singleton<TimeManager>
{
    public GameTime currentTime;
    Coroutine currentCorutine;
    public Action<GameTime> TimeActions;

    bool isPaused = false;
    public bool forceSleep = false;

    [Header("Time Settings")]
    public float Duration = 10.0f;
    public float TimeSclae = 2.0f;
    [field: SerializeField] public int resetTime { get; private set; } = 2;


    protected override void Initialize()
    {
        isPaused = false;
        Init();
    }

    public void Init()
    {
        currentTime.date = GameInfoSaveManager.Instance.currentGameInfo.CurrentTime.date;
        currentTime.hour = GameInfoSaveManager.Instance.currentGameInfo.CurrentTime.hour;
        currentTime.minute = GameInfoSaveManager.Instance.currentGameInfo.CurrentTime.minute;
        StopAllCoroutines();
        currentCorutine = StartCoroutine(TimeLogic());
    }

    public void UpdateTime()
    {
        currentTime.minute += 10;
        if (currentTime.minute == 60)
        {
            currentTime.minute = 0;
            currentTime.hour++;
            if (currentTime.hour == 24)
            {
                currentTime.hour = 0;
                currentTime.date++;

                StartCoroutine(TutSpawner.Instance.DisplayTutorial(19));
            }
            //강제 취침
            else if (currentTime.hour == resetTime)
            {
                MapControl.Instance.player.playerAnimation.ForceSleepAnim();
            }
        }
    }

    public IEnumerator TimeLogic()
    {
        float timecounter = 0.0f;
        do
        {
            if (forceSleep)
                break;
            //Wait until Unpause
            while (isPaused)
            {
                yield return null;
            }
            timecounter += Time.deltaTime;
            if (timecounter >= Duration / TimeSclae)
            {
                timecounter = 0.0f;
                UpdateTime();
                if (forceSleep)
                    break;
                TimeActions?.Invoke(currentTime);
            }
            yield return null;
        } while (true);
        TimeActions?.Invoke(currentTime);
        SetTomorrow();
    }

    public void SetTomorrow()
    {
        MapControl.Instance.player.controller.IsInteract = true;
        StopCoroutine(currentCorutine);
        if (currentTime.hour >= 6)
            currentTime.date++;
        currentTime.hour = 6;
        currentTime.minute = 0;
        // 맵 리로딩 로직
        currentCorutine = StartCoroutine(EndDay());
    }

    public IEnumerator EndDay()
    {
        GameInfoSaveManager.Instance.SaveCacheGameInfo(true);
        PlayerManager.Instance.playerStamina.Init();
        if (forceSleep)
        {
            PlayerManager.Instance.playerStamina.cur /= 2;
            forceSleep = false;
        }
        SaveManager.Instance.OnSaveSlot(0, true);
        SceneChangeManager.Instance.ChangeScene(
            (SceneName)GameInfoSaveManager.Instance.currentGameInfo.CurrentScene, GameInfoSaveManager.Instance.currentGameInfo.CurrentLocation);
        while (TransitController.Instance.transitor.isTransitioning)
            yield return null;
        TutSpawner.Instance.isTutorOpen = false;
        currentCorutine = StartCoroutine(TimeLogic());
    }

    public int GetActualUpdateDate()
    {
        if (currentTime.hour >= 6)
            return currentTime.date;
        return currentTime.date - 1;
    }

    //UI가 열리고 닫힐 시 실행
    public void PauseTime(bool? PauseState = null)
    {
        if (PauseState == true)
        {
            isPaused = true;
        }
        else if (PauseState == false)
        {
            isPaused = false;
        }
        else
        {
            isPaused = !isPaused;
        }
    }
}
