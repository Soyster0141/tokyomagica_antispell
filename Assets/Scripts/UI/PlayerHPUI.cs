using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// プレイヤーのHPを表示するUI
/// </summary>
public class PlayerHPUI : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private int playerNumber = 1;
    
    [Header("UI要素")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private CharacterPortrait characterPortrait;
    
    [Header("HPバーの色")]
    [SerializeField] private Color highHPColor = Color.green;
    [SerializeField] private Color mediumHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;
    
    private PlayerData playerData;
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            GameManager.Instance.OnDamageDealt.AddListener(OnDamageDealt);
        }
        
        UpdateDisplay();
    }
    
    void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Initialization)
        {
            UpdateDisplay();
        }
    }
    
    void OnDamageDealt(int damagedPlayerNumber, int damage)
    {
        if (damagedPlayerNumber == playerNumber)
        {
            // ダメージが0より大きい場合はダメージ画像を表示
            if (damage > 0 && characterPortrait != null)
            {
                Debug.Log($"[PlayerHPUI] Player {playerNumber} がダメージを受けました！Damage状態を表示します。");
                characterPortrait.SetStateTemporary(CharacterState.Damage, 2.0f);
            }
            // ダメージが0の場合（完全防御）はSpellCast状態に変更
            else if (damage == 0 && characterPortrait != null)
            {
                Debug.Log($"[PlayerHPUI] Player {playerNumber} は完全に防御しました！SpellCast状態に変更します。");
                characterPortrait.SetState(CharacterState.SpellCast);
            }
            
            UpdateDisplay();
        }
    }
    
    void UpdateDisplay()
    {
        if (GameManager.Instance == null) return;
        
        playerData = GameManager.Instance.GetPlayerData(playerNumber);
        
        if (playerData == null) return;
        
        // プレイヤー名表示
        if (playerNameText != null)
        {
            playerNameText.text = playerData.playerName;
        }
        
        // HP数値表示
        if (hpText != null)
        {
            hpText.text = $"HP: {playerData.currentHP} / {playerData.maxHP}";
        }
        
        // HPバー表示
        if (hpSlider != null)
        {
            hpSlider.maxValue = playerData.maxHP;
            hpSlider.value = playerData.currentHP;
        }
        
        // HPバーの色変更
        if (hpFillImage != null)
        {
            float hpRatio = playerData.GetHPRatio();
            
            if (hpRatio > 0.6f)
            {
                hpFillImage.color = highHPColor;
            }
            else if (hpRatio > 0.3f)
            {
                hpFillImage.color = mediumHPColor;
            }
            else
            {
                hpFillImage.color = lowHPColor;
            }
        }
    }
    
    /// <summary>
    /// キャラクターを設定
    /// </summary>
    public void SetCharacter(CharacterData character)
    {
        if (characterPortrait != null)
        {
            characterPortrait.SetCharacter(character);
        }
    }
    
    /// <summary>
    /// キャラクターの状態を設定
    /// </summary>
    public void SetCharacterState(CharacterState state)
    {
        if (characterPortrait != null)
        {
            characterPortrait.SetState(state);
        }
    }
}
