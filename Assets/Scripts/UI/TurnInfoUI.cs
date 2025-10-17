using UnityEngine;
using TMPro;

/// <summary>
/// 現在のターン情報を表示するUI
/// </summary>
public class TurnInfoUI : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private TextMeshProUGUI phaseInfoText;
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTurnStarted.AddListener(OnTurnStarted);
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
        }
    }
    
    void OnTurnStarted(TurnData turnData)
    {
        if (turnNumberText != null)
        {
            turnNumberText.text = $"ターン {turnData.turnNumber}";
        }
    }
    
    void OnGameStateChanged(GameState newState)
    {
        if (phaseInfoText == null) return;
        
        switch (newState)
        {
            case GameState.Initialization:
                phaseInfoText.text = "ゲーム準備中...";
                break;
            case GameState.StringCreation:
                phaseInfoText.text = "文字列作成フェーズ";
                break;
            case GameState.Typing:
                phaseInfoText.text = "タイピングフェーズ";
                break;
            case GameState.DamageCalculation:
                phaseInfoText.text = "ダメージ計算中...";
                break;
            case GameState.TurnTransition:
                phaseInfoText.text = "次のターンへ...";
                break;
            case GameState.GameOver:
                phaseInfoText.text = "ゲーム終了";
                break;
        }
    }
}
