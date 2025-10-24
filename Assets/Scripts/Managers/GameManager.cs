using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// ゲームモード
/// </summary>
public enum GameMode
{
    PvP,    // プレイヤー vs プレイヤー
    PvCPU   // プレイヤー vs CPU
}

/// <summary>
/// ゲーム全体を管理するメインマネージャー
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private GameSettings settings;
    
    [Header("CPU設定")]
    [SerializeField] private CPUController cpuController;
    [SerializeField] private CPUDifficulty.Level cpuDifficulty = CPUDifficulty.Level.Normal;
    
    [Header("キャラクター画像")]
    [SerializeField] private CharacterPortrait player1Portrait;
    [SerializeField] private CharacterPortrait player2Portrait;
    
    [Header("演出")]
    [SerializeField] private PerfectDefenseEffect perfectDefenseEffect;
    [SerializeField] private DamageEffect player1DamageEffect;
    [SerializeField] private DamageEffect player2DamageEffect;
    
    [Header("プレイヤー")]
    private PlayerData player1;
    private PlayerData player2;
    private CharacterData player1Character;
    private CharacterData player2Character;
    
    [Header("魔力成長システム")]
    private ManaGrowthSystem player1Mana;
    private ManaGrowthSystem player2Mana;
    
    [Header("ゲーム状態")]
    private GameState currentState;
    private TurnData currentTurn;
    private int turnCounter = 0;
    private float currentTimeLimit;
    private GameMode gameMode = GameMode.PvP;  // デフォルトはPvP
    
    [Header("禁止文字システム")]
    // 各プレイヤーが使用できない文字のリスト
    private string player1BannedChars = "";
    private string player2BannedChars = "";
    
    [Header("イベント")]
    public UnityEvent<GameState> OnGameStateChanged;
    public UnityEvent<TurnData> OnTurnStarted;
    public UnityEvent<int, int> OnDamageDealt; // プレイヤー番号、ダメージ量
    public UnityEvent<int> OnPlayerDefeated; // 敗北したプレイヤー番号
    public UnityEvent<int> OnGameWon; // 勝利したプレイヤー番号
    
    // シングルトン
    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // キャラクター選択を使用する場合はコメントアウト
        // InitializeGame();
    }
    
    /// <summary>
    /// ゲームの初期化（キャラクター選択なし）
    /// </summary>
    public void InitializeGame()
    {
        if (settings == null)
        {
            Debug.LogError("GameSettings が設定されていません！");
            return;
        }
        
        // プレイヤー初期化（デフォルトHP）
        player1 = new PlayerData("Player 1", settings.initialHP, 1);
        player2 = new PlayerData("Player 2", settings.initialHP, 2);
        
        // 初期制限時間設定
        currentTimeLimit = settings.initialTimeLimit;
        
        // 最初のターン開始
        turnCounter = 0;
        ChangeState(GameState.Initialization);
        
        StartCoroutine(StartFirstTurn());
    }
    
    IEnumerator StartFirstTurn()
    {
        yield return new WaitForSeconds(1f);
        if (this != null && gameObject != null)
        {
            StartNewTurn();
        }
    }
    
    /// <summary>
    /// 新しいターンを開始
    /// </summary>
    public void StartNewTurn()
    {
        turnCounter++;
        
        // ターンごとに役割を交代（奇数ターン：P1が作成、偶数ターン：P2が作成）
        int creatorPlayer = (turnCounter % 2 == 1) ? 1 : 2;
        int typerPlayer = (creatorPlayer == 1) ? 2 : 1;
        
        currentTurn = new TurnData(turnCounter, creatorPlayer, typerPlayer, currentTimeLimit);
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"ターン {turnCounter} 開始 - Player {creatorPlayer} が文字列作成");
        }
        
        OnTurnStarted?.Invoke(currentTurn);
        
        // 文字列作成中は詠唱中状態に
        SetPlayerState(currentTurn.creatorPlayerNumber, CharacterState.Casting);
        // タイピングするプレイヤーは待機中
        SetPlayerState(currentTurn.typerPlayerNumber, CharacterState.Idle);
        
        ChangeState(GameState.StringCreation);
    }
    
    /// <summary>
    /// 文字列作成が完了した時
    /// </summary>
    public void OnStringCreated(string createdString)
    {
        currentTurn.createdString = createdString;
        
        // 詠唱完了時に、このプレイヤーの禁止文字をクリア
        ClearBannedChars(currentTurn.creatorPlayerNumber);
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"作成された文字列: {createdString}");
        }
        
        // 魔法詠唱状態に変更（文字列作成したプレイヤー）
        SetPlayerState(currentTurn.creatorPlayerNumber, CharacterState.SpellCast);
        // タイピングするプレイヤーは防御詠唱中
        SetPlayerState(currentTurn.typerPlayerNumber, CharacterState.Casting);
        
        ChangeState(GameState.Typing);
    }
    
    /// <summary>
    /// タイピング結果を処理
    /// </summary>
    public void OnTypingCompleted(string typedString)
    {
        ChangeState(GameState.DamageCalculation);
        
        // 正答率を計算
        int correctChars = CalculateCorrectCharacters(currentTurn.createdString, typedString);
        float accuracy = (float)correctChars / currentTurn.createdString.Length;
        
        // ダメージ計算
        int damage = CalculateDamage(accuracy);
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"正解文字数: {correctChars}/{currentTurn.createdString.Length}");
            Debug.Log($"正答率: {accuracy:P0}");
            Debug.Log($"ダメージ: {damage}");
        }
        
        // ダメージを与える
        PlayerData damagedPlayer = GetPlayerData(currentTurn.typerPlayerNumber);
        damagedPlayer.TakeDamage(damage);
        
        // ダメージエフェクトを再生
        if (damage > 0)
        {
            DamageEffect damageEffect = currentTurn.typerPlayerNumber == 1 ? player1DamageEffect : player2DamageEffect;
            if (damageEffect != null)
            {
                damageEffect.PlayDamageEffect(damage);
            }
        }
        
        OnDamageDealt?.Invoke(currentTurn.typerPlayerNumber, damage);
        
        // 魔力獲得処理（タイピングしたプレイヤー）
        int missCount = currentTurn.createdString.Length - correctChars;
        ManaGrowthSystem typerMana = GetPlayerManaSystem(currentTurn.typerPlayerNumber);
        
        if (typerMana != null)
        {
            int gainedMana = typerMana.GainMana(missCount);
            
            if (settings.showDebugLogs)
            {
                Debug.Log($"魔力獲得: +{gainedMana} (ミス: {missCount}回) - 現在: {typerMana.CurrentMana}");
            }
            
            // レベルアップ判定
            if (typerMana.CanLevelUp())
            {
                typerMana.LevelUp();
                // TODO: レベルアップ演出
                Debug.Log($"Player {currentTurn.typerPlayerNumber} レベルアップ！最大文字数: {typerMana.CurrentMaxStringLength}");
            }
        }
        
        // ゲーム終了判定
        if (!damagedPlayer.IsAlive())
        {
            // 勝敗の状態を設定
            SetPlayerState(currentTurn.typerPlayerNumber, CharacterState.Defeat);
            SetPlayerState(currentTurn.creatorPlayerNumber, CharacterState.Victory);
            
            OnPlayerDefeated?.Invoke(currentTurn.typerPlayerNumber);
            OnGameWon?.Invoke(currentTurn.creatorPlayerNumber);
            ChangeState(GameState.GameOver);
        }
        else
        {
            // 制限時間を減少
            currentTimeLimit = Mathf.Max(settings.minTimeLimit, 
                                        currentTimeLimit - settings.timeDecreasePerTurn);
            
            // ダメージ表示が終わるまで待ってから次のターンへ
            StartCoroutine(TransitionToNextTurn());
        }
    }
    
    IEnumerator TransitionToNextTurn()
    {
        ChangeState(GameState.TurnTransition);
        
        // ダメージ表示用に2秒待つ（ダメージ表示期間）
        yield return new WaitForSeconds(2.0f);
        
        // ダメージ表示が終わったら、次のターンを開始
        // （StartNewTurnでCasting状態に変わる）
        
        if (this != null && gameObject != null)
        {
            StartNewTurn();
        }
    }
    
    /// <summary>
    /// 特定のプレイヤーの状態を設定
    /// </summary>
    private void SetPlayerState(int playerNumber, CharacterState state)
    {
        CharacterPortrait portrait = playerNumber == 1 ? player1Portrait : player2Portrait;
        if (portrait != null)
        {
            portrait.SetState(state);
        }
    }
    
    /// <summary>
    /// 両プレイヤーの状態を更新
    /// </summary>
    private void UpdateCharacterStates(CharacterState state)
    {
        if (player1Portrait != null)
        {
            player1Portrait.SetState(state);
        }
        if (player2Portrait != null)
        {
            player2Portrait.SetState(state);
        }
    }

    
    /// <summary>
    /// 前方一致で正しく入力できた文字数を計算
    /// </summary>
    private int CalculateCorrectCharacters(string original, string typed)
    {
        // 逆順文字列を作成（入力すべき文字列）
        string reversedOriginal = ReverseString(original);
        
        int correctCount = 0;
        int minLength = Mathf.Min(reversedOriginal.Length, typed.Length);
        
        for (int i = 0; i < minLength; i++)
        {
            if (reversedOriginal[i] == typed[i])
            {
                correctCount++;
            }
            else
            {
                break; // 前方一致が途切れたら終了
            }
        }
        
        return correctCount;
    }
    
    /// <summary>
    /// 文字列を逆順にする
    /// </summary>
    private string ReverseString(string str)
    {
        char[] charArray = str.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }
    
    /// <summary>
    /// ダメージを計算
    /// </summary>
    private int CalculateDamage(float accuracy)
    {
        return Mathf.RoundToInt((1f - accuracy) * settings.maxDamage);
    }
    
    /// <summary>
    /// ゲーム状態を変更
    /// </summary>
    private void ChangeState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"ゲーム状態変更: {newState}");
        }
    }
    
    /// <summary>
    /// プレイヤーデータを取得
    /// </summary>
    public PlayerData GetPlayerData(int playerNumber)
    {
        return playerNumber == 1 ? player1 : player2;
    }
    
    public TurnData GetCurrentTurn() => currentTurn;
    public GameState GetCurrentState() => currentState;
    public float GetCurrentTimeLimit() => currentTimeLimit;
    public GameSettings GetSettings() => settings;
    
    /// <summary>
    /// キャラクター選択でゲームを初期化
    /// </summary>
    public void InitializeGameWithCharacters(CharacterData char1, CharacterData char2)
    {
        InitializeGameWithCharacters(char1, char2, GameMode.PvP, CPUDifficulty.Level.Normal);
    }
    
    /// <summary>
    /// キャラクター選択でゲームを初期化（ゲームモード指定）
    /// </summary>
    public void InitializeGameWithCharacters(CharacterData char1, CharacterData char2, GameMode mode, CPUDifficulty.Level difficulty)
    {
        if (settings == null)
        {
            Debug.LogError("GameSettings が設定されていません！");
            return;
        }
        
        // ゲームモードを設定
        gameMode = mode;
        cpuDifficulty = difficulty;
        
        // キャラクターデータを保存
        player1Character = char1;
        player2Character = char2;
        
        // ポートレートにキャラクターを設定
        if (player1Portrait != null)
        {
            player1Portrait.SetCharacter(char1);
        }
        if (player2Portrait != null)
        {
            player2Portrait.SetCharacter(char2);
        }
        
        // プレイヤー初期化（キャラクターのHPを使用）
        player1 = new PlayerData(char1.characterName, char1.maxHP, 1);
        
        if (gameMode == GameMode.PvCPU)
        {
            player2 = new PlayerData("CPU (" + char2.characterName + ")", char2.maxHP, 2);
            
            // CPUを初期化
            if (cpuController != null)
            {
                cpuController.Initialize(char2, settings, difficulty);
            }
            else
            {
                Debug.LogError("CPUController が設定されていません！");
            }
        }
        else
        {
            player2 = new PlayerData(char2.characterName, char2.maxHP, 2);
        }
        
        // 魔力システム初期化
        player1Mana = new ManaGrowthSystem(char1);
        player2Mana = new ManaGrowthSystem(char2);
        
        // 初期制限時間設定
        currentTimeLimit = settings.initialTimeLimit;
        
        // 最初のターン開始
        turnCounter = 0;
        ChangeState(GameState.Initialization);
        
        Debug.Log($"ゲーム開始: {gameMode}, 難易度: {cpuDifficulty}");
        
        StartCoroutine(StartFirstTurn());
    }
    
    /// <summary>
    /// プレイヤーのキャラクターデータを取得
    /// </summary>
    public CharacterData GetPlayerCharacter(int playerNumber)
    {
        return playerNumber == 1 ? player1Character : player2Character;
    }
    
    /// <summary>
    /// プレイヤーの魔力システムを取得
    /// </summary>
    public ManaGrowthSystem GetPlayerManaSystem(int playerNumber)
    {
        return playerNumber == 1 ? player1Mana : player2Mana;
    }
    
    /// <summary>
    /// 現在のゲームモードを取得
    /// </summary>
    public GameMode GetGameMode()
    {
        return gameMode;
    }
    
    /// <summary>
    /// 指定したプレイヤーがCPUかどうか
    /// </summary>
    public bool IsCPUPlayer(int playerNumber)
    {
        return gameMode == GameMode.PvCPU && playerNumber == 2;
    }
    
    /// <summary>
    /// CPUコントローラーを取得
    /// </summary>
    public CPUController GetCPUController()
    {
        return cpuController;
    }
    
    /// <summary>
    /// プレイヤーにダメージを与える
    /// </summary>
    public void ApplyDamage(int playerNumber, int damage)
    {
        PlayerData player = GetPlayerData(playerNumber);
        if (player == null)
        {
            Debug.LogError($"Player {playerNumber} が見つかりません！");
            return;
        }
        
        // ダメージを適用
        player.currentHP = Mathf.Max(0, player.currentHP - damage);
        
        // イベント発火
        OnDamageDealt?.Invoke(playerNumber, damage);
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"Player {playerNumber} が {damage} ダメージを受けました。HP: {player.currentHP}/{player.maxHP}");
        }
        
        // HPが0になったら敗北
        if (player.currentHP <= 0)
        {
            OnPlayerDefeated?.Invoke(playerNumber);
            int winnerNumber = playerNumber == 1 ? 2 : 1;
            OnGameWon?.Invoke(winnerNumber);
            ChangeState(GameState.GameOver);
        }
    }
    
    // ========== 禁止文字システム ==========
    
    /// <summary>
    /// 指定したプレイヤーの禁止文字を設定
    /// </summary>
    public void SetBannedChars(int playerNumber, string chars)
    {
        if (playerNumber == 1)
        {
            player1BannedChars = chars;
        }
        else
        {
            player2BannedChars = chars;
        }
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"[GameManager] Player {playerNumber} の禁止文字を設定: {chars}");
        }
    }
    
    /// <summary>
    /// 指定したプレイヤーの禁止文字を取得
    /// </summary>
    public string GetBannedChars(int playerNumber)
    {
        return playerNumber == 1 ? player1BannedChars : player2BannedChars;
    }
    
    /// <summary>
    /// 指定したプレイヤーの禁止文字をクリア
    /// </summary>
    public void ClearBannedChars(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1BannedChars = "";
        }
        else
        {
            player2BannedChars = "";
        }
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"[GameManager] Player {playerNumber} の禁止文字をクリア");
        }
    }
    
    /// <summary>
    /// 完全防御成功時の処理（相手の文字を禁止）
    /// </summary>
    public void OnPerfectDefense(int defenderPlayerNumber, string defendedString)
    {
        // 相手のプレイヤー番号
        int opponentPlayerNumber = defenderPlayerNumber == 1 ? 2 : 1;
        
        // 防御した文字列の各文字を相手の禁止リストに追加
        SetBannedChars(opponentPlayerNumber, defendedString);
        
        // 完全防御演出を再生
        if (perfectDefenseEffect != null)
        {
            perfectDefenseEffect.PlayEffect(defendedString);
        }
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"[GameManager] 完全防御！Player {opponentPlayerNumber} は次のターン、以下の文字が使用できません: {defendedString}");
        }
    }
}
