using System;
using TMPro;
using UnityEngine;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private CanvasGroup rootCanvas;

    [SerializeField, Range(0.2f, 1f)] private float completedAlpha = 0.6f;

    private QuestData _lastQuest;
    private float _timer;

    private void Update()
    {
        var qm = QuestManager.Instance;
        if (qm == null || qm.questList == null || qm.questList.Count == 0)
        {
            return;
        }

        var currentQuest = qm.questList[0];
        if (_lastQuest != currentQuest)
        {
            _lastQuest = currentQuest;
            Refresh();
        }

        _timer += Time.deltaTime;
        if (_timer >= 0.5f)
        {
            _timer = 0f;
            Refresh();
        }

    }



    private void OnEnable()
    {
        Refresh();
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged += Refresh;
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged -= Refresh;
    }
    public void Refresh()
    {
        var qm = QuestManager.Instance;
        if (qm == null || qm.questList == null || qm.questList.Count == 0)
        {
            SetEmpty();
            return;
        }
        QuestData candidate = null;
        for (int i = 0; i < qm.questList.Count; i++)
        {
            var q = qm.questList[i];
            if (q.Status == QuestStatus.InProgress)
            {
                candidate = q;
                break;
            }
        }
        if (candidate == null)
        {
            for (int i = 0; i < qm.questList.Count; i++)
            {
                var q = qm.questList[i];
                if (q.Status == QuestStatus.Completed)
                {
                    candidate = q;
                    break;
                }
            }
        }
        if (candidate == null)
        {
            SetEmpty();
            return;
        }

        Apply(candidate);
    }

    public void RefreshHUD()
    {
        Refresh();
    }

    private void SetEmpty()
    {
        if (titleText) titleText.text = "";
        if (objectiveText) objectiveText.text = "";
        if (stateText) stateText.text = "";
        if (rootCanvas) rootCanvas.alpha = 0f;
    }
    private void Apply(QuestData q)
    {
        if (rootCanvas) rootCanvas.alpha = 1f;
        if (titleText)
            titleText.text = string.IsNullOrEmpty(q.QuestTitle) ? q.QuestId : q.QuestTitle;

        if (stateText)
        {
            if (q.Status == QuestStatus.Completed)
            {
                stateText.text = "완료됨";
                stateText.color = new Color(1f, 1f, 1f, 0.85f);
            }
            else
            {
                stateText.text = "진행중";
                stateText.color = Color.white;
            }
        }

        string line = BuildOneLine(q);
        if (objectiveText) objectiveText.text = line;

        if (rootCanvas)
            rootCanvas.alpha = (q.Status == QuestStatus.Completed) ? completedAlpha : 1f;
    }

    private string BuildOneLine(QuestData q)
    {
        if (q.Conditions == null || q.Conditions.Count == 0)
            return "";

        var cond = q.Conditions[0];
        string desc = cond.Target;
        int cur = Mathf.Max(0, cond.CurrentCount);
        int req = Mathf.Max(1, cond.Count);

        if (!string.IsNullOrEmpty(desc) && desc.StartsWith("N"))
        {
            string npcName = TryGetNpcName(desc);
            if (!string.IsNullOrEmpty(npcName))
                desc = npcName;
        }
        switch (cond.Action)
        {
            case QuestAction.Talk:
                {
                    req = 1;
                    cur = q.Status == QuestStatus.Completed ? 1 : (cond.IsCompleted() ? 1 : 0);
                    if (string.IsNullOrEmpty(desc)) desc = "대화하기";
                    else desc = desc + "와 대화하기";
                    return $"{desc} {cur}/{req}";
                }

            case QuestAction.Deliver:
                {
                    string what;
                    var d = cond as DeliverQuestCondition;
                    if (d != null)
                    {
                        what = d.TypeFilter.HasValue
                            ? QuestSlot.FormatItemType(d.TypeFilter.Value)
                            : QuestSlot.BuildDeliverDesc(d);
                    }
                    else
                    {
                        what = string.IsNullOrEmpty(desc) ? "아이템" : desc;
                    }
                    return $"{what} 전달하기 {cur}/{req}";
                }

            default:
                {
                    if (string.IsNullOrEmpty(desc)) desc = "목표";
                    if (q.Status == QuestStatus.Completed) cur = req;
                    else cur = Mathf.Min(cur, req);

                    return $"{desc} {cur}/{req}";
                }
        }
    }
    private string TryGetNpcName(string npcId)
    {
        if (string.IsNullOrEmpty(npcId))
        {
            return "";
        }

        var npcManager = NPCManager.Instance;
        if (npcManager != null)
        {
            var listField = npcManager.GetType().GetField(
                "currentNPCs",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            var list = listField?.GetValue(npcManager) as System.Collections.IEnumerable;

            if (list != null)
            {
                foreach (var n in list)
                {
                    if (n == null)
                    {
                        continue;
                    }

                    var t = n.GetType();

                    var idField = t.GetField("npcID", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    var id = idField != null ? idField.GetValue(n) as string : null;
                    if (id != npcId)
                    {
                        continue;
                    }

                    var nameProp = t.GetProperty("displayName") ?? t.GetProperty("npcName") ?? t.GetProperty("name");
                    if (nameProp != null && nameProp.PropertyType == typeof(string))
                    {
                        var s = nameProp.GetValue(n) as string;
                        if (!string.IsNullOrEmpty(s)) return s;
                    }

                    var nameField = t.GetField("displayName") ?? t.GetField("npcName") ?? t.GetField("name");
                    if (nameField != null && nameField.FieldType == typeof(string))
                    {
                        var s = nameField.GetValue(n) as string;

                        if (!string.IsNullOrEmpty(s))
                        {
                            return s;
                        }
                    }

                    var comp = n as Component;
                    if (comp != null)
                    {
                        var s = comp.gameObject.name.Replace("(Clone)", "").Trim();
                        if (!string.IsNullOrEmpty(s))
                        {
                            return s;
                        }
                    }
                    break;
                }
            }
        }

        var datas = Resources.LoadAll<NPCData>("NPCData");
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i] != null && datas[i].NpcID == npcId && !string.IsNullOrEmpty(datas[i].NpcName))
            {
                return datas[i].NpcName;
            }

        }
        return npcId;
    }

}
