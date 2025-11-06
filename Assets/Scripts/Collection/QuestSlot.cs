
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private Transform objectiveRoot;
    [SerializeField] private GameObject objectiveRowPrefab;

    public void Set(QuestData q)
    {
        ClearObjectives();

        if (q == null)
        {
            if (titleText)
            {
                titleText.text = "";
            }

            if (stateText)
            {
                stateText.text = "";
            }

            return;
        }

        if (titleText)
        {
            titleText.text = string.IsNullOrEmpty(q.QuestTitle) ? q.QuestId : q.QuestTitle;
        }

        if (stateText)
        {
            if (q.Status == QuestStatus.Completed)
            {
                stateText.text = "완료됨";
                stateText.color = new Color(1f, 1f, 1f, 0.6f);
            }

            else
            {
                stateText.text = "진행중";
                stateText.color = Color.white;
            }
        }

        if (q.Conditions == null || objectiveRoot == null || objectiveRowPrefab == null)
        {
            return;
        }

        string headerNpcId = null;
        if (q.Conditions != null && q.Conditions.Count > 0)
        {
            var first = q.Conditions[0];

            if (first.Action == QuestAction.Talk)
            {
                headerNpcId = first.Target;
            }

            else if (first.Action == QuestAction.Deliver)
            {
                var d = first as DeliverQuestCondition;
                if (d != null)
                {
                    headerNpcId = d.TargetNPCID;
                }

                if (string.IsNullOrEmpty(headerNpcId))
                {
                    headerNpcId = first.Target;
                }
            }
            else
            {
                headerNpcId = null;
            }
        }

        if (npcPortrait != null)
        {
            var data = FindNpcData(headerNpcId);
            if (data != null && data.sprite != null)
            {
                npcPortrait.sprite = data.sprite;
                npcPortrait.enabled = true;
                npcPortrait.preserveAspect = true;

                if (q.Status == QuestStatus.Completed)
                {
                    npcPortrait.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
                else
                {
                    npcPortrait.color = Color.white;
                }

            }

            else
            {
                npcPortrait.enabled = false;
            }
        }


        for (int i = 0; i < q.Conditions.Count; i++)
        {
            QuestConditionBase cond = q.Conditions[i];

            GameObject row = Instantiate(objectiveRowPrefab, objectiveRoot, false);
            TMP_Text label = row != null ? row.GetComponentInChildren<TMP_Text>() : null;
            if (label == null)
            {
                continue;
            }

            string desc = cond.Target;
            int current = cond.CurrentCount;
            int required = cond.Count;

            if ((cond.Action == QuestAction.Deliver))
            {
                var d = cond as DeliverQuestCondition;
                if (d != null)
                {
                    desc = BuildDeliverDesc(d);
                }
            }

            else if (!string.IsNullOrEmpty(desc) && desc.StartsWith("N"))
            {
                var npcName = GetNpcNameById(desc);

                if (!string.IsNullOrEmpty(npcName))
                {
                    desc = npcName;
                }
            }

            bool isTalkLike =
                cond.Action == QuestAction.Talk;

            if (isTalkLike)
            {
                required = 1;
                current = (q.Status == QuestStatus.Completed) ? 1 : (cond.IsCompleted() ? 1 : 0);

                if (string.IsNullOrEmpty(desc))
                {
                    desc = "대화하기";
                }
                else
                {
                    desc = desc + "와 대화하기";
                }
            }

            else
            {
                if (required <= 0)
                {
                    required = 1;
                }

                if (current < 0)
                {
                    current = 0;
                }

                if (current > required)
                {
                    current = required;
                }

                if (q.Status == QuestStatus.Completed)
                {
                    current = required;
                }


                if (string.IsNullOrEmpty(desc))
                {
                    desc = "목표";
                }
            }

            string progress = $"{current}/{required}";
            switch (cond.Action)
            {
                case QuestAction.Deliver:
                    label.text = $"{desc} 전달하기 {progress}";
                    break;
                case QuestAction.Talk:
                    label.text = $"{desc} {progress}";
                    break;

                default:
                    label.text = $"{desc} {progress}";
                    break;
            }
        }
    }

    private void ClearObjectives()
    {
        if (objectiveRoot == null)
        {
            return;
        }

        for (int i = objectiveRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(objectiveRoot.GetChild(i).gameObject);
        }
    }

    private string GetNpcNameById(string npcId)
    {
        if (string.IsNullOrEmpty(npcId))
        {
            return "";
        }

        NPCData[] allNpcs = Resources.LoadAll<NPCData>("NpcData");

        foreach (var npc in allNpcs)
        {
            if (npc != null && npc.NpcID == npcId)
            {
                return npc.NpcName;
            }
        }
        return npcId;
    }

    private static System.Collections.Generic.Dictionary<string, NPCData> _npcCache;

    private static NPCData FindNpcData(string npcId)
    {
        if (string.IsNullOrEmpty(npcId))
        {
            return null;
        }

        if (_npcCache != null && _npcCache.TryGetValue(npcId, out var hit))
        {
            return hit;
        }

        var all = Resources.LoadAll<NPCData>("NpcData");

        if (_npcCache == null)
        {
            _npcCache = new System.Collections.Generic.Dictionary<string, NPCData>();
        }

        for (int i = 0; i < all.Length; i++)
        {
            var d = all[i];
            if (d != null && !string.IsNullOrEmpty(d.NpcID))
                _npcCache[d.NpcID] = d;
        }

        _npcCache.TryGetValue(npcId, out var found);
        return found;

    }

    public static string BuildDeliverDesc(DeliverQuestCondition d)
    {
        if (d == null)
        {
            return "아이템";
        }

        if (d.TypeFilter.HasValue)
        {
            return FormatItemType(d.TypeFilter.Value);
        }

        if (d.ItemIDs == null || d.ItemIDs.Count == 0)
        {
            return "아이템";
        }

        var rm = ResourceManager.Instance;
        var names = new List<string>();
        bool allFish = true;


        for (int i = 0; i < d.ItemIDs.Count; i++)
        {

            string name = null;

            if (rm != null)
            {
                var ItemData = rm.GetItem(d.ItemIDs[i]);
                if (ItemData != null)
                {
                    name = ItemData.itemName;

                    if (ItemData.itemType != ItemType.Fish)
                    {
                        allFish = false;
                    }
                }
            }
            names.Add(string.IsNullOrEmpty(name) ? $"아이템{d.ItemIDs[i]}" : name);
        }

        if (allFish)
        {
            return "생선";
        }

        if (AllContain(names, new[] { "버섯" }))
        { 
            return "버섯";
        }

        if (AllContain(names, new[] { "꽃" }))
        { 
            return "꽃";
        }

        return string.Join("/", names);

    }

    private static bool AllContain(IList<string> names, string[] keywords)
    {
        if (names == null || names.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < names.Count; i++)
        {
            var n = names[i] ?? "";
            bool anyHit = false;


            for (int k = 0; k < keywords.Length; k++)
            {
                if (n.IndexOf(keywords[k], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    anyHit = true;
                    break;
                }

            }

            if (!anyHit)
            {
                return false;
            }
            
        }
        return true;
    }

    public static string FormatItemType(ItemType t)
    {
        switch (t)
        {
            case ItemType.Fish:
                return "생선";
            default:
                return "아이템";

        }
    }

}