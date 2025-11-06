using UnityEngine;

public class Questandinventory : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private QuestManager questManager;

    private PlayerInventory subscribedInventory;

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>(true);
        questManager = FindObjectOfType<QuestManager>(true);
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedSubscribe());
    }

    private void OnDisable()
    {
        TryUnsubscribe();
    }

    private System.Collections.IEnumerator DelayedSubscribe()
    {
        yield return new WaitForSeconds(0.1f);
        TrySubscribe();
        OnInventoryChanged();
    }

    private void Update()
    {
        var liveInv = PlayerInventory.Instance;
        if (liveInv != null && liveInv != subscribedInventory)
        {
            TryUnsubscribe();
            playerInventory = liveInv;
            TrySubscribe();
            OnInventoryChanged();
        }
    }

    private void TrySubscribe()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>(true);

        if (questManager == null)
            questManager = FindObjectOfType<QuestManager>(true);

        if (playerInventory != null && subscribedInventory != playerInventory)
        {
            playerInventory.SubscribeOnItemChange(OnInventoryChanged);
            subscribedInventory = playerInventory;
        }
    }

    private void TryUnsubscribe()
    {
        if (subscribedInventory != null)
        {
            subscribedInventory.UnsubscribeOnItemChange(OnInventoryChanged);
            subscribedInventory = null;
        }
    }

    private void OnInventoryChanged()
    {
        if (questManager == null)
            questManager = FindObjectOfType<QuestManager>(true);

        var inv = PlayerInventory.Instance ?? playerInventory;
        if (inv == null) return;
        if (questManager == null || questManager.questList == null) return;

        for (int qi = 0; qi < questManager.questList.Count; qi++)
        {
            QuestData quest = questManager.questList[qi];
            if (quest == null) continue;
            if (quest.Status != QuestStatus.InProgress) continue;
            if (quest.Conditions == null || quest.Conditions.Count == 0) continue;

            for (int ci = 0; ci < quest.Conditions.Count; ci++)
            {
                var cond = quest.Conditions[ci];
                if (cond == null || cond.Action != QuestAction.Deliver) continue;

                string targetNpcId = null;
                var d = cond as DeliverQuestCondition;
                if (d != null && !string.IsNullOrEmpty(d.TargetNPCID))
                    targetNpcId = d.TargetNPCID;
                if (string.IsNullOrEmpty(targetNpcId))
                    targetNpcId = cond.Target;

                var param = new DeliverQuestCheckParam
                {

                    NpcID = targetNpcId ?? "",
                    Inventory = inv
                };

                cond.OnProgress(param);
            }
        }

        var shud = FindObjectOfType<QuestHUD>(true);
        if (shud != null)
        {
            var m = shud.GetType().GetMethod("RefreshHUD");
            if (m != null) m.Invoke(shud, null);
        }

        var list = FindObjectOfType<Questlist>(true);
        if (list != null)
        {
            var m2 = list.GetType().GetMethod("RefreshUI");
            if (m2 != null) m2.Invoke(list, null);
        }
    }
}
