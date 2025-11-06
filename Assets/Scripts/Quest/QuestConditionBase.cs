using System;
using UnityEngine;

[Serializable]
public abstract class QuestConditionBase
{
    public QuestAction Action { get; protected set; }
    public string Target { get; protected set; }
    public int Count { get; protected set; }
    public int CurrentCount { get; protected set; }

    public abstract bool IsCompleted();
    public abstract void OnProgress(object parameter);
}
