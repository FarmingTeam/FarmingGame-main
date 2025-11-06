
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISaveDataBox : MonoBehaviour
{
    public bool HasSaveData = false;
    [SerializeField] public Toggle toggle;

    [SerializeField] TMP_Text saveDateText;
    [SerializeField] TMP_Text playTimeText;
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text nameText;

    [SerializeField] RawImage screenshot;
    [SerializeField] Outline outline;

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
            playTimeText.text = DateStringBuilder.Append("Day").Append(currentGameInfo.CurrentTime.date.ToString()).Append(" ")
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
            HasSaveData = true;
        }
        else
            HasSaveData = false;

        toggle.isOn = false;
        outline.enabled = false;
    }


    public void Toggle()
    {
        outline.enabled = toggle.isOn;
    }
}
