
using UnityEngine;

public class MoneyUICreator: MonoBehaviour
{
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private PlayerPocketMoney wallet;

    private MoneyUI ui;

    private void Start()
    {
        parent = GetComponent<Canvas>().transform;
        var go = Instantiate(uiPrefab, parent);
        go.name = "PocketMoneyUI";
        go.SetActive(true);

        ui = go.GetComponent<MoneyUI>();

        wallet = PlayerManager.Instance.playerMoney;
        if (wallet == null || ui == null)
            return;

        ui.Refresh(wallet.Amount);
        wallet.OnChanged += ui.Refresh;
    }

    private void OnDestroy()
    {
        if (wallet != null)
        {
            wallet.OnChanged -= ui.Refresh;
        }
    }
}
