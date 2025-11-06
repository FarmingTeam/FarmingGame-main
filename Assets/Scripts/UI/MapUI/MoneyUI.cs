using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneytext;

    public void Refresh(int amount)
    {
        if (!moneytext)
        {
            return;
        }
        moneytext.text = $"{amount:N0}";
    }
}
