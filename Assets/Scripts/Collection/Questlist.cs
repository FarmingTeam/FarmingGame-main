using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Questlist : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject slotPrefab;

    private readonly List<QuestSlot> slots = new();

    private void OnEnable()
    {
        Refresh();
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStateChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStateChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (QuestManager.Instance == null)
        {
            return;
        }

        List<QuestData> all = QuestManager.Instance.questList;
        List<QuestData> viewList = new List<QuestData>();

        for (int i = 0; i < all.Count; i++)
        {
            QuestData q = all[i];
            if (q.Status == QuestStatus.InProgress || q.Status == QuestStatus.Completed)
            {
                viewList.Add(q);
            }
        }

        viewList.Sort(CompareQuests);
        EnsureSlotCount(viewList.Count);

        for (int i = 0; i < viewList.Count; i++)
        {
            slots[i].Set(viewList[i]);
            slots[i].gameObject.SetActive(true);
        }

        for (int i = viewList.Count; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }

    }

    private int CompareQuests(QuestData a, QuestData b)
    {
        int orderA = a.Status == QuestStatus.InProgress ? 0 : 1;
        int orderB = b.Status == QuestStatus.InProgress ? 0 : 1;

        if (orderA != orderB)
        {
            return orderA.CompareTo(orderB);
        }

        return string.Compare(a.QuestId, b.QuestId, System.StringComparison.Ordinal);
    }

    private void EnsureSlotCount(int count)
    {
        while (slots.Count < count)
        {
            GameObject go = Instantiate(slotPrefab, contentRoot, false);
            QuestSlot slot = go.GetComponent<QuestSlot>();
            slots.Add(slot);
        }
    }

    public void RefreshUI()
    {
        var mi = typeof(Questlist).GetMethod("Refresh", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (mi != null)
        {
            mi.Invoke(this, null);
        }
    }

}