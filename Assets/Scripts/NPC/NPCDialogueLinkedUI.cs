
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NPCDialogueLinkedUI : MonoBehaviour
{
    private List<DialogueRow> dialogues = new List<DialogueRow>();
    private int currentDialogueIndex = 0;
    private string currentQuestID;
    private QuestDialogueMapping questDialogueMapping = new QuestDialogueMapping();
    public GameObject shopUIPrefab;
    private bool isAcceptDialogue = false;
    private bool isWaitingForChoice = false;
    public NPCData currentNpcData;
    public GameObject uiToolBar;
    public GameObject staminaUI;
    private int q002TransitionCount = 0;

    public void UpdateQuestID(string questID)
    {
        if (this == null || gameObject == null)
        {
            Debug.LogWarning("[NPCDialogueLinkedUI] 객체가 파괴되어 접근 불가");
            return;
        }
        currentQuestID = questID;
        QuestStatus status = QuestManager.Instance.GetQuestStatus(currentQuestID);
        Debug.Log($"[디버그1][UpdateQuestID] currentQuestID={currentQuestID}, status={status}");
        if (status == QuestStatus.Completed) Debug.Log("---- Completed 분기 들어옴 ----");
        if (status == QuestStatus.Locked) Debug.Log("---- Locked 분기 들어옴 ----");

        // (1) 퀘스트 존재 확인
        var questData = QuestManager.Instance.questList.FirstOrDefault(q => q.QuestId == currentQuestID);
        if (questData == null)
        {
            Debug.LogWarning("questData가 null");
            // null 처리 분기 또는 리턴
        }
        if (questData != null)
        {
            foreach (var cond in questData.Conditions)
                if (cond.Action == QuestAction.Deliver)
                {
                    var deliverParam = new DeliverQuestCheckParam
                    {
                        NpcID = GetComponent<NPC>().npcID,
                        Inventory = PlayerInventory.Instance
                    };
                    cond.OnProgress(deliverParam);
                }
        }
        var allDialogs = NpcDialog.Instance.allDialogues; // 또는 GetDialoguesByNpc(GetComponent<NPC>().npcID)
        foreach (var d in allDialogs)
        {
            Debug.Log($"[ALL] {d.DialogIdx} / NPC:{d.ConnectNPC} / Quest:{d.ConnectQuest}");
        }

        bool goalAchieved = QuestManager.Instance.IsQuestGoalAchieved(currentQuestID);
        Debug.Log($"[퀘스트 진입] goalAchieved: {goalAchieved}");

        // (2) QuestID가 비어있으면 초기화
        if (string.IsNullOrEmpty(currentQuestID))
        {
            dialogues.Clear();
            currentDialogueIndex = 0;
            return;
        }

        // (3) QuestStatus 상태별 맵핑 먼저!
        if (status == QuestStatus.Completed)
        {
            bool iisQ003Completed = currentQuestID == "Q003";

            dialogues = NpcDialog.Instance.GetDialoguesByNpc(GetComponent<NPC>().npcID)
                .Where(d =>
                    string.IsNullOrEmpty(d.ConnectQuest) &&
                    !(iisQ003Completed && d.DialogIdx == "D032")
                )
                .ToList();
            Debug.Log("[DIALOGUES DEBUG]" + string.Join(",", dialogues.Select(d => d.DialogIdx + "(" + d.ConnectQuest + ")")));
            currentDialogueIndex = 0;
            return;
        }

        if (status == QuestStatus.Locked)
        {
            var range = questDialogueMapping.GetRange(currentQuestID, QuestState.Locked);
            Debug.Log($"[Locked 진입] range: {range?.StartIdx} ~ {range?.EndIdx}");
            if (range != null)
            {
                dialogues = NpcDialog.Instance.allDialogues
                    .Where(d => !string.IsNullOrEmpty(d.ConnectQuest)
                        && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper())
                    .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                    .Where(d =>
                    {
                        int num = int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray()));
                        int s = int.Parse(new string(range.StartIdx.Where(char.IsDigit).ToArray()));
                        int e = int.Parse(new string(range.EndIdx.Where(char.IsDigit).ToArray()));
                        return num >= s && num <= e;
                    })
                    .ToList();
                Debug.Log($"[Locked 진입] 필터된 dialogues: {string.Join(",", dialogues.Select(d => d.DialogIdx))}");
                currentDialogueIndex = 0;
            }
            else
            {
                dialogues.Clear();
                currentDialogueIndex = 0;
            }
            return;
        }

        if (status == QuestStatus.InProgress && goalAchieved)
        {
            // 여기는 목표 달성만 체크 (클리어로직 별도 분리 가능)
        }

        if (status == QuestStatus.InProgress)
        {
            bool isGoalAchieved = QuestManager.Instance.IsQuestGoalAchieved(currentQuestID);

            // isAcceptDialogue 변수는 수락 대화 중임을 나타내게 외부에서 설정해주어야 함
            if (isAcceptDialogue)
            {
                // 수락 대화(초기 대화) 필터링 예시 (대사 인덱스로 구분)
                dialogues = NpcDialog.Instance.allDialogues
                    .Where(d =>
                        !string.IsNullOrEmpty(d.ConnectQuest)
                        && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper()
                        && d.DialogIdx.StartsWith("D001") // 예: 초기 대사 D001 ~ D005 범위 설정 필요
                    )
                    .Where(d => d.ConnectNPC != null && CleanDialogIdx(d.ConnectNPC) == CleanDialogIdx(GetComponent<NPC>().npcID))
                    .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                    .ToList();

                currentDialogueIndex = 0;

                // 대화가 끝나면 isAcceptDialogue = false; 로 전환하는 로직이 필요
                return;
            }
            else if (isGoalAchieved)
            {
                // 조건 달성 후 클리어 대사 필터링
                var range = questDialogueMapping.GetRange(currentQuestID, QuestState.InProgressAchieved);
                if (range != null)
                {
                    dialogues = NpcDialog.Instance.allDialogues
                        .Where(d =>
                            !string.IsNullOrEmpty(d.ConnectQuest)
                            && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper()
                            && InRange(d.DialogIdx, range.StartIdx, range.EndIdx)
                        )
                        .Where(d => d.ConnectNPC != null && CleanDialogIdx(d.ConnectNPC) == CleanDialogIdx(GetComponent<NPC>().npcID))
                        .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                        .ToList();

                    currentDialogueIndex = 0;
                    return;
                }
            }
            else
            {
                // 진행중이지만 목표 달성 못한 경우 기존 진행중 대사 처리
                var range = questDialogueMapping.GetRange(currentQuestID, GetQuestState(currentQuestID));
                if (range != null)
                {
                    dialogues = NpcDialog.Instance.allDialogues
                        .Where(d =>
                            !string.IsNullOrEmpty(d.ConnectQuest)
                            && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper()
                            && InRange(d.DialogIdx, range.StartIdx, range.EndIdx)
                        )
                        .Where(d => d.ConnectNPC != null && CleanDialogIdx(d.ConnectNPC) == CleanDialogIdx(GetComponent<NPC>().npcID))
                        .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                        .ToList();

                    currentDialogueIndex = 0;
                }
                else
                {
                    dialogues.Clear();
                    currentDialogueIndex = 0;
                }
                return;
            }
        }

        if (status == QuestStatus.Available)
        {
            // Available 시점 맵핑
            var range = questDialogueMapping.GetRange(currentQuestID, QuestState.NotStarted);
            Debug.Log($"[Available 진입] range: {range?.StartIdx} ~ {range?.EndIdx}");
            if (range != null)
            {
                dialogues = NpcDialog.Instance.allDialogues
                     .Where(d =>
                     (
                        !string.IsNullOrEmpty(d.ConnectQuest)
                        && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper()
                     )
                    ||
                     (
                        d.DialogIdx.Trim().ToUpper() == "D042"
                        && d.ConnectQuest != null
                        && d.ConnectQuest.Trim().ToUpper() == "Q004"
                     )
                     )
                    .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                    .Where(d =>
                    {
                        int num = int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray()));
                        int s = int.Parse(new string(range.StartIdx.Where(char.IsDigit).ToArray()));
                        int e = int.Parse(new string(range.EndIdx.Where(char.IsDigit).ToArray()));
                        return num >= s && num <= e;
                    })
                    .ToList();
                Debug.Log($"[Available 진입] 필터된 dialogues: {string.Join(",", dialogues.Select(d => d.DialogIdx))}");
                currentDialogueIndex = 0;
            }
            else
            {
                dialogues.Clear();
                currentDialogueIndex = 0;
            }
            return;
        }
        bool isQ003Completed = QuestManager.Instance.GetQuestStatus("Q003") == QuestStatus.Completed;

        // 기본 대화 fallback
        dialogues = NpcDialog.Instance.GetDialoguesByNpc(GetComponent<NPC>().npcID)
    .Where(d =>
         string.IsNullOrEmpty(d.ConnectQuest)
         || (CleanDialogIdx(d.DialogIdx) == "D042")   // 추가!
         || (d.DialogIdx.Trim().ToUpper() == "D042" && d.ConnectQuest != null && d.ConnectQuest.Trim().ToUpper() == "Q004"))
    .Where(d => !(isQ003Completed && CleanDialogIdx(d.DialogIdx) == "D028"))
    .ToList();
        Debug.Log("[DIALOGUES DEBUG]" + string.Join(",", dialogues.Select(d => d.DialogIdx + "(" + d.ConnectQuest + ")")));
        currentDialogueIndex = 0;
    }

    public void StartDialogue()
    {
        var uiSwitch = FindObjectOfType<UISwitch>();
        if (uiSwitch != null && uiSwitch.inventoryAction != null)
            uiSwitch.inventoryAction.action.Disable(); // 탭키 비활성화
        NPCManager.Instance.IsInteracting = true;
        var Status = QuestManager.Instance.GetQuestStatus(currentQuestID);
        List<DialogueRow> baseDialogues = null;
        List<DialogueRow> questDialogues = null;
        DialogueUI dialogueUI = null;

        // 1. 퀘스트 NPC(Available)
        if (!string.IsNullOrEmpty(currentQuestID) && Status == QuestStatus.Available)
        {
            QuestManager.Instance.StartQuest(currentQuestID);
            isAcceptDialogue = true;

            var range = questDialogueMapping.GetRange(currentQuestID, QuestState.NotStarted);
            if (range == null)
            {
                Debug.LogWarning($"[StartDialogue] DialogueRange 매핑 없음: {currentQuestID}");

                questDialogues = NpcDialog.Instance.allDialogues
                    .Where(d => !string.IsNullOrEmpty(d.ConnectQuest)
                                && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper())
                    .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                    .ToList();

                if (questDialogues.Count > 0)
                {
                    dialogues = questDialogues;
                    currentDialogueIndex = 0;
                    dialogueUI = UIManager.Instance.GetUI<DialogueUI>();
                    if (dialogueUI != null)
                        dialogueUI.OnDialogueScreenClick = ShowNextDialogue;
                    UIManager.Instance.OpenUI<DialogueUI>();
                    Time.timeScale = 0f;
                    ShowNextDialogue();
                    return;
                }
                // 없으면 기본 대사 분기로!
                baseDialogues = NpcDialog.Instance.GetDialoguesByNpc(GetComponent<NPC>().npcID)
                   .Where(d => string.IsNullOrEmpty(d.ConnectQuest)).ToList();
                if (baseDialogues.Count == 0)
                {
                    NPCManager.Instance.IsInteracting = false;
                    UIManager.Instance.CloseUI<DialogueUI>();
                    Debug.LogWarning("퀘스트/매핑 없는 NPC 대화 데이터 없음");
                    return;
                }
                dialogues = baseDialogues;
                currentDialogueIndex = 0;
                var baseDialogueUI = UIManager.Instance.GetUI<DialogueUI>();
                if (baseDialogueUI != null)
                    baseDialogueUI.OnDialogueScreenClick = ShowNextDialogue;
                UIManager.Instance.OpenUI<DialogueUI>();
                Time.timeScale = 0f;
                ShowNextDialogue();
                return;
            }

            var allQuestDialogues = NpcDialog.Instance.allDialogues
                .Where(d => !string.IsNullOrEmpty(d.ConnectQuest)
                    && d.ConnectQuest.Trim().ToUpper() == currentQuestID.Trim().ToUpper())
                .OrderBy(d => int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray())))
                .ToList();

            dialogues = allQuestDialogues
                .Where(d =>
                {
                    int num = int.Parse(new string(d.DialogIdx.Where(char.IsDigit).ToArray()));
                    int s = int.Parse(new string(range.StartIdx.Where(char.IsDigit).ToArray()));
                    int e = int.Parse(new string(range.EndIdx.Where(char.IsDigit).ToArray()));
                    return num >= s && num <= e;
                })
                .ToList();

            currentDialogueIndex = 0;
            var dialogueUis = UIManager.Instance.GetUI<DialogueUI>();
            if (dialogueUis != null)
                dialogueUis.OnDialogueScreenClick = ShowNextDialogue;
            UIManager.Instance.OpenUI<DialogueUI>();
            Time.timeScale = 0f;
            ShowNextDialogue();
            return;
        }

        // 2. 퀘스트 NPC(나머지)
        if (!string.IsNullOrEmpty(currentQuestID))
        {
            isAcceptDialogue = false;
            UpdateQuestID(currentQuestID);
            currentDialogueIndex = 0;

            if (dialogues.Count == 0)
            {
                NPCManager.Instance.IsInteracting = false;
                UIManager.Instance.CloseUI<DialogueUI>();
                if (uiSwitch != null && uiSwitch.inventoryAction != null)
                    uiSwitch.inventoryAction.action.Enable();
                Debug.LogWarning("퀘스트 대화 데이터 없음");
                return;
            }

            var dialogueUIs = UIManager.Instance.GetUI<DialogueUI>();
            if (dialogueUIs != null)
                dialogueUIs.OnDialogueScreenClick = ShowNextDialogue;
            UIManager.Instance.OpenUI<DialogueUI>();
            Time.timeScale = 0f;
            ShowNextDialogue();
            return;
        }

        // 3. 퀘스트 없는 NPC
        baseDialogues = NpcDialog.Instance.GetDialoguesByNpc(GetComponent<NPC>().npcID)
           .Where(d => string.IsNullOrEmpty(d.ConnectQuest)).ToList();

        Debug.Log($"[StartDialogue] baseDialogues count: {baseDialogues.Count}");

        if (baseDialogues.Count == 0)
        {
            NPCManager.Instance.IsInteracting = false;
            UIManager.Instance.CloseUI<DialogueUI>();
            if (uiSwitch != null && uiSwitch.inventoryAction != null)
                uiSwitch.inventoryAction.action.Enable(); // 탭키 활성화
            Debug.LogWarning("퀘스트 없는 NPC 대화 데이터 없음");
            return;
        }

        dialogues = baseDialogues;
        currentDialogueIndex = 0;

        dialogueUI = UIManager.Instance.GetUI<DialogueUI>();
        if (dialogueUI != null)
            dialogueUI.OnDialogueScreenClick = ShowNextDialogue;
        UIManager.Instance.OpenUI<DialogueUI>();
        Time.timeScale = 0f;
        ShowNextDialogue();
    }

    public void ShowNextDialogue()
    {
        var dialogueUI = UIManager.Instance.GetUI<DialogueUI>();
        if (dialogueUI != null && dialogueUI.IsTyping)
        {
            // 타이핑 중이면 바로 완료시키고 대사 넘김 방지
            dialogueUI.FinishTypingImmediately();
            return;
        }

        if (isWaitingForChoice)
        {
            Debug.Log("선택지 대기 중이므로 ShowNextDialogue 중복 실행 차단");
            return;
        }

        Debug.Log($"[FLOW] ShowNextDialogue called idx={currentDialogueIndex}, dialogues.Count={dialogues.Count}");
        if (dialogues.Count > currentDialogueIndex)
        {
            var curr = dialogues[currentDialogueIndex];
            Debug.Log($"[FLOW] DialogIdx={curr.DialogIdx}, selections.Count={curr.Selections?.Count ?? -1}");
        }

        Debug.Log($"ShowNextDialogue CALLED, currIdx:{currentDialogueIndex} / total:{dialogues?.Count ?? 0}");
        if (dialogues == null || currentDialogueIndex >= dialogues.Count)
        {

            UIManager.Instance.CloseUI<DialogueUI>();
            Time.timeScale = 1f;
            var uiSwitch = FindObjectOfType<UISwitch>();
            if (uiSwitch != null && uiSwitch.inventoryAction != null)
                uiSwitch.inventoryAction.action.Enable(); // 탭키 활성화
            
            if (currentQuestID == "Q002" && QuestManager.Instance.GetQuestStatus("Q002") == QuestStatus.InProgress && (dialogues.Count > 0 && dialogues[dialogues.Count - 1].DialogIdx == "D020") &&
                q002TransitionCount < 1) // 지급 트리거 대사 idx
            {
                q002TransitionCount++;
                PlayerEquipment.Instance.AddEquipmentByID(10007);
                var toolbar = GameObject.FindObjectOfType<UIToolBar>();
                if (toolbar != null && toolbar.gameObject.activeInHierarchy)
                {
                    toolbar.SetToolBar();
                }

                StartCoroutine(TutSpawner.Instance.DisplayTutorial(7));
            }

            if (!string.IsNullOrEmpty(currentQuestID))
            {
                var questData = QuestManager.Instance.questList.FirstOrDefault(q => q.QuestId == currentQuestID);
                var status = QuestManager.Instance.GetQuestStatus(currentQuestID);
                bool isGoalAchieved = QuestManager.Instance.IsQuestGoalAchieved(currentQuestID);
                bool noGoalQuest = questData != null && (questData.Conditions == null || questData.Conditions.Count == 0);

                // (1) 조건 없는/대사퀘스트 강제 완료 처리
                bool shouldCompleteDirect =
                    (questData.Conditions == null || questData.Conditions.Count == 0) ||
                    (questData.Conditions.All(c => c.Action == QuestAction.Talk));

                // (2) InProgress고 목표 달성되었으면 바로 완료!
                if ((status == QuestStatus.InProgress && (isGoalAchieved || noGoalQuest))
                    || (status == QuestStatus.InProgress && shouldCompleteDirect))
                {
                    QuestManager.Instance.CompleteQuest(currentQuestID);
                    Debug.Log($"[ShowNextDialogue] 퀘스트 자동 완료 처리: {currentQuestID}");
                    UpdateQuestID(currentQuestID);
                }
            }
            NPCManager.Instance.IsInteracting = false;
            currentDialogueIndex = 0;
            return;
        }
        DialogueRow currentDialogue = dialogues[currentDialogueIndex];
        dialogueUI.OnDialogueScreenClick = ShowNextDialogue;

        // 선택지가 있으면 버튼 동적 생성 및 분기
        if (currentDialogue.Selections != null && currentDialogue.Selections.Count > 0)
        {
            isWaitingForChoice = true;
            ShowDialogue(currentDialogue);

            var choices = currentDialogue.Selections; // ["상점", "대화하기", "종료"]
            dialogueUI.ShowChoice(choices.ToArray(), choiceIdx =>
            {
                isWaitingForChoice = false;
             
                // NextDialog 셀 값 파싱
                string[] nextIndices = null;
                if (!string.IsNullOrEmpty(currentDialogue.NextDialog))
                {
                    string ndCsv = currentDialogue.NextDialog
                         .Replace("{", "")
                         .Replace("}", "")
                         .Replace(" ", "")
                         .Replace("\r", "")
                         .Replace("\n", "")
                         .Replace("\t", "")
                         .Replace("'", "")
                         .Replace("\"", "")
                         .Replace(", ", ",");
                    nextIndices = ndCsv
                        .Split(',')
                        .Select(s => CleanDialogIdx(s))
                        .ToArray();
                }
                Debug.Log($"Choices: {string.Join(",", choices)}");
                Debug.Log($"nextIndices: {string.Join(",", nextIndices)}");
                Debug.Log($"choiceIdx: {choiceIdx}, nextIdx: {nextIndices[choiceIdx]}");
                for (int i = 0; i < (nextIndices?.Length ?? 0); i++)
                {
                    Debug.Log($"[DEBUG] nextIndices[{i}] = [{nextIndices[i]}]");
                }
                Debug.Log($"선택지 idx: {choiceIdx}, nextIndices 전체: {string.Join(", ", nextIndices ?? new string[] { })}");
                if (nextIndices != null && choiceIdx < nextIndices.Length)
                {
                    string nextIdx = nextIndices[choiceIdx];
                    var hasD042 = dialogues.Any(d => CleanDialogIdx(d.DialogIdx) == "D042");

                    Debug.Log("[dialogues 전체]" + string.Join(",", dialogues.Select(d => $"{CleanDialogIdx(d.DialogIdx)}({d.ConnectQuest})")));

                    int NextIdxPos = dialogues.FindIndex(d => CleanDialogIdx(d.DialogIdx) == nextIdx);

                    Debug.Log($"nextIdxPos: {NextIdxPos}, total dialogues: {dialogues.Count}");

                    if (choices[choiceIdx] == "대화하기" && currentDialogue.DialogIdx == "D040" && GetComponent<NPC>().npcID == "N005")
                    {
                        QuestManager.Instance.StartQuest("Q004");
                        Debug.Log("Q004 퀘스트를 시작합니다!");
                    }
                    // "상점"이면 상점UI 오픈
                    if (choices[choiceIdx] == "상점")
                    {
                        OpenShop();
                        UIManager.Instance.CloseUI<DialogueUI>();
                        TimeManager.Instance.PauseTime(true);
                        currentDialogueIndex = 0;
                        return;
                    }

                    // "종료" 처리
                    else if (nextIdx.ToUpper() == "EXIT" || choices[choiceIdx] == "종료")
                    {
                        UIManager.Instance.CloseUI<DialogueUI>();
                        Time.timeScale = 1f;
                        TimeManager.Instance.PauseTime(false);
                        var uiSwitch = FindObjectOfType<UISwitch>();
                        if (uiSwitch != null && uiSwitch.inventoryAction != null)
                            uiSwitch.inventoryAction.action.Enable(); // 탭키 활성화
                        currentDialogueIndex = 0;
                        NPCManager.Instance.IsInteracting = false;
                        return;
                    }
                    else
                    {
                        int nextIdxPos = dialogues.FindIndex(d => CleanDialogIdx(d.DialogIdx) == nextIdx);
                        currentDialogueIndex = nextIdxPos >= 0 ? nextIdxPos : currentDialogueIndex + 1;
                        ShowNextDialogue();
                        return;
                    }

                }
                else
                {
                    currentDialogueIndex++;
                    ShowNextDialogue();
                    return;
                }
            });

            return;
        }

        ShowDialogue(currentDialogue);
        // 선택지 없으면 기본적으로 다음 대사
        if (!string.IsNullOrEmpty(currentDialogue.NextDialog))
        {
            string nd = CleanDialogIdx(currentDialogue.NextDialog);
            int nextPos = dialogues.FindIndex(d => CleanDialogIdx(d.DialogIdx) == nd);
            currentDialogueIndex = nextPos >= 0 ? nextPos : dialogues.Count; // 더 없으면 종료
        }
        else
        {
            currentDialogueIndex = dialogues.Count; // 종료(혹은 대화 끄기)
        }

        dialogueUI.SetClickable(true);
        Debug.Log("ShowNextDialogue tail reached, next idx: " + currentDialogueIndex + " / total: " + (dialogues?.Count ?? 0));
    }

    private string CleanDialogIdx(string s) // 특수문자 구분  및 공백 제거
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("{", "")
                .Replace("}", "")
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\t", "")
                .Replace("\n", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Trim().ToUpper();
    }

    private void ShowDialogue(DialogueRow dialogue)
    {
        if (this == null || gameObject == null)
            return;

        var npcComp = GetComponent<NPC>();
        if (npcComp == null || npcComp.npcData == null)
        {
            Debug.LogWarning("[ShowDialogue] NPC 컴포넌트 또는 데이터 없음");
            return;
        }

        DialogueUI dialogueUI = UIManager.Instance.GetUI<DialogueUI>();
        if (dialogueUI == null)
        {
            Debug.LogWarning("[ShowDialogue] DialogueUI를 찾을 수 없음");
            return;
        }
        Debug.Log($"Speaker raw value: '{dialogue.ConnectNPC}', normalized: '{dialogue.ConnectNPC.Trim().ToLower()}'");
        string speaker = dialogue.ConnectNPC.Trim().ToLower() == "p" || dialogue.ConnectNPC.ToLower() == "player"
            ? PlayerManager.Instance.playerName
            : GetComponent<NPC>().npcData.NpcName;
        string dialogueText = dialogue.DialogCon;
        if (dialogueText.Contains("{PlayerName}"))
        {
            string playerName = PlayerManager.Instance.playerName; // 실제 플레이어명
            dialogueText = dialogueText.Replace("{PlayerName}", playerName);
        }
        dialogueUI.SetNpcSprite(currentNpcData.sprite);
        dialogueUI.SetDialogue(speaker, dialogueText);
        Debug.Log($"SetDialogueGradual 호출 전 isTyping: {dialogueUI.IsTyping}");
        dialogueUI?.SetDialogueGradual(dialogueText, 0.02f);
        Debug.Log($"SetDialogueGradual 호출 후 isTyping: {dialogueUI.IsTyping}");
        if (dialogueUI != null)
        {
            dialogueUI.UpdateFriendshipHearts(GetComponent<NPC>().npcID);
        }
    }

    private QuestState GetQuestState(string questId)
    {
        QuestStatus status = QuestManager.Instance.GetQuestStatus(questId);
        bool goalAchieved = QuestManager.Instance.IsQuestGoalAchieved(questId);
        Debug.Log($"[QuestState 체크] status={status}, goalAchieved={goalAchieved}");

        switch (status)
        {
            case QuestStatus.Locked:
                return QuestState.Locked;
            case QuestStatus.Available:
                return QuestState.NotStarted;
            case QuestStatus.InProgress:
                return goalAchieved ? QuestState.InProgressAchieved : QuestState.InProgressNotAchieved;
            case QuestStatus.Completed:
                return QuestState.Completed;
            default:
                return QuestState.NotStarted;
        }
    }
    public void OpenShop()
    {
        var uiSwitch = FindObjectOfType<UISwitch>();
        if (uiSwitch != null && uiSwitch.inventoryAction != null)
            uiSwitch.inventoryAction.action.Disable();

        string npcID = GetComponent<NPC>().npcID;
        var shopUI = FindObjectOfType<ShopUIController>();
        if (shopUI != null)
        {
            shopUI.OpenShopUIWithBuyTab(npcID);
        }
        else
        {
            // Canvas로 부모 설정
            var canvas = UIManager.Instance.canvas;
            GameObject instance = Instantiate(shopUIPrefab, canvas.transform);

            // 하위 오브젝트까지 검색해서 찾기
            var shopCtrl = instance.GetComponentInChildren<ShopUIController>(true);
            if (shopCtrl != null)
                shopCtrl.OpenShopUIWithBuyTab(npcID);
            else
                Debug.LogError("Prefab에 ShopUIController 컴포넌트 없어 실행 불가");
        }
    }
    bool InRange(string dialogIdx, string start, string end)
    {
        int num = int.Parse(new string(dialogIdx.Where(char.IsDigit).ToArray()));
        int s = int.Parse(new string(start.Where(char.IsDigit).ToArray()));
        int e = int.Parse(new string(end.Where(char.IsDigit).ToArray()));
        return num >= s && num <= e;
    }
}
