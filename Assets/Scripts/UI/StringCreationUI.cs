using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 文字列作成フェーズのUIを管理
/// </summary>
public class StringCreationUI : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject creationPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI currentStringText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Transform characterButtonContainer;
    [SerializeField] private GameObject characterButtonPrefab;
    
    private List<Button> characterButtons = new List<Button>();
    private string currentString = "";
    private GameSettings settings;
    
    void Awake()
    {
        confirmButton?.onClick.AddListener(OnConfirmButtonClicked);
        creationPanel?.SetActive(false);
    }
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            GameManager.Instance.OnTurnStarted.AddListener(OnTurnStarted);
            settings = GameManager.Instance.GetSettings();
        }
    }
    
    void Update()
    {
        // GameManagerの状態を毎フレーム確認
        if (GameManager.Instance != null)
        {
            GameState currentState = GameManager.Instance.GetCurrentState();
            bool shouldBeActive = currentState == GameState.StringCreation;
            
            if (creationPanel != null && creationPanel.activeSelf != shouldBeActive)
            {
                creationPanel.SetActive(shouldBeActive);
                
                if (shouldBeActive && settings != null)
                {
                    TurnData turn = GameManager.Instance.GetCurrentTurn();
                    if (turn != null)
                    {
                        OnTurnStarted(turn);
                    }
                    ResetUI();
                }
            }
        }
    }
    
    void OnGameStateChanged(GameState newState)
    {
        bool shouldBeActive = newState == GameState.StringCreation;
        
        if (creationPanel != null)
        {
            creationPanel.SetActive(shouldBeActive);
            Debug.Log($"StringCreationPanel active set to: {shouldBeActive}");
        }
        else
        {
            Debug.LogError("StringCreationPanel is null!");
        }
        
        if (shouldBeActive)
        {
            ResetUI();
        }
    }
    
    void OnTurnStarted(TurnData turnData)
    {
        if (instructionText != null && settings != null)
        {
            // キャラクター名を表示
            PlayerData player = GameManager.Instance.GetPlayerData(turnData.creatorPlayerNumber);
            string playerName = player != null ? player.playerName : $"Player {turnData.creatorPlayerNumber}";
            
            instructionText.text = $"{playerName}: 文字を選んで文字列を作成してください\n" +
                                  $"（{settings.minStringLength}～{settings.maxStringLength}文字）";
        }
    }
    
    void ResetUI()
    {
        currentString = "";
        UpdateCurrentStringDisplay();
        CreateCharacterButtons();
        UpdateConfirmButton();
    }
    
    void CreateCharacterButtons()
    {
        // 既存のすべての子オブジェクトを削除（ボタンと空白スペースの両方）
        foreach (Transform child in characterButtonContainer)
        {
            Destroy(child.gameObject);
        }
        characterButtons.Clear();
        
        if (settings == null || characterButtonContainer == null || characterButtonPrefab == null)
            return;
        
        // 使用可能文字を取得
        string availableChars = GetAvailableCharactersForCurrentPlayer();
        
        // 完全なレイアウト（アランを基準）
        // 5行 × 17列（小文字2列 + 濁音5列 + 五十音10列）
        
        // 小文字（2列 × 5行）
        string[] smallRows = new string[]
        {
            "ャァ",
            "ュィ",
            "ョゥ",
            "ッェ",
            "　ォ"  // 5行目は1文字だけなので空白
        };
        
        // 濁音・半濁音（5列 × 5行）
        string[] dakuonRows = new string[]
        {
            "パバダザガ",
            "ピビヂジギ",
            "プブヅズグ",
            "ペベデゼゲ",
            "ポボドゾゴ"
        };
        
        // 五十音（10列 × 5行）
        string[] baseRows = new string[]
        {
            "ワラヤマハナタサカア",
            "ヲリ　ミヒニチシキイ",
            "　ルユムフヌツスクウ",
            "　レ　メヘネテセケエ",
            "ンロヨモホノトソコオ"
        };
        
        // 各行を処理
        for (int row = 0; row < 5; row++)
        {
            // 小文字
            foreach (char c in smallRows[row])
            {
                if (c == '　')
                {
                    CreateEmptySpace();
                }
                else
                {
                    CreateCharacterButtonOrDisabled(c, availableChars);
                }
            }
            
            // 濁音・半濁音
            foreach (char c in dakuonRows[row])
            {
                CreateCharacterButtonOrDisabled(c, availableChars);
            }
            
            // 五十音
            foreach (char c in baseRows[row])
            {
                if (c == '　')
                {
                    CreateEmptySpace();
                }
                else
                {
                    CreateCharacterButtonOrDisabled(c, availableChars);
                }
            }
        }
    }
    
    /// <summary>
    /// 文字ボタンを作成（使えない場合は非活性）
    /// </summary>
    void CreateCharacterButtonOrDisabled(char character, string availableChars)
    {
        GameObject btnObj = Instantiate(characterButtonPrefab, characterButtonContainer);
        Button btn = btnObj.GetComponent<Button>();
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        
        if (btnText != null)
        {
            btnText.text = character.ToString();
        }
        
        // 使用可能かチェック
        bool canUse = availableChars.Contains(character.ToString());
        
        if (canUse)
        {
            // 使える場合：通常のボタン
            char c = character;
            btn.onClick.AddListener(() => OnCharacterButtonClicked(c));
            characterButtons.Add(btn);
        }
        else
        {
            // 使えない場合：非活性にする
            btn.interactable = false;
            
            // 色を薄くする（オプション）
            ColorBlock colors = btn.colors;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            btn.colors = colors;
            
            // テキストの色も薄くする
            if (btnText != null)
            {
                btnText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
    }
    
    /// <summary>
    /// 空のスペースを作成
    /// </summary>
    void CreateEmptySpace()
    {
        GameObject emptyObj = new GameObject("EmptySpace");
        emptyObj.transform.SetParent(characterButtonContainer);
        RectTransform rt = emptyObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 60);
        LayoutElement le = emptyObj.AddComponent<LayoutElement>();
        le.preferredWidth = 60;
        le.preferredHeight = 60;
    }
    
    /// <summary>
    /// 現在のプレイヤーが使用できる文字を取得
    /// </summary>
    string GetAvailableCharactersForCurrentPlayer()
    {
        TurnData turn = GameManager.Instance?.GetCurrentTurn();
        if (turn != null)
        {
            CharacterData playerCharacter = GameManager.Instance.GetPlayerCharacter(turn.creatorPlayerNumber);
            if (playerCharacter != null)
            {
                return playerCharacter.availableCharacters;
            }
        }
        
        // デフォルト
        return settings?.availableCharacters ?? "";
    }
    
    void OnCharacterButtonClicked(char character)
    {
        if (settings != null && currentString.Length < settings.maxStringLength)
        {
            currentString += character;
            UpdateCurrentStringDisplay();
            UpdateConfirmButton();
        }
    }
    
    public void OnBackspaceButtonClicked()
    {
        if (currentString.Length > 0)
        {
            currentString = currentString.Substring(0, currentString.Length - 1);
            UpdateCurrentStringDisplay();
            UpdateConfirmButton();
        }
    }
    
    void UpdateCurrentStringDisplay()
    {
        if (currentStringText != null)
        {
            currentStringText.text = string.IsNullOrEmpty(currentString) ? "（文字列）" : currentString;
        }
    }
    
    void UpdateConfirmButton()
    {
        if (confirmButton != null && settings != null)
        {
            confirmButton.interactable = currentString.Length >= settings.minStringLength &&
                                        currentString.Length <= settings.maxStringLength;
        }
    }
    
    void OnConfirmButtonClicked()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(currentString))
        {
            GameManager.Instance.OnStringCreated(currentString);
        }
    }
}
