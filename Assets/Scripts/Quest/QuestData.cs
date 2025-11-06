
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class QuestData
{
    public string QuestId;
    public string QuestTitle;
    public string QuestReward;
    public string PreQuestId;    // 선행 퀘스트 ID, 없으면 빈 문자열
    public string QuestGiverNpcId;
    public QuestStatus Status;   // Locked, Available, InProgress, Completed 등
    public List<QuestConditionBase> Conditions = new List<QuestConditionBase>();

    public string DeductionTarget = "";  // 차감할 아이템 Target ( ex : "ItemID:1101~1106")
    public int DeductionCount = 0;
    public bool IsCompleted => Conditions != null && Conditions.All(x => x.IsCompleted());

    public void UpdateProgress(object parameter, QuestAction action)
    {
        foreach (var cond in Conditions)
        {
            if (cond.Action == action)
                cond.OnProgress(parameter);
        }
    }
}
