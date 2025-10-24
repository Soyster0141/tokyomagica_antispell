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
    
    [Header("エフェクト")]
    [SerializeField] private SpellCastEffect spellCastEffect;
    
    [Header("タイマー")]
    [SerializeField] private StringCreationTimer creationTimer;
    
    private List<Button> characterButtons = new List<Button>();
    private string currentString = "";
    private string cpuActualString = "";  // CPUが実際に入力している文字列
    private GameSettings settings;
    private bool isCPUCreating = false;  // CPU詠唱中フラグ
    
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
            
            instructionText.text = $"{playerName}: 呪文を詠唱せよ\n" +
                                  $"（{settings.minStringLength}～{settings.maxStringLength}文字）";
        }
        
        // CPUのターンなら自動で文字列作成
        if (GameManager.Instance != null && GameManager.Instance.IsCPUPlayer(turnData.creatorPlayerNumber))
        {
            currentStringText.text = "";
            StartCoroutine(HandleCPUStringCreation());
        }
    }
    
    void ResetUI()
    {
        // CPU詠唱中はリセットしない
        if (isCPUCreating)
        {
            Debug.Log("[StringCreationUI] ResetUI: CPU詠唱中のためスキップ");
            //return;
        }
        else 
        {         
        cpuActualString = "";  // 追加
        currentString = "";
        isCPUCreating = false;  // 追加
        UpdateCurrentStringDisplay();
        UpdateConfirmButton();
        }
        CreateCharacterButtons();
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
            "ーォ"  // 5行目は1文字だけなので空白 // 最後に「ー」を追加
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
        
        // 五十音（10列 × 5行）+ 長音符
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
            string availableChars = playerCharacter != null 
                ? playerCharacter.availableCharacters 
                : (settings?.availableCharacters ?? "");
            
            // 禁止文字を取得して除外
            string bannedChars = GameManager.Instance.GetBannedChars(turn.creatorPlayerNumber);
            
            if (!string.IsNullOrEmpty(bannedChars))
            {
                // 禁止文字を除外した文字列を作成
                string result = "";
                foreach (char c in availableChars)
                {
                    if (!bannedChars.Contains(c.ToString()))
                    {
                        result += c;
                    }
                }
                
                if (settings.showDebugLogs)
                {
                    Debug.Log($"[StringCreationUI] Player {turn.creatorPlayerNumber} の使用可能文字: {result} (禁止: {bannedChars})");
                }
                
                return result;
            }
            
            return availableChars;
        }
        
        // デフォルト
        return settings?.availableCharacters ?? "";
    }
    
    void OnCharacterButtonClicked(char character)
    {
        // CPU詠唱中はボタン入力を受け付けない
        if (isCPUCreating)
        {
            return;
        }

        // SE再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCharacterButtonClick();
        }
        
        // 現在のプレイヤーの最大文字数を取得
        int maxLength = GetCurrentPlayerMaxStringLength();
        
        if (currentString.Length < maxLength)
        {
            currentString += character;
            UpdateCurrentStringDisplay();
            UpdateConfirmButton();
        }
    }
    
    public void OnBackspaceButtonClicked()
    {
        // SE再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackspaceButtonClick();
        }
        
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
            // CPU詠唱中は「X」でマスク（一部見える場合も）
            if (isCPUCreating)
            {
                // 現在のプレイヤーの先読み能力を取得
                int peekCount = GetCurrentPlayerPeekAbility();
                
                string displayText = CreateMaskedString(cpuActualString, peekCount);
                currentStringText.text = string.IsNullOrEmpty(displayText) ? "（呪文）" : displayText;
                
                Debug.Log($"[StringCreationUI] CPU詠唱中表示更新: {displayText} (実際: {cpuActualString}, 先読み: {peekCount}文字)");
            }
            else
            {
                // 通常時は白
                currentStringText.color = Color.white;
                currentStringText.text = string.IsNullOrEmpty(currentString) ? "（呪文）" : currentString;
            }
        }
        else
        {
            Debug.LogError("[StringCreationUI] currentStringText is null!");
        }
    }
    
    /// <summary>
    /// 現在のプレイヤーの先読み能力を取得
    /// </summary>
    int GetCurrentPlayerPeekAbility()
    {
        TurnData turn = GameManager.Instance?.GetCurrentTurn();
        if (turn != null)
        {
            // CPUのターンなら、相手（プレイヤー）の能力を取得
            int opponentPlayerNumber = turn.creatorPlayerNumber == 1 ? 2 : 1;
            CharacterData opponentCharacter = GameManager.Instance.GetPlayerCharacter(opponentPlayerNumber);
            
            if (opponentCharacter != null)
            {
                return opponentCharacter.peekCharacterCount;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// 文字列をマスクする（先頭n文字だけ見える）
    /// </summary>
    string CreateMaskedString(string actualString, int visibleCount)
    {
        if (string.IsNullOrEmpty(actualString))
        {
            return "";
        }
        
        // 先頭からvisibleCount文字だけ見える
        int visible = Mathf.Min(visibleCount, actualString.Length);
        string visiblePart = actualString.Substring(0, visible);
        string maskedPart = new string('X', actualString.Length - visible);
        
        return visiblePart + maskedPart;
    }

    /// <summary>
    /// CPUが文字を1文字追加(実際にボタンを押す演出を行う)
    /// </summary>
    public void CPUAddCharacter(char character)
    {
        Debug.Log($"[StringCreationUI] CPUAddCharacter呼び出し: {character}, isCPUCreating={isCPUCreating}");
        
        if (isCPUCreating)
        {
            // 実際の文字列に追加
            cpuActualString += character;
            Debug.Log($"[StringCreationUI] cpuActualString更新: {cpuActualString}");

            // 該当するボタンを探して、視覚的フィードバックを発火
            Button targetButton = FindButtonForCharacter(character);
            if (targetButton != null)
            {
                // 一時的にボタンを有効化（アニメーション用）
                bool wasInteractable = targetButton.interactable;
                targetButton.interactable = true;
                
                // ボタンの視覚エフェクト（プレスアニメーション）をトリガー
                // Note: onClickは呼ばない（currentStringに追加されてしまうので）
                
                // 元の状態に戻す
                targetButton.interactable = wasInteractable;
            }
            
            // 画面表示を更新（「？」でマスク表示される）
            UpdateCurrentStringDisplay();

            // SE再生
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCharacterButtonClick();
            }
        }
    }

    /// <summary>
    /// 指定した文字のボタンを探す
    /// </summary>
    private Button FindButtonForCharacter(char character)
    {
        foreach (Button btn in characterButtons)
        {
            if (btn != null)
            {
                TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null && btnText.text == character.ToString())
                {
                    return btn;
                }
            }
        }
        return null;
    }

    void UpdateConfirmButton()
    {
        if (confirmButton != null && settings != null)
        {
            int maxLength = GetCurrentPlayerMaxStringLength();
            confirmButton.interactable = currentString.Length >= settings.minStringLength &&
                                        currentString.Length <= maxLength;
        }
    }
    
    /// <summary>
    /// 現在のプレイヤーの最大文字数を取得
    /// </summary>
    int GetCurrentPlayerMaxStringLength()
    {
        TurnData turn = GameManager.Instance?.GetCurrentTurn();
        if (turn != null)
        {
            ManaGrowthSystem manaSystem = GameManager.Instance.GetPlayerManaSystem(turn.creatorPlayerNumber);
            if (manaSystem != null)
            {
                return manaSystem.CurrentMaxStringLength;
            }
        }
        
        // デフォルト
        return settings?.maxStringLength ?? 10;
    }
    
    void OnConfirmButtonClicked()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(currentString))
        {
            // SE再生
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayConfirmButtonClick();
            }
            
            // 詠唱エフェクトを再生
            if (spellCastEffect != null && currentStringText != null)
            {
                TurnData turn = GameManager.Instance.GetCurrentTurn();
                if (turn != null)
                {
                    PlayerData player = GameManager.Instance.GetPlayerData(turn.creatorPlayerNumber);
                    string playerName = player != null ? player.playerName : "";
                    spellCastEffect.PlayCastEffect(playerName, currentStringText.transform.position);
                    
                    // 詠唱エフェクトSE再生
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySpellCast();
                    }
                    
                    // 魔法詠唱画像を一時的に表示
                    PlayerHPUI[] hpUIs = FindObjectsOfType<PlayerHPUI>();
                    foreach (PlayerHPUI hpUI in hpUIs)
                    {
                        hpUI.SetCharacterState(CharacterState.SpellCast);
                    }
                }
            }
            
            // タイマーを停止
            if (creationTimer != null)
            {
                creationTimer.StopTimer();
            }
            
            GameManager.Instance.OnStringCreated(currentString);
        }
    }
    
    /// <summary>
    /// 時間切れ時の処理
    /// </summary>
    public void OnTimeExpired()
    {
        Debug.Log("[StringCreationUI] 時間切れ処理開始");
        
        if (GameManager.Instance == null) return;
        
        TurnData turn = GameManager.Instance.GetCurrentTurn();
        if (turn == null) return;
        
        // 1文字以上入力済みの場合：その文字列で確定
        if (!string.IsNullOrEmpty(currentString))
        {
            Debug.Log($"[StringCreationUI] 時間切れ - 入力済み文字列で確定: {currentString}");
            
            // 詠唱エフェクトを再生
            if (spellCastEffect != null && currentStringText != null)
            {
                PlayerData player = GameManager.Instance.GetPlayerData(turn.creatorPlayerNumber);
                string playerName = player != null ? player.playerName : "";
                spellCastEffect.PlayCastEffect(playerName, currentStringText.transform.position);
                
                // 詠唱エフェクトSE再生
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySpellCast();
                }
            }
            
            GameManager.Instance.OnStringCreated(currentString);
        }
        // 0文字（空）の場合：ペナルティ
        else
        {
            Debug.Log("[StringCreationUI] 時間切れ - 空文字列でペナルティ");
            
            // ペナルティダメージを計算
            PlayerData player = GameManager.Instance.GetPlayerData(turn.creatorPlayerNumber);
            if (player != null && creationTimer != null)
            {
                int penaltyDamage = creationTimer.CalculatePenaltyDamage(player.maxHP);
                Debug.Log($"[StringCreationUI] ペナルティダメージ: {penaltyDamage}");
                
                // ダメージを与える
                GameManager.Instance.ApplyDamage(turn.creatorPlayerNumber, penaltyDamage);
            }
            
            // 空文字列で次のフェーズへ（タイピングフェーズはスキップされる）
            GameManager.Instance.OnStringCreated("");
        }
    }

    /// <summary>
    /// CPUの文字列作成を処理
    /// </summary>
    System.Collections.IEnumerator HandleCPUStringCreation()
    {
        Debug.Log("[StringCreationUI] HandleCPUStringCreation開始");
        
        // CPU詠唱中フラグON
        isCPUCreating = true;
        cpuActualString = "";  // リセット

        Debug.Log($"[StringCreationUI] isCPUCreatingをTrueに設定: {isCPUCreating}");

        // ボタンを無効化
        foreach (var btn in characterButtons)
        {
            if (btn != null) btn.interactable = false;
        }
        if (confirmButton != null) confirmButton.interactable = false;

        // CPUに文字列を作成させる（1文字ずつ）
        CPUController cpu = GameManager.Instance.GetCPUController();

        if (cpu != null)
        {
            Debug.Log("[StringCreationUI] CreateStringWithCallback呼び出し前");
            yield return cpu.CreateStringWithCallback(this);
            Debug.Log("[StringCreationUI] CreateStringWithCallback完了");
        }

        // アニメーションを停止
        isCPUCreating = false;
        Debug.Log($"[StringCreationUI] isCPUCreatingをFalseに設定: {isCPUCreating}");

        // 実際に作成された文字列を currentString に設定
        currentString = cpuActualString;

        // タイマーを停止
        if (creationTimer != null)
        {
            creationTimer.StopTimer();
        }

        // エフェクトを再生
        if (spellCastEffect != null && currentStringText != null)
        {
            TurnData turn = GameManager.Instance.GetCurrentTurn();
            if (turn != null)
            {
                PlayerData player = GameManager.Instance.GetPlayerData(turn.creatorPlayerNumber);
                string playerName = player != null ? player.playerName : "";
                spellCastEffect.PlayCastEffect(playerName, currentStringText.transform.position);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySpellCast();
                }
            }
        }

        // 文字列作成完了
        GameManager.Instance.OnStringCreated(currentString);
    }

}
