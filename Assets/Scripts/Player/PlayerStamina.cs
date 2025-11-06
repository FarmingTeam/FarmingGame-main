
using System;
using UnityEngine;


public class PlayerStamina : MonoBehaviour
{
    [SerializeField] public float max = 100;
    [SerializeField] public StaminaUI ui;
    
    private StaminaUI staminaUI;
    public float cur;

    public Action<int> StaminaUseActions;


    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        if (ui != null)
        {
            SetUI(ui);
        }
        
    }

    private void Update()
    {
        if (cur < 10 || cur == 10)
        {
            StartCoroutine(TutSpawner.Instance.DisplayTutorial(11));
        }
    }

    public void Init()
    {
        cur = max;
    }

    public void SetUI(StaminaUI ui)
    {
        staminaUI = ui;
        staminaUI?.Refresh(cur, max);
    }

    public bool Check(float cost = 0.0f)
    {
        if (cost == 0 && cur <= 0)
            return false;
        if (cur - cost < 0)
            return false;
        return true;
    }

    public bool Consume(float cost) // 조건부 차감
    {
        if (cur < cost)
        {
            return false;
        }
        cur -= cost;
        cur = Mathf.Clamp(cur, 0f, max);

        staminaUI?.Refresh(cur, max);
        StaminaUseActions?.Invoke((int)-cost);
        return true;
    }

    public void Recover(float amount)  // 회복
    {
        cur = Mathf.Clamp(cur + amount, 0f, max);
        staminaUI?.Refresh(cur, max);
        StaminaUseActions?.Invoke((int)amount);
    }
}
