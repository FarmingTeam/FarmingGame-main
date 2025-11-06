using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;


[Serializable]
public class QuestSaveData
{
    public string QuestId;
    public QuestStatus Status;
    public List<bool> ConditionsCompleted;
}

[Serializable]
public class QuestSaveDataList
{
    public List<QuestSaveData> Quests = new List<QuestSaveData>();
}

public class QuestManager : Singleton<QuestManager>
{
    public List<QuestData> questList = new List<QuestData>();

    private Dictionary<string, QuestData> questDict = new Dictionary<string, QuestData>();
    public bool IsInitialized { get; private set; } = false;
    private int completedQuestCount = 0;
    private const int FUNNEL_START_STEP = 3; // 첫 퀘스트 완료 = 퍼널 스텝 3부터 시작
    protected override void Initialize()
    {
        // Singleton에서 Awake 시 호출되는 초기화 메서드
        LoadQuestsFromCsv("QuestData/Quest");
        InitializeQuestStates();
        IsInitialized = true;
        OnQuestStateChanged?.Invoke();
    }

    public void LoadQuestsFromCsv(string resourcePath)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);
        questList = CsvQuestParser.ParseQuestCsv(csvFile);
    }

    private void InitializeQuestStates()
    {
        questDict.Clear();
        foreach (var q in questList)
        {
            questDict[q.QuestId] = q;
            q.Status = string.IsNullOrEmpty(q.PreQuestId) ? QuestStatus.Available : QuestStatus.Locked;
        }
    }

    public void UpdateProgress(string questId, QuestAction action, object parameter)
    {
        if (!questDict.ContainsKey(questId)) return;
        var quest = questDict[questId];
        quest.UpdateProgress(parameter, action);
        if (quest.IsCompleted && quest.Status != QuestStatus.Completed)
            CompleteQuest(questId);
    }
    public event Action OnQuestStateChanged;
    public void CompleteQuest(string questId)
    {
        if (questDict.ContainsKey(questId))
        {
            questDict[questId].Status = QuestStatus.Completed;
            var ql = questList.FirstOrDefault(q => q.QuestId == questId);

            if (ql != null)
            {
                ql.Status = QuestStatus.Completed;
                Debug.Log("questList의 Status도 Completed로 변경됨: " + ql.QuestId);

                if (questId != "Q002")
                {
                    RewardUtil.DeductQuestItems(ql.DeductionTarget, ql.DeductionCount);
                }

                completedQuestCount++;
                int funnelStep = FUNNEL_START_STEP + (completedQuestCount - 1);
                AnalyticsManager.SendFunnelStepEvent(funnelStep);
                Debug.Log($"[Analytics] 퀘스트 완료: {questId}, 완료 카운트: {completedQuestCount}, 퍼널 스텝: {funnelStep}");
            }
            Debug.Log("퀘스트 Status 변경 완료: " + questDict[questId].Status);
            if (questId == "Q001")
            {
                StartCoroutine(Q001_QuestClearSequence(ql));
            }
            else
            {
                RewardUtil.GiveQuestReward(ql.QuestReward, ql.QuestGiverNpcId);
            }
            UnlockNextQuests(questId);
            Debug.Log($"{questId} 퀘스트가 완료되었습니다.");
            
            OnQuestStateChanged?.Invoke();
        }
    }

    private void UnlockNextQuests(string completedQuestId)
    {
        foreach (var quest in questDict.Values)
        {
            if (quest.PreQuestId == completedQuestId && quest.Status == QuestStatus.Locked)
            {
                quest.Status = QuestStatus.Available;
                Debug.Log($"{quest.QuestId} 퀘스트가 해금되었습니다.");

                var questInList = questList.FirstOrDefault(q => q.QuestId == quest.QuestId);
                if (questInList != null)
                {
                    questInList.Status = QuestStatus.Available;
                    Debug.Log($"[UnlockNextQuests] {quest.QuestId} 해금됨 (questList)");
                }
                else
                {
                    Debug.LogWarning($"[UnlockNextQuests] questList에서 {quest.QuestId}를 찾을 수 없음!");
                }
            }
        }
    }

    public List<string> GetQuestsForNPC(string npcId)
    {
        return questList
            .Where(q => q.QuestGiverNpcId.Equals(npcId, System.StringComparison.OrdinalIgnoreCase))
            .Select(q => q.QuestId)
            .ToList();
    }

    public bool CanStartQuest(string questId)
    {
        if (!questDict.ContainsKey(questId)) return false;
        var quest = questDict[questId];
        if (quest.Status != QuestStatus.Available) return false;

        if (!string.IsNullOrEmpty(quest.PreQuestId))
        {
            if (!questDict.ContainsKey(quest.PreQuestId)) return false;
            if (questDict[quest.PreQuestId].Status != QuestStatus.Completed) return false;
        }
        return true;
    }

    public void StartQuest(string questId)
    {
        if (CanStartQuest(questId))
        {
            questDict[questId].Status = QuestStatus.InProgress;
            Debug.Log($"{questId} 퀘스트가 시작됩니다.");
        }
        else
        {
            Debug.Log($"{questId} 퀘스트는 선행 퀘스트 미완료로 시작할 수 없습니다.");
        }
    }

    public QuestStatus GetQuestStatus(string questId)
    {
        if (questDict.ContainsKey(questId))
            return questDict[questId].Status;
        return QuestStatus.Locked;
    }

    public bool IsQuestGoalAchieved(string questId)
    {
        var quest = questList.FirstOrDefault(q => q.QuestId == questId);
        if (quest == null)
            return false;
        if (quest.Conditions == null || quest.Conditions.Count == 0)
            return true;
        // 조건이 있는 경우, 개별 조건 모두 달성됐는지 체크
        return quest.Conditions.All(cond => cond.IsCompleted());
    }

    public bool CanReceiveQuest(string questId, string npcId) // 특정 NPC 퀘스트 수락 가능 여부
    {
        if (!questDict.ContainsKey(questId)) return false;
        QuestData quest = questDict[questId];
        if (quest.Status != QuestStatus.Available) return false;
        if (!string.IsNullOrEmpty(quest.PreQuestId))
        {
            if (!questDict.ContainsKey(quest.PreQuestId)) return false;
            if (questDict[quest.PreQuestId].Status != QuestStatus.Completed) return false;
        }
        if (!string.Equals(quest.QuestGiverNpcId, npcId, StringComparison.OrdinalIgnoreCase))
        {
            // 이 NPC가 퀘스트를 줄 권한 없음
            return false;
        }
        return true;
    }

    // 아래부터 저장/로드 기능
    public void SaveQuestStatus(int slotNumber) // 저장기능
    {
        var saveList = new List<QuestSaveData>();
        foreach (var quest in questList)
        {
            saveList.Add(new QuestSaveData
            {
                QuestId = quest.QuestId,
                Status = quest.Status,
                ConditionsCompleted = quest.Conditions.Select(c => c.IsCompleted()).ToList()
            });
        }

        var wrapper = new QuestSaveDataList { Quests = saveList };
        string json = JsonUtility.ToJson(wrapper, true);

        string dirPath = Path.Combine(Application.persistentDataPath, SaveManager.SAVEFILEPATH + slotNumber);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        string savePath = Path.Combine(dirPath, "questSave.json");
        File.WriteAllText(savePath, json);

        string countPath = Path.Combine(dirPath, "questCompletedCount.txt");
        File.WriteAllText(countPath, completedQuestCount.ToString());

        Debug.Log("퀘스트 상태 저장 완료: " + savePath);
    }

    public void LoadQuestStatus(int slotNumber) // 로드기능
    {
        string dirPath = Path.Combine(Application.persistentDataPath, SaveManager.SAVEFILEPATH + slotNumber);
        string loadPath = Path.Combine(dirPath, "questSave.json");
        if (!File.Exists(loadPath))
        {
            Debug.LogWarning("퀘스트 저장 파일이 없습니다: " + loadPath);
            return;
        }
        string json = File.ReadAllText(loadPath);
        var wrapper = JsonUtility.FromJson<QuestSaveDataList>(json);

        foreach (var savedQuest in wrapper.Quests)
        {
            var quest = questList.Find(q => q.QuestId == savedQuest.QuestId);
            if (quest != null)
            {
                quest.Status = savedQuest.Status;
                for (int i = 0; i < quest.Conditions.Count && i < savedQuest.ConditionsCompleted.Count; i++)
                {
                    if (savedQuest.ConditionsCompleted[i])
                    {
                        // CurrentCount 등 완료 상태 반영
                    }
                }
            }
        }

        string countPath = Path.Combine(dirPath, "questCompletedCount.txt");
        if (File.Exists(countPath))
        {
            string countStr = File.ReadAllText(countPath);
            if (int.TryParse(countStr, out int loadedCount))
            {
                completedQuestCount = loadedCount;
                Debug.Log($"퀘스트 완료 카운트 로드: {completedQuestCount}");
            }
        }
        else
        {
            // 파일이 없으면 현재 Completed 상태인 퀘스트 개수로 계산
            completedQuestCount = questList.Count(q => q.Status == QuestStatus.Completed);
            Debug.Log($"퀘스트 완료 카운트 계산: {completedQuestCount}");
        }

        Debug.Log("퀘스트 상태 로드 완료: " + loadPath);
    }

    private IEnumerator Q001_QuestClearSequence(QuestData ql)
    {
        // *** 트랜지션 시작 ***
        SceneChangeManager.Instance.transitor.TransitStart();
        yield return new WaitForSeconds(0.5f);

        SFXManager.Instance.PlaySFX(SFXManager.SFXType.Footstep);

        RewardUtil.GiveQuestReward(ql.QuestReward, ql.QuestGiverNpcId);  //보상지급
        var toolbar = GameObject.Find("UIToolBar(Clone)");
        if (toolbar != null)
        {
            toolbar.SetActive(false);
            toolbar.SetActive(true);
        }
        if (ql.QuestId == "Q001")
        {
            // Q001: 특정 NPC 제거 필요
            NPCManager.Instance.DestroyNpc("N001"); // N001만 제거
        }
        GameObject clickBlocker = GameObject.Find("ClickBlocker");
        if (clickBlocker != null)
            clickBlocker.SetActive(false);
    }
}
