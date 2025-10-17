using UnityEngine;

/// <summary>
/// プレイヤーのデータを管理するクラス
/// </summary>
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int currentHP;
    public int maxHP;
    public int playerNumber; // 1 or 2
    
    public PlayerData(string name, int hp, int number)
    {
        playerName = name;
        currentHP = hp;
        maxHP = hp;
        playerNumber = number;
    }
    
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }
    
    /// <summary>
    /// HPを回復
    /// </summary>
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
    
    /// <summary>
    /// プレイヤーが生存しているか
    /// </summary>
    public bool IsAlive()
    {
        return currentHP > 0;
    }
    
    /// <summary>
    /// HP割合を取得（0.0～1.0）
    /// </summary>
    public float GetHPRatio()
    {
        return (float)currentHP / maxHP;
    }
}
