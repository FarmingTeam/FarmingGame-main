using System;

[Serializable]
public class TalkQuestCondition : QuestConditionBase
{
    public TalkQuestCondition(string target, int count)
    {
        Action = QuestAction.Talk;
        Target = target;
        Count = count;
        CurrentCount = 0;
    }

    public override bool IsCompleted() => CurrentCount >= Count;

    public override void OnProgress(object parameter)
    {
        if (parameter is string tid && tid == Target)
            CurrentCount++;
    }
}
