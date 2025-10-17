using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトル画面を管理
/// </summary>
public class TitleScreen : MonoBehaviour
{
    [Header("ボタン")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    
    [Header("設定")]
    [SerializeField] private string gameSceneName = "GameScene";
    
    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
    }
    
    void OnStartButtonClicked()
    {
        SceneTransitionManager.LoadScene(gameSceneName);
    }
    
    void OnExitButtonClicked()
    {
        SceneTransitionManager.QuitGame();
    }
}
