
using System.Collections.Generic;
using UnityEngine;

public class NPCQuestHandler : MonoBehaviour
{
    public string npcID;
    public string CurrentQuestID { get; private set; }

    public void AutoSelectQuestID()
    {
        if (this == null || !gameObject)
        {
            Debug.LogWarning("[NPCQuestHandler] 객체가 이미 파괴됨. 메서드 호출 중단.");
            return;
        }

        Debug.Log($"[NPC: {gameObject.name}] AutoSelectQuestID - npcID: {npcID}");
        List<string> npcQuestIds = QuestManager.Instance.GetQuestsForNPC(npcID);
        Debug.Log($"[AutoSelectQuestID] npcID={npcID}, 퀘스트 수={npcQuestIds.Count}");

        // 모든 퀘스트 상태 출력
        foreach (var qid in npcQuestIds)
        {
            QuestStatus status = QuestManager.Instance.GetQuestStatus(qid);
            Debug.Log($"  {qid}: {status}");
        }

        // 우선순위 1: InProgress 퀘스트 찾기
        foreach (var qid in npcQuestIds)
        {
            QuestStatus status = QuestManager.Instance.GetQuestStatus(qid);
            if (status == QuestStatus.InProgress)
            {
                CurrentQuestID = qid;
                Debug.Log($"[AutoSelectQuestID] InProgress 퀘스트 선택: {CurrentQuestID}");
                return;
            }
        }

        // 우선순위 2: Available 퀘스트 찾기
        foreach (var qid in npcQuestIds)
        {
            QuestStatus status = QuestManager.Instance.GetQuestStatus(qid);
            if (status == QuestStatus.Available)
            {
                CurrentQuestID = qid;
                Debug.Log($"[AutoSelectQuestID] Available 퀘스트 선택: {CurrentQuestID}");
                return;
            }
        }

        // 우선순위 3: Completed 퀘스트 찾기 (대화용)
        foreach (var qid in npcQuestIds)
        {
            QuestStatus status = QuestManager.Instance.GetQuestStatus(qid);
            if (status == QuestStatus.Completed)
            {
                CurrentQuestID = qid;
                Debug.Log($"[AutoSelectQuestID] Completed 퀘스트 선택: {CurrentQuestID}");
                return;
            }
        }

        // 우선순위 4: 아무것도 없으면 비우기
        CurrentQuestID = string.Empty;
        Debug.LogWarning("[AutoSelectQuestID] 진행 가능 퀘스트 없음, CurrentQuestID가 비어있음");
    }

    public QuestStatus GetQuestStatus()
    {
        if (this == null || !gameObject)
        {
            Debug.LogWarning("[NPCQuestHandler] 객체가 이미 파괴됨. 메서드 호출 중단.");
            return QuestStatus.None;
        }
        return QuestManager.Instance.GetQuestStatus(CurrentQuestID);
    }
}
