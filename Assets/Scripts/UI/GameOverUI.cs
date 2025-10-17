using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム終了時の結果表示UI
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameWon.AddListener(OnGameWon);
        }
        
        restartButton?.onClick.AddListener(OnRestartButtonClicked);
        quitButton?.onClick.AddListener(OnQuitButtonClicked);
        
        gameOverPanel?.SetActive(false);
    }
    
    void OnGameWon(int winnerPlayerNumber)
    {
        gameOverPanel?.SetActive(true);
        
        if (resultText != null)
        {
            resultText.text = $"Player {winnerPlayerNumber} の勝利！";
        }
    }
    
    void OnRestartButtonClicked()
    {
        // 現在のシーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
