using UnityEngine;

/// <summary>
/// プレイヤーの魔力成長システムを管理
/// タイピング成功で魔力を獲得し、レベルアップすると詠唱最大文字数が増加
/// </summary>
public class ManaGrowthSystem
{
    private CharacterData characterData;
    private int currentMana;
    private int currentMaxStringLength;
    
    /// <summary>
    /// 現在の魔力
    /// </summary>
    public int CurrentMana => currentMana;
    
    /// <summary>
    /// 現在の詠唱最大文字数
    /// </summary>
    public int CurrentMaxStringLength => currentMaxStringLength;
    
    /// <summary>
    /// 現在のレベル（初期値3文字 = レベル1）
    /// </summary>
    public int CurrentLevel => currentMaxStringLength - characterData.initialMaxStringLength + 1;
    
    /// <summary>
    /// 最大レベルに到達しているか
    /// </summary>
    public bool IsMaxLevel => currentMaxStringLength >= characterData.maxStringLengthLimit;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ManaGrowthSystem(CharacterData data)
    {
        characterData = data;
        currentMana = 0;
        currentMaxStringLength = data.initialMaxStringLength;
    }
    
    /// <summary>
    /// タイピング成功時に魔力を獲得
    /// </summary>
    /// <param name="missCount">ミスした回数</param>
    /// <returns>獲得した魔力量</returns>
    public int GainMana(int missCount)
    {
        // 魔力計算: 基本獲得量 × (1 - ミス回数 × 減少率)
        float reduction = missCount * characterData.manaReductionPerMiss;
        float gainedMana = characterData.manaGainPerPerfect * (1f - reduction);
        
        // 負の値にはならない
        int finalMana = Mathf.Max(0, Mathf.RoundToInt(gainedMana));
        
        currentMana += finalMana;
        
        Debug.Log($"ManaGrowthSystem: 魔力獲得 +{finalMana} (ミス: {missCount}回) - 現在: {currentMana}/{characterData.manaRequiredForLevelUp}");
        
        return finalMana;
    }
    
    /// <summary>
    /// レベルアップ可能かチェック
    /// </summary>
    public bool CanLevelUp()
    {
        return !IsMaxLevel && currentMana >= characterData.manaRequiredForLevelUp;
    }
    
    /// <summary>
    /// レベルアップを実行
    /// </summary>
    /// <returns>レベルアップに成功したか</returns>
    public bool LevelUp()
    {
        if (!CanLevelUp())
        {
            return false;
        }
        
        // 魔力を消費
        currentMana -= characterData.manaRequiredForLevelUp;
        
        // 最大文字数を増やす
        currentMaxStringLength = Mathf.Min(
            currentMaxStringLength + 1,
            characterData.maxStringLengthLimit
        );
        
        Debug.Log($"ManaGrowthSystem: レベルアップ！ レベル{CurrentLevel} - 最大文字数: {currentMaxStringLength}");
        
        return true;
    }
    
    /// <summary>
    /// 魔力をリセット（デバッグ用）
    /// </summary>
    public void ResetMana()
    {
        currentMana = 0;
        currentMaxStringLength = characterData.initialMaxStringLength;
        Debug.Log("ManaGrowthSystem: 魔力とレベルをリセットしました");
    }
}
