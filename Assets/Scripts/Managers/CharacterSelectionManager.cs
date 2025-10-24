using UnityEngine;

/// <summary>
/// キャラクター選択からゲーム開始までの流れを管理
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private CharacterSelectionUI selectionUI;
    
    [Header("CPU設定")]
    [SerializeField] private bool isCPUMode = false;
    [SerializeField] private CPUDifficulty.Level cpuDifficulty = CPUDifficulty.Level.Normal;
    
    private CharacterData player1SelectedCharacter;
    private CharacterData player2SelectedCharacter;
    
    void Start()
    {
        if (selectionUI != null)
        {
            selectionUI.OnCharacterSelected.AddListener(OnPlayerSelectedCharacter);
        }
    }
    
    /// <summary>
    /// プレイヤーがキャラクターを選択した時
    /// </summary>
    void OnPlayerSelectedCharacter(int playerNumber, CharacterData character)
    {
        Debug.Log($"Player {playerNumber} が {character.characterName} を選択しました");
        
        if (playerNumber == 1)
        {
            player1SelectedCharacter = character;
        }
        else if (playerNumber == 2)
        {
            player2SelectedCharacter = character;
        }
        
        // 両プレイヤーが選択完了したらゲーム開始
        if (player1SelectedCharacter != null && player2SelectedCharacter != null)
        {
            StartGame();
        }
    }
    
    /// <summary>
    /// ゲームを開始
    /// </summary>
    void StartGame()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log($"ゲーム開始: {player1SelectedCharacter.characterName} vs {player2SelectedCharacter.characterName}");
            
            // ゲームモードを決定
            GameMode mode = isCPUMode ? GameMode.PvCPU : GameMode.PvP;
            GameManager.Instance.InitializeGameWithCharacters(player1SelectedCharacter, player2SelectedCharacter, mode, cpuDifficulty);
            
            // キャラクター画像をPlayerHPUIに設定
            PlayerHPUI[] hpUIs = FindObjectsOfType<PlayerHPUI>();
            foreach (PlayerHPUI hpUI in hpUIs)
            {
                int playerNumber = hpUI.GetComponent<PlayerHPUI>() != null ? 
                    System.Convert.ToInt32(hpUI.GetType().GetField("playerNumber", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(hpUI)) : 0;
                
                if (playerNumber == 1 && player1SelectedCharacter != null)
                {
                    hpUI.SetCharacter(player1SelectedCharacter);
                }
                else if (playerNumber == 2 && player2SelectedCharacter != null)
                {
                    hpUI.SetCharacter(player2SelectedCharacter);
                }
            }
        }
        else
        {
            Debug.LogError("GameManager が見つかりません！");
        }
    }
}
