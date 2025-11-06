using System;
using TMPro;
using UnityEngine;


public class PlayerPocketMoney : MonoBehaviour
{
    [SerializeField] private int amount = 0; // 현재 소지금
    [SerializeField] private int step = 500; // 한번 누를때마다 증감되는 양

    [SerializeField] private TMP_Text moneytext;

    public int Amount => amount;
    public event Action<int> OnChanged;

    private void Awake()
    {
        Refresh();
    }

    public void Init()
    {
        amount = 1000;
    }

    public void Init(int money)
    {
        amount = money;
    }


    public void Add(int value)
    {
        value = Mathf.Max(0, value);
        amount = Mathf.Max(0, amount+value);
        OnChanged?.Invoke(amount);
        Refresh();
    }

    public bool TrySpend(int cost) 
    {
        cost = Mathf.Max(0, cost);
        if (amount < cost)
        {
            return false;
        }
        amount -= cost;
        OnChanged?.Invoke(amount);
        Refresh();
        return true;
    }

    public void Spend(int cost)

    {
        if (!TrySpend(cost))
        {
            Debug.Log("돈이 없어요");
        }
    }

    void Refresh()
    {
        if (moneytext)
        {
            moneytext.text = $"{amount:N0}";
        }
    }
}
