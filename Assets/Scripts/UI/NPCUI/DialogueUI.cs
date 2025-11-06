using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    public Image silhouette1;     // NPC 이미지
    public Image silhouette2;     // 플레이어 이미지
    [SerializeField] TextMeshProUGUI textbox;
    [SerializeField] TextMeshProUGUI namebox;
    private bool ignoreNextClick = true;
    private bool isClickable = false;
    public Image[] heartImages;
    public GameObject pressE;
    public RectTransform polygon;

    public bool IsTyping => isTyping;

    public GameObject choiceButtonPrefab;    // <== 버튼 프리팹 Inspector 할당
    public Transform choiceButtonParent;     // <== SelectBox 영역 Inspector 할당
    private List<GameObject> choiceButtons = new List<GameObject>();
    private Action<int> onChoiceSelected; //  선택 콜백 저장용
    private bool isTyping = false;

    // 밝고 어두운 색상 미리 준비
    public Color npcBright = Color.white;
    public Color npcDim = new Color(0.4f, 0.4f, 0.4f, 1f);
    public Color playerBright = Color.white;
    public Color playerDim = new Color(0.4f, 0.4f, 0.4f, 1f);
    public System.Action OnDialogueScreenClick;
    private Coroutine typingCoroutine;
    private string fullText;
    private float dialogueSfxLastTime = 0f;
    private float dialogueSfxInterval = 1.0f;

    public void SetDialogue(string speaker, string content)
    {
        textbox.text = content;
        namebox.text = speaker;

        bool isNpcSpeaking = !IsPlayer(speaker);

        // Silhouette 색상 조절
        if (isNpcSpeaking)
        {
            silhouette1.color = npcBright;
            silhouette2.color = playerDim;
        }
        else
        {
            silhouette1.color = npcDim;
            silhouette2.color = playerBright;
        }
        if (pressE != null) pressE.SetActive(false);
        if (polygon != null) polygon.gameObject.SetActive(false);
    }


    bool IsPlayer(string speaker)
    {
        string playerName = PlayerManager.Instance.playerName.ToLower();
        string name = speaker.ToLower();
        return name == playerName || name.Contains("플레이어") || name.Contains("player");
    }
    void OnEnable()
    {
        if (pressE != null) pressE.SetActive(false);
        if (polygon != null) polygon.gameObject.SetActive(false);
        Debug.Log($"[DialogueUI] enabled. SortingOrder: {GetComponentInParent<Canvas>().sortingOrder}, RaycastTarget: {textbox.raycastTarget}");
        ignoreNextClick = true;
        HideChoice();
    }
    void Awake()
    {
        Debug.Log($"heartImages 배열 길이: {heartImages?.Length ?? -1}");
        for (int i = 0; i < (heartImages?.Length ?? 0); i++)
        {
            Debug.Log($"heartImages[{i}]: {heartImages[i]}");
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Clicked! isTyping:{isTyping} isClickable:{isClickable} OnDialogueScreenClick null?:{OnDialogueScreenClick == null}");

            if (ignoreNextClick)
            {
                Debug.Log("ignoreNextClick:true, 리턴");
                ignoreNextClick = false;
                return;
            }

            if (isTyping)
            {
                Debug.Log("isTyping:true, 타이핑 전체 표시");
                CompleteTyping();
                isTyping = false;
                isClickable = true;
                return;
            }

            if (isClickable)
            {
                Debug.Log("isClickable:true, OnDialogueScreenClick invoke");
                OnDialogueScreenClick?.Invoke();
                isClickable = false;
                return;
            }
        }
    }
    public void ShowChoice(string[] choices, Action<int> onChoiceSelected)
    {
        Debug.Log($"ShowChoice: choices length = {(choices == null ? "null" : choices.Length.ToString())}");
        HideChoice();
        this.onChoiceSelected = onChoiceSelected;

        for (int i = 0; i < choices.Length; i++)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceButtonParent);
            choiceButtons.Add(btnObj);
            Debug.Log($"{i}번 버튼 생성: {btnObj != null}");
            var btn = btnObj.GetComponent<Button>();
            var txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log($"{i}번 Button: {btn != null}, TMP: {txt != null}");
            txt.text = choices[i];

            int idx = i;
            btn.onClick.AddListener(() =>
            {
                HideChoice();
                ignoreNextClick = true;
                onChoiceSelected?.Invoke(idx);
            });
        }
    }

    public void HideChoice()
    {
        foreach (var btnObj in choiceButtons)
            Destroy(btnObj);
        choiceButtons.Clear();
    }

    public void SetClickable(bool value)
    {
        StartCoroutine(ClickableDelay(value));
    }

    private IEnumerator ClickableDelay(bool value)
    {
        yield return null; // 한 프레임 무조건 기다림
        isClickable = value;
        //ignoreNextClick = false;
    }

    public void UpdateFriendshipHearts(string npcId)
    {
        Debug.Log($"호감도 갱신용 npcId: {npcId}");
        int favor = NPCAffinity.GetFavor(npcId); // 호감도 0~100

        int heartsToShow = Mathf.CeilToInt(favor / 20f);
        heartsToShow = Mathf.Clamp(heartsToShow, 0, heartImages.Length);

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
            {
                Debug.LogError($"heartImages[{i}]가 null입니다!");
                continue;
            }
            var imageComponent = heartImages[i];
            if (imageComponent == null)
            {
                Debug.LogError($"heartImages[{i}]에 Image 컴포넌트가 없습니다!");
                continue;
            }
            heartImages[i].gameObject.SetActive(true); // 항상 켜짐
            if (i < heartsToShow)
                heartImages[i].color = Color.red; // 채워진 하트 컬러
            else
                heartImages[i].color = Color.gray; // 빈 하트(회색/반투명 등)
        }
    }
    public void SetDialogueGradual(string message, float delay = 0.02f)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        fullText = message;
        isTyping = true;
        Debug.Log("[SetDialogueGradual] 타이핑 시작");
        typingCoroutine = StartCoroutine(TypeText(message, delay));
        if (pressE != null) pressE.SetActive(false);
        if (polygon != null) polygon.gameObject.SetActive(false);
    }

    private IEnumerator TypeText(string message, float delay)
    {
        textbox.text = "";
        bool sfxPlayed = false; // 효과음 재생 여부 체크

        foreach (char c in message)
        {
            textbox.text += c;

            if (!sfxPlayed)
            {
                SFXManager.Instance.PlaySFX(SFXManager.SFXType.Dialogue);
                sfxPlayed = true;
            }
            yield return new WaitForSecondsRealtime(delay);
            if (!isTyping) yield break;
        }
        typingCoroutine = null;
        isTyping = false;
        SFXManager.Instance.StopSFX(SFXManager.SFXType.Dialogue);
        CompleteTyping();
    }

    public void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        textbox.text = fullText;
        isTyping = false;
        SFXManager.Instance.StopSFX(SFXManager.SFXType.Dialogue);
        Debug.Log("타이핑 끝, pressE:" + (pressE != null) + " polygon:" + (polygon != null));
        if (pressE != null) pressE.SetActive(true);
        if (polygon != null) polygon.gameObject.SetActive(true);
    }

    public void SetNpcSprite(Sprite npcSprite)
    {
        if (npcSprite != null)
            silhouette1.sprite = npcSprite;
        else
            silhouette1.sprite = null; // 기본 이미지 적용
    }

    public void FinishTypingImmediately()
    {
        CompleteTyping();
    }
}
