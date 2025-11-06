
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInputPopup : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button openCheckButton;
    [SerializeField] private GameObject ReturnPopup;

    [SerializeField] private GameObject checkPopup;
    [SerializeField] private TextMeshProUGUI checkText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private string _inputName = "";

    private void Start()
    {
        checkPopup.SetActive(false);
        inputField.characterLimit = 16;

    }
    public void OnClickCheck()
    {
        _inputName = inputField.text.Trim();

        if (string.IsNullOrWhiteSpace(_inputName))
        {
            _inputName = "";
            checkText.text = $"이름이 비어 있습니다. \n기본 이름으로 진행할까요?";
        }
        else
        {
            checkText.text = $"{_inputName}\n이 이름으로 진행할까요?";
        }

        if (checkPopup != null)
        {
            checkPopup.SetActive(true);
        }
    }

    public void OnClickYes()
    {
        AnalyticsManager.SendFunnelStepEvent(1);
        NameInput.Instance.SetName(_inputName);
        SceneChangeManager.Instance.ChangeScene(SceneName.OpeningScene);
        checkPopup.SetActive(false);
    }

    public void OnClickNo()
    {
        checkPopup.SetActive(false);
    }

    public void OnClickPopupOff()
    {
        ReturnPopup.SetActive(false);
    }
    
}
