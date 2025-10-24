using UnityEngine;

/// <summary>
/// キャラクターのデータを保持するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("基本情報")]
    [Tooltip("キャラクター名")]
    public string characterName;
    
    [Tooltip("キャラクターの説明文")]
    [TextArea(3, 5)]
    public string description;
    
    [Header("キャラクター画像")]
    [Tooltip("待機中の画像")]
    public Sprite idleSprite;
    
    [Tooltip("詠唱中の画像")]
    public Sprite castingSprite;
    
    [Tooltip("魔法詠唱時の画像")]
    public Sprite spellCastSprite;
    
    [Tooltip("ダメージ時の画像")]
    public Sprite damageSprite;
    
    [Tooltip("勝利時の画像")]
    public Sprite victorySprite;
    
    [Tooltip("敗北時の画像")]
    public Sprite defeatSprite;
    
    [Header("ステータス")]
    [Tooltip("最大HP")]
    public int maxHP = 100;
    
    [Tooltip("使用可能な文字列（例：アイウエオ...）")]
    [TextArea(5, 10)]
    public string availableCharacters;
    
    [Header("魔力成長システム")]
    [Tooltip("完全成功時の魔力獲得量")]
    public int manaGainPerPerfect = 10;
    
    [Tooltip("ミス1回ごとの魔力減少率（0.2 = 20%減少）")]
    [Range(0f, 1f)]
    public float manaReductionPerMiss = 0.2f;
    
    [Tooltip("レベルアップに必要な魔力")]
    public int manaRequiredForLevelUp = 20;
    
    [Tooltip("詠唱最大文字数（初期値）")]
    public int initialMaxStringLength = 3;
    
    [Tooltip("詠唱最大文字数（最大値）")]
    public int maxStringLengthLimit = 10;
    
    [Header("将来の拡張用")]
    [Tooltip("ダメージ倍率（1.0が標準）")]
    public float damageMultiplier = 1.0f;
    
    [Tooltip("制限時間ボーナス（秒）")]
    public float timeBonus = 0f;
    
    [Tooltip("特殊能力の説明（将来使用）")]
    [TextArea(2, 4)]
    public string specialAbility;
    
    [Header("CPU用固有文字列")]
    [Tooltip("このキャラクターが好んで使う文字列（CPUモード時）")]
    public string[] signatureStrings;

    [Header("相手の入力を覗き見できるか")]
    public bool canPeekOpponentInput = false;
    [Header("覗き見できる文字数")]
    public int peekCharacterCount = 0; 

    /// <summary>
    /// 指定した文字がこのキャラクターで使用可能かチェック
    /// </summary>
    public bool CanUseCharacter(char c)
    {
        return availableCharacters.Contains(c.ToString());
    }
    
    /// <summary>
    /// 使用可能文字をフィルタリングして返す
    /// </summary>
    public string GetFilteredCharacters(string allCharacters)
    {
        string filtered = "";
        foreach (char c in allCharacters)
        {
            if (CanUseCharacter(c))
            {
                filtered += c;
            }
        }
        return filtered;
    }
}
