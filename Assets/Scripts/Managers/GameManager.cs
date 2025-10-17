using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// ゲーム全体を管理するメインマネージャー
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private GameSettings settings;
    
    [Header("プレイヤー")]
    private PlayerData player1;
    private PlayerData player2;
    private CharacterData player1Character;
    private CharacterData player2Character;
    
    [Header("ゲーム状態")]
    private GameState currentState;
    private TurnData currentTurn;
    private int turnCounter = 0;
    private float currentTimeLimit;
    
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
        ChangeState(GameState.StringCreation);
    }
    
    /// <summary>
    /// 文字列作成が完了した時
    /// </summary>
    public void OnStringCreated(string createdString)
    {
        currentTurn.createdString = createdString;
        
        if (settings.showDebugLogs)
        {
            Debug.Log($"作成された文字列: {createdString}");
        }
        
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
        
        OnDamageDealt?.Invoke(currentTurn.typerPlayerNumber, damage);
        
        // ゲーム終了判定
        if (!damagedPlayer.IsAlive())
        {
            OnPlayerDefeated?.Invoke(currentTurn.typerPlayerNumber);
            OnGameWon?.Invoke(currentTurn.creatorPlayerNumber);
            ChangeState(GameState.GameOver);
        }
        else
        {
            // 制限時間を減少
            currentTimeLimit = Mathf.Max(settings.minTimeLimit, 
                                        currentTimeLimit - settings.timeDecreasePerTurn);
            
            StartCoroutine(TransitionToNextTurn());
        }
    }
    
    IEnumerator TransitionToNextTurn()
    {
        ChangeState(GameState.TurnTransition);
        yield return new WaitForSeconds(2f);
        if (this != null && gameObject != null)
        {
            StartNewTurn();
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
        if (settings == null)
        {
            Debug.LogError("GameSettings が設定されていません！");
            return;
        }
        
        // キャラクターデータを保存
        player1Character = char1;
        player2Character = char2;
        
        // プレイヤー初期化（キャラクターのHPを使用）
        player1 = new PlayerData(char1.characterName, char1.maxHP, 1);
        player2 = new PlayerData(char2.characterName, char2.maxHP, 2);
        
        // 初期制限時間設定
        currentTimeLimit = settings.initialTimeLimit;
        
        // 最初のターン開始
        turnCounter = 0;
        ChangeState(GameState.Initialization);
        
        StartCoroutine(StartFirstTurn());
    }
    
    /// <summary>
    /// プレイヤーのキャラクターデータを取得
    /// </summary>
    public CharacterData GetPlayerCharacter(int playerNumber)
    {
        return playerNumber == 1 ? player1Character : player2Character;
    }
}
