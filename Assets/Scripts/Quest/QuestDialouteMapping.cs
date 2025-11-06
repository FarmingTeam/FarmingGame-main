
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    Locked,
    NotStarted,
    InProgressNotAchieved,
    InProgressAchieved,
    Completed
}

public class DialogueRange
{
    public string StartIdx;
    public string EndIdx;
    public DialogueRange(string start, string end)
    {
        StartIdx = start;
        EndIdx = end;
    }
}

public class QuestDialogueMapping
{
    public Dictionary<string, Dictionary<QuestState, DialogueRange>> questDialogRanges = new Dictionary<string, Dictionary<QuestState, DialogueRange>>();

    public QuestDialogueMapping()
    {
        questDialogRanges["Q002"] = new Dictionary<QuestState, DialogueRange>()
        {
            { QuestState.Locked, new DialogueRange("D008", "D008") },
            { QuestState.NotStarted, new DialogueRange("D009", "D020") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D026", "D028") },
            { QuestState.InProgressAchieved, new DialogueRange("D021", "D025") },
        };
        questDialogRanges["Q003"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D032", "D032") },
            { QuestState.NotStarted, new DialogueRange("D033", "D037") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D038", "D038") },
            { QuestState.InProgressAchieved, new DialogueRange("D039", "D040") },
        };
        questDialogRanges["Q006"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D068", "D068") },
            { QuestState.NotStarted, new DialogueRange("D069", "D074") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D097", "D097") },
            { QuestState.InProgressAchieved, new DialogueRange("D075", "D075") },
        };
        questDialogRanges["Q007"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D076", "D076") },
            { QuestState.NotStarted, new DialogueRange("D077", "D081") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D098", "D098") },
            { QuestState.InProgressAchieved, new DialogueRange("D082", "D083") },
        };
        questDialogRanges["Q008"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D084", "D084") },
            { QuestState.NotStarted, new DialogueRange("D085", "D087") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D099", "D099") },
            { QuestState.InProgressAchieved, new DialogueRange("D088", "D090") },
        };
        questDialogRanges["Q009"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D047", "D047") },
            { QuestState.NotStarted, new DialogueRange("D048", "D056") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D095", "D095") },
            { QuestState.InProgressAchieved, new DialogueRange("D057", "D059") },
        };
        questDialogRanges["Q010"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D060", "D060") },
            { QuestState.NotStarted, new DialogueRange("D061", "D064") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D096", "D096") },
            { QuestState.InProgressAchieved, new DialogueRange("D065", "D067") },
        };
        questDialogRanges["Q012"] = new Dictionary<QuestState, DialogueRange>()
         {
            { QuestState.Locked, new DialogueRange("D099", "D099") },
            { QuestState.NotStarted, new DialogueRange("D114", "D116") },
            { QuestState.InProgressNotAchieved, new DialogueRange("D111", "D111") },
            { QuestState.InProgressAchieved, new DialogueRange("D112", "D113") },
        };
        // 필요 퀘스트 추가
    }

    public DialogueRange GetRange(string questId, QuestState state)
    {
        if (questDialogRanges.TryGetValue(questId, out var stateDict))
            if (stateDict.TryGetValue(state, out var range))
                return range;
        return null;
    }
}
