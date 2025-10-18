using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 文字列作成フェーズの時間制限を管理
/// </summary>
public class StringCreationTimer : MonoBehaviour
{
    [Header("タイマー設定")]
    [SerializeField] private float initialTimeLimit = 30f;  // 初期制限時間
    [SerializeField] private float timeDecreasePerTurn = 3f; // ターンごとの減少量
    [SerializeField] private float minimumTimeLimit = 10f;   // 最小制限時間
    
    [Header("警告設定")]
    [SerializeField] private float warningTime1 = 10f;       // 黄色警告の秒数
    [SerializeField] private float warningTime2 = 5f;        // 赤色警告の秒数
    
    [Header("ペナルティ設定")]
    [SerializeField] private float penaltyPercentage = 0.1f; // ペナルティダメージ（最大HPの10%）
    
    [Header("UI要素")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerProgressBar;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private Image timerPanelBackground;  // 背景パネルの Image
    
    [Header("警告時の色")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.6f, 1f);    // 青
    [SerializeField] private Color warningColor1 = Color.yellow;                // 黄色
    [SerializeField] private Color warningColor2 = Color.red;                   // 赤
    
    // 内部状態
    private float currentTimeLimit;
    private float remainingTime;
    private bool isTimerActive = false;
    private int currentTurnNumber = 0;
    
    // 警告状態
    private bool isInWarning1 = false;
    private bool isInWarning2 = false;
    
    // 点滅用
    private float blinkTimer = 0f;
    private const float BLINK_INTERVAL = 0.5f;
    
    // 震え用
    private float shakeTimer = 0f;
    private const float SHAKE_INTERVAL = 0.1f;
    private Vector3 originalBarPosition;
    
    // イベント
    public event Action OnTimeExpired;
    
    void Awake()
    {
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        
        if (timerProgressBar != null)
        {
            originalBarPosition = timerProgressBar.transform.localPosition;
        }
    }
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            GameManager.Instance.OnTurnStarted.AddListener(OnTurnStarted);
        }
    }
    
    void Update()
    {
        if (!isTimerActive) return;
        
        // 時間を減らす
        remainingTime -= Time.deltaTime;
        
        // 時間切れチェック
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            OnTimerExpired();
        }
        
        // UI更新
        UpdateTimerUI();
        
        // 警告状態の更新
        UpdateWarningState();
    }
    
    void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.StringCreation)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }
    
    void OnTurnStarted(TurnData turnData)
    {
        currentTurnNumber = turnData.turnNumber;
        CalculateTimeLimit();
    }
    
    /// <summary>
    /// ターン数に応じた制限時間を計算
    /// </summary>
    void CalculateTimeLimit()
    {
        // 制限時間 = Max(初期時間 - (ターン数 × 減少量), 最小時間)
        currentTimeLimit = Mathf.Max(
            initialTimeLimit - (currentTurnNumber * timeDecreasePerTurn),
            minimumTimeLimit
        );
        
        Debug.Log($"[StringCreationTimer] Turn {currentTurnNumber}: Time limit = {currentTimeLimit}秒");
    }
    
    /// <summary>
    /// タイマー開始
    /// </summary>
    public void StartTimer()
    {
        remainingTime = currentTimeLimit;
        isTimerActive = true;
        isInWarning1 = false;
        isInWarning2 = false;
        
        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
        }
        
        ResetWarningEffects();
        UpdateTimerUI();
    }
    
    /// <summary>
    /// タイマー停止
    /// </summary>
    public void StopTimer()
    {
        isTimerActive = false;
        
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        
        ResetWarningEffects();
    }
    
    /// <summary>
    /// タイマーUI更新
    /// </summary>
    void UpdateTimerUI()
    {
        // テキスト更新
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = seconds.ToString();
        }
        
        // プログレスバー更新
        if (timerProgressBar != null)
        {
            float fillAmount = remainingTime / currentTimeLimit;
            timerProgressBar.fillAmount = fillAmount;
        }
    }
    
    /// <summary>
    /// 警告状態の更新
    /// </summary>
    void UpdateWarningState()
    {
        // 残り5秒以下（赤色警告）
        if (remainingTime <= warningTime2 && !isInWarning2)
        {
            isInWarning2 = true;
            Debug.Log("[StringCreationTimer] Warning 2: 残り5秒");
        }
        // 残り10秒以下（黄色警告）
        else if (remainingTime <= warningTime1 && !isInWarning1)
        {
            isInWarning1 = true;
            Debug.Log("[StringCreationTimer] Warning 1: 残り10秒");
        }
        
        // 時間に応じてプログレスバーの色を徐々に変化させる
        UpdateProgressBarColor();
        
        // 警告2の演出（点滅＋震え）
        if (isInWarning2)
        {
            UpdateBlinkEffect();
            UpdateShakeEffect();
        }
    }
    
    /// <summary>
    /// プログレスバーの色を時間に応じて徐々に変化
    /// </summary>
    void UpdateProgressBarColor()
    {
        if (timerProgressBar == null) return;
        
        Color targetColor;
        
        if (remainingTime <= warningTime2)
        {
            // 残り5秒以下: 赤色
            targetColor = warningColor2;
        }
        else if (remainingTime <= warningTime1)
        {
            // 残り5～10秒: 黄色から赤色に徐々に変化
            float t = (remainingTime - warningTime2) / (warningTime1 - warningTime2);
            targetColor = Color.Lerp(warningColor2, warningColor1, t);
        }
        else
        {
            // 残り10秒以上: 青色から黄色に徐々に変化
            float t = (remainingTime - warningTime1) / (currentTimeLimit - warningTime1);
            targetColor = Color.Lerp(warningColor1, normalColor, t);
        }
        
        timerProgressBar.color = targetColor;
    }
    

    
    /// <summary>
    /// 点滅エフェクト更新
    /// </summary>
    void UpdateBlinkEffect()
    {
        blinkTimer += Time.deltaTime;
        
        if (blinkTimer >= BLINK_INTERVAL)
        {
            blinkTimer = 0f;
            
            if (timerText != null)
            {
                timerText.enabled = !timerText.enabled;
            }
        }
    }
    
    /// <summary>
    /// 震えエフェクト更新
    /// </summary>
    void UpdateShakeEffect()
    {
        shakeTimer += Time.deltaTime;
        
        if (shakeTimer >= SHAKE_INTERVAL)
        {
            shakeTimer = 0f;
            
            if (timerProgressBar != null)
            {
                float offsetY = UnityEngine.Random.Range(-2f, 2f);
                timerProgressBar.transform.localPosition = originalBarPosition + new Vector3(0, offsetY, 0);
            }
        }
    }
    
    /// <summary>
    /// 警告エフェクトをリセット
    /// </summary>
    void ResetWarningEffects()
    {
        // 色をリセット
        if (timerProgressBar != null)
        {
            timerProgressBar.color = normalColor;
        }
        
        if (timerText != null)
        {
            timerText.color = Color.white;
            timerText.enabled = true;
        }
        
        // 背景色をリセット
        if (timerPanelBackground != null)
        {
            timerPanelBackground.color = new Color(0, 0, 0, 0.5f);
        }
        
        // 位置をリセット
        if (timerProgressBar != null)
        {
            timerProgressBar.transform.localPosition = originalBarPosition;
        }
        
        // タイマーリセット
        blinkTimer = 0f;
        shakeTimer = 0f;
    }
    
    /// <summary>
    /// 時間切れ時の処理
    /// </summary>
    void OnTimerExpired()
    {
        isTimerActive = false;
        
        Debug.Log("[StringCreationTimer] 時間切れ！");
        
        // イベント発火
        OnTimeExpired?.Invoke();
        
        // StringCreationUIに時間切れを通知
        StringCreationUI creationUI = FindObjectOfType<StringCreationUI>();
        if (creationUI != null)
        {
            creationUI.OnTimeExpired();
        }
    }
    
    /// <summary>
    /// ペナルティダメージを計算
    /// </summary>
    public int CalculatePenaltyDamage(int maxHP)
    {
        return Mathf.CeilToInt(maxHP * penaltyPercentage);
    }
    
    /// <summary>
    /// 残り時間を取得
    /// </summary>
    public float GetRemainingTime()
    {
        return remainingTime;
    }
    
    /// <summary>
    /// タイマーが動作中か
    /// </summary>
    public bool IsActive()
    {
        return isTimerActive;
    }
}
