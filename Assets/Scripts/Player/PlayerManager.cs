using System;
using System.IO;
using System.Text;
using UnityEngine;


[Serializable]
public class PlayerData
{
    public string Name;
    public StaminaData Stamina;
    public MoneyData Money;
}

[Serializable]
public class StaminaData
{
    public float staminaCur;
    public float staminaMax;
}

[Serializable]
public class MoneyData
{
    public int money;
}
public class PlayerManager : Singleton<PlayerManager>
{
    public string playerName;

    public PlayerStamina playerStamina;
    public PlayerPocketMoney playerMoney;

    public int ToolIdx = -1;
    public TileReader.Facing Direction = TileReader.Facing.Down;

    public const string PLAYERDATAJSON = "PlayerData.Json";


    protected override void Initialize()
    {
        if (NameInput.Instance != null)
        {
            playerName = NameInput.Instance.GetName();
            Destroy(NameInput.Instance.gameObject);
        }
        playerStamina = gameObject.AddComponent<PlayerStamina>();
        playerMoney = gameObject.AddComponent<PlayerPocketMoney>();
    }

    public void MakeInitialData()
    {
        playerStamina.Init();
        playerMoney.Init();
    }

    public void SaveCacheToSlot(int slotNumber)
    {
        //이름, 스테미너, 소지금  정보 저장
        StaminaData staminaData = new StaminaData();
        staminaData.staminaCur = playerStamina.cur;
        staminaData.staminaMax = playerStamina.max;

        MoneyData moneyData = new MoneyData();
        moneyData.money = playerMoney.Amount;

        PlayerData playerData = new PlayerData();
        playerData.Name = playerName;
        playerData.Stamina = staminaData;
        playerData.Money = moneyData;


        //파일 이름 지정
        StringBuilder stringBuilder1 = new StringBuilder();
        stringBuilder1.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(PLAYERDATAJSON);

        //위에서 만든 Data를 Json으로 변경
        string playerDataString = JsonUtility.ToJson(playerData, true);

        //파일에 해당 Json 텍스트를 파일에다 적기
        File.WriteAllText(stringBuilder1.ToString(), playerDataString);
    }


    public void LoadSlotToCache(int slotNumber)
    {
        StringBuilder stringBuilder1 = new StringBuilder();
        //디렉토리 경로
        stringBuilder1.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slotNumber)
            .Append("/").Append(PLAYERDATAJSON);

        if (!File.Exists(stringBuilder1.ToString()))
            return;

        //이름, 스테미너, 소지금로드
        PlayerData playerData = new PlayerData();

        string playerInfo = File.ReadAllText(stringBuilder1.ToString());


        playerData = JsonUtility.FromJson<PlayerData>(playerInfo);

        playerName = playerData.Name;
        playerStamina.cur = playerData.Stamina.staminaCur;
        playerStamina.max = playerData.Stamina.staminaMax;
        playerMoney.Init(playerData.Money.money);
    }
}
