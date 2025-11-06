
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISaveDataButton : MonoBehaviour
{
    [SerializeField] UILoaderPopup uiLoaderPopup;
    [SerializeField] int slotNumber;
    [SerializeField] Button button;

    [SerializeField] TMP_Text saveDateText;
    [SerializeField] TMP_Text playTimeText;
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text nameText;

    [SerializeField] RawImage screenshot;

    public void OnEnable()
    {
        UpdateUI(slotNumber);
    }

    public void UpdateUI(int slot)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Application.persistentDataPath).Append("/").Append(SaveManager.SAVEFILEPATH).Append(slot);
        string path = stringBuilder.ToString();
        if (Directory.Exists(path))
        {
            stringBuilder.Clear().Append(path).Append('/').Append(GameInfoSaveManager.GAMEINFONAME);
            saveDateText.text = File.GetCreationTime(path).ToString("yyyy-MM-dd");

            string gameInfoString = File.ReadAllText(stringBuilder.ToString());
            GameInfo currentGameInfo = JsonUtility.FromJson<GameInfo>(gameInfoString);

            StringBuilder DateStringBuilder = new StringBuilder();
            playTimeText.text = DateStringBuilder.Append("Day").Append(currentGameInfo.CurrentTime.date).Append("  ")
                .Append(currentGameInfo.CurrentTime.hour.ToString("D2")).Append(" : ")
                .Append(currentGameInfo.CurrentTime.minute.ToString("D2")).ToString();

            stringBuilder.Clear().Append(path).Append('/').Append(PlayerManager.PLAYERDATAJSON);
            string playerInfoString = File.ReadAllText(stringBuilder.ToString());
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(playerInfoString);
            goldText.text = playerData.Money.money.ToString() + " G";
            nameText.text = playerData.Name.ToString();


            stringBuilder.Clear().Append(path).Append('/').Append(SaveManager.PREVIEWNAME);
            byte[] imageData = File.ReadAllBytes(stringBuilder.ToString());
            Texture2D texture = new Texture2D(150, 150);
            if (texture.LoadImage(imageData))
            {
                screenshot.color = Color.white;
                screenshot.texture = texture;
            }
            button.interactable = true;
        }
        else
            button.interactable = false;

    }

    public void OnLoadSlotButton()
    {
        if (uiLoaderPopup.gameObject.activeSelf)
            return;
        uiLoaderPopup.selectNum = slotNumber;
        uiLoaderPopup.gameObject.SetActive(true);
    }
}
