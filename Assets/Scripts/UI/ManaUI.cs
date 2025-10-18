using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// プレイヤーの魔力ゲージと最大文字数を表示
/// </summary>
public class ManaUI : MonoBehaviour
{
    [Header("プレイヤー1")]
    [SerializeField] private Slider player1ManaBar;
    [SerializeField] private TextMeshProUGUI player1ManaText;
    
    [Header("プレイヤー2")]
    [SerializeField] private Slider player2ManaBar;
    [SerializeField] private TextMeshProUGUI player2ManaText;
    
    [Header("最大文字数表示")]
    [SerializeField] private TextMeshProUGUI maxStringLengthText;
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
        }
    }
    
    void Update()
    {
        // 毎フレーム更新（魔力変化をリアルタイムで反映）
        UpdateManaDisplay();
    }
    
    void OnGameStateChanged(GameState newState)
    {
        // 文字列作成フェーズで最大文字数を更新
        if (newState == GameState.StringCreation)
        {
            UpdateMaxStringLengthDisplay();
        }
    }
    
    /// <summary>
    /// 魔力ゲージを更新
    /// </summary>
    void UpdateManaDisplay()
    {
        if (GameManager.Instance == null) return;
        
        // プレイヤー1
        ManaGrowthSystem p1Mana = GameManager.Instance.GetPlayerManaSystem(1);
        if (p1Mana != null)
        {
            UpdatePlayerManaBar(player1ManaBar, player1ManaText, p1Mana);
        }
        
        // プレイヤー2
        ManaGrowthSystem p2Mana = GameManager.Instance.GetPlayerManaSystem(2);
        if (p2Mana != null)
        {
            UpdatePlayerManaBar(player2ManaBar, player2ManaText, p2Mana);
        }
    }
    
    /// <summary>
    /// 個別プレイヤーの魔力ゲージを更新
    /// </summary>
    void UpdatePlayerManaBar(Slider manaBar, TextMeshProUGUI manaText, ManaGrowthSystem manaSystem)
    {
        if (manaBar == null) return;
        
        CharacterData character = GetCharacterForManaSystem(manaSystem);
        if (character == null) return;
        
        // プログレスバーの値を更新
        manaBar.maxValue = character.manaRequiredForLevelUp;
        manaBar.value = manaSystem.CurrentMana;
        
        // テキスト表示（オプション）
        if (manaText != null)
        {
            manaText.text = $"{manaSystem.CurrentMana}/{character.manaRequiredForLevelUp}";
        }
        
        // 最大レベルに達している場合
        if (manaSystem.IsMaxLevel)
        {
            manaBar.value = manaBar.maxValue;
            if (manaText != null)
            {
                manaText.text = "MAX";
            }
        }
    }
    
    /// <summary>
    /// 最大文字数表示を更新
    /// </summary>
    void UpdateMaxStringLengthDisplay()
    {
        if (maxStringLengthText == null || GameManager.Instance == null) return;
        
        TurnData turn = GameManager.Instance.GetCurrentTurn();
        if (turn == null) return;
        
        ManaGrowthSystem manaSystem = GameManager.Instance.GetPlayerManaSystem(turn.creatorPlayerNumber);
        if (manaSystem == null) return;
        
        CharacterData character = GameManager.Instance.GetPlayerCharacter(turn.creatorPlayerNumber);
        if (character == null) return;
        
        // 「詠唱可能: 4/10文字」の形式で表示
        maxStringLengthText.text = $"詠唱可能: {manaSystem.CurrentMaxStringLength}/{character.maxStringLengthLimit}文字";
    }
    
    /// <summary>
    /// ManaSystemに対応するCharacterDataを取得
    /// </summary>
    CharacterData GetCharacterForManaSystem(ManaGrowthSystem manaSystem)
    {
        if (GameManager.Instance == null) return null;
        
        ManaGrowthSystem p1Mana = GameManager.Instance.GetPlayerManaSystem(1);
        ManaGrowthSystem p2Mana = GameManager.Instance.GetPlayerManaSystem(2);
        
        if (manaSystem == p1Mana)
        {
            return GameManager.Instance.GetPlayerCharacter(1);
        }
        else if (manaSystem == p2Mana)
        {
            return GameManager.Instance.GetPlayerCharacter(2);
        }
        
        return null;
    }
}
