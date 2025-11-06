
using TMPro;

public class UITime : UIBase
{
    public TMP_Text DateText;
    public TMP_Text HourText;
    public TMP_Text MinuteText;

    public void Start()
    {
        if (TimeManager.Instance != null)
            UpdateUI(TimeManager.Instance.currentTime);
        TimeManager.Instance.TimeActions += UpdateUI;
    }

    public void UpdateUI(GameTime time)
    {
        DateText.text = time.date.ToString();
        HourText.text = time.hour.ToString("D2");
        MinuteText.text = time.minute.ToString("D2");
    }

    public void OnDestroy()
    {
        if(TimeManager.Instance != null )
            TimeManager.Instance.TimeActions -= UpdateUI;
    }
}
