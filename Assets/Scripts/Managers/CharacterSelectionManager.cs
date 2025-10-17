using UnityEngine;

/// <summary>
/// キャラクター選択からゲーム開始までの流れを管理
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private CharacterSelectionUI selectionUI;
    
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
            GameManager.Instance.InitializeGameWithCharacters(player1SelectedCharacter, player2SelectedCharacter);
        }
        else
        {
            Debug.LogError("GameManager が見つかりません！");
        }
    }
}
