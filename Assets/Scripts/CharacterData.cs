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
    
    [Header("ステータス")]
    [Tooltip("最大HP")]
    public int maxHP = 100;
    
    [Tooltip("使用可能な文字列（例：アイウエオ...）")]
    [TextArea(5, 10)]
    public string availableCharacters;
    
    [Header("将来の拡張用")]
    [Tooltip("ダメージ倍率（1.0が標準）")]
    public float damageMultiplier = 1.0f;
    
    [Tooltip("制限時間ボーナス（秒）")]
    public float timeBonus = 0f;
    
    [Tooltip("特殊能力の説明（将来使用）")]
    [TextArea(2, 4)]
    public string specialAbility;
    
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
