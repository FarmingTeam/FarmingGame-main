
using System.Collections;
using System.Linq;
using UnityEngine;



public class NPC : MonoBehaviour
{
    public string npcID;
    public float interactDistance;
    public NPCData npcData;

    private NPCQuestHandler questHandler;
    private NPCDialogueLinkedUI dialogueManager;
    public GameObject dialogueAvailableIcon;
    public Transform playerTransform;
    public GameObject questAvailableIcon;   // 느낌표
    public GameObject questInProgressIcon; // 물음표

    void Start()
    {
        npcID = npcData.NpcID.Trim();
        var npcQuestIds = QuestManager.Instance.GetQuestsForNPC(npcID);
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        questHandler = GetComponent<NPCQuestHandler>();
        questHandler.npcID = npcID;
        dialogueManager = GetComponent<NPCDialogueLinkedUI>();
        questHandler.AutoSelectQuestID();
        dialogueManager.UpdateQuestID(questHandler.CurrentQuestID);
        Debug.Log($"[NPC: {gameObject.name}] Start 진입 - npcID: {npcID} / npcData: {npcData.NpcID} / questHandler.npcID: {questHandler?.npcID}");
        StartCoroutine(WaitForQuestManagerInitialization());

    }

    private void Update()
    {
        if (playerTransform == null)
            return;

        bool canTalk = false;
        var status = QuestManager.Instance.GetQuestStatus(questHandler.CurrentQuestID);
        if ((status == QuestStatus.InProgress || status == QuestStatus.Available) || HasDialogueOrQuest())
            canTalk = true;
        /*
        // **거리 기반 proximity 아이콘 제어**
        bool inRange = IsInteractable(playerTransform.position);

        if (dialogueAvailableIcon != null)
            dialogueAvailableIcon.SetActive(canTalk && inRange);*/

        if (questAvailableIcon != null)
            questAvailableIcon.SetActive(status == QuestStatus.Available);
        if (questInProgressIcon != null)
            questInProgressIcon.SetActive(status == QuestStatus.InProgress);
    }

    private bool HasDialogueOrQuest()
    {
        // 대화 가능한 기본 대화 존재 여부 확인
        var baseDialogues = NpcDialog.Instance.GetDialoguesByNpc(npcID)
                                 .Where(d => string.IsNullOrEmpty(d.ConnectQuest))
                                 .ToList();

        if (baseDialogues.Count > 0)
            return true;

        // 혹은 퀘스트와 연결된 대화가 존재하면 true 반환(필요시 추가 구현)
        // 현재는 기본 대화가 있는지만 체크

        return false;
    }

    private IEnumerator WaitForQuestManagerInitialization()
    {
        while (!QuestManager.Instance.IsInitialized)
            yield return null;

        // 초기화 완료 후 이벤트 구독 등록
        QuestManager.Instance.OnQuestStateChanged += OnQuestStateChanged;

        // 첫 상태 갱신 호출
        OnQuestStateChanged();
    }

    public bool IsInteractable(Vector2 InteractPos)
    {
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), InteractPos);
        if (dist <= interactDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnInteract()
    {
        if (playerTransform != null && !IsInteractable(playerTransform.position))
        {
            Debug.Log($"[NPC: {npcID}] 플레이어가 너무 멀리 있음. 거리: {Vector2.Distance(transform.position, playerTransform.position):F2}");
            return; // 거리가 멀면 대화 시작 안 함
        }
        NPCManager.Instance.IsInteracting = true;
        dialogueManager.StartDialogue();
    }

    void OnQuestStateChanged()
    {
        if (questHandler != null)
        {
            questHandler.AutoSelectQuestID();
        }
        else
        {
            Debug.LogWarning("npcQuestHandler가 null임");
        }
        questHandler.AutoSelectQuestID();
        dialogueManager.UpdateQuestID(questHandler.CurrentQuestID);
    }

}