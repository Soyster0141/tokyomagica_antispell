using UnityEngine;

/// <summary>
/// CPUのタイピング性能を計算するクラス
/// </summary>
public class CPUTypingCalculator
{
    private CPUDifficulty difficulty;
    
    public CPUTypingCalculator(CPUDifficulty difficulty)
    {
        this.difficulty = difficulty;
    }
    
    /// <summary>
    /// 最終的な正答率を計算
    /// </summary>
    public float CalculateFinalAccuracy(string targetString, float timeLimit)
    {
        float accuracy = difficulty.baseAccuracy;
        
        // 各要素で補正
        accuracy *= CalculateTimeFactor(timeLimit);
        accuracy *= CalculateLengthFactor(targetString.Length);
        accuracy *= CalculateDifficultyCharFactor(targetString);
        accuracy *= CalculateConsecutiveFactor(targetString);
        
        // 最低値でクランプ
        accuracy = Mathf.Clamp(accuracy, difficulty.minAccuracy, 1.0f);
        
        return accuracy;
    }
    
    /// <summary>
    /// 時間プレッシャー係数を計算
    /// </summary>
    private float CalculateTimeFactor(float timeLimit)
    {
        // 時間が短くなるほど係数が下がる
        float timeStress = Mathf.Max(0, (25f - timeLimit) * difficulty.timeSensitivity);
        float factor = Mathf.Max(difficulty.minAccuracy, 1.0f - timeStress);
        
        return factor;
    }
    
    /// <summary>
    /// 文字数係数を計算（長いほど難しい）
    /// </summary>
    private float CalculateLengthFactor(int length)
    {
        // 3文字を基準として、1文字増えるごとに3%減少
        return Mathf.Max(0.8f, 1.0f - (length - 3) * 0.03f);
    }
    
    /// <summary>
    /// 難しい文字の係数を計算
    /// </summary>
    private float CalculateDifficultyCharFactor(string text)
    {
        int hardCharCount = 0;
        
        // 入力が難しい小文字
        string[] hardCharPatterns = { "ッ", "ャ", "ュ", "ョ", "ァ", "ィ", "ゥ", "ェ", "ォ" };
        
        foreach (string pattern in hardCharPatterns)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].ToString() == pattern)
                {
                    hardCharCount++;
                }
            }
        }
        
        // 1つの難しい文字につき4%減少
        return Mathf.Max(0.85f, 1.0f - hardCharCount * 0.04f);
    }
    
    /// <summary>
    /// 連続文字の係数を計算（連続する同じ文字は打ちやすい）
    /// </summary>
    private float CalculateConsecutiveFactor(string text)
    {
        int consecutiveCount = 0;
        
        for (int i = 1; i < text.Length; i++)
        {
            if (text[i] == text[i - 1])
            {
                consecutiveCount++;
            }
        }
        
        // 連続文字1組につき2%上昇（最大10%）
        return Mathf.Min(1.1f, 1.0f + consecutiveCount * 0.02f);
    }
    
    /// <summary>
    /// 正答率に基づいてタイピング結果を生成
    /// </summary>
    public string SimulateTyping(string targetString, float accuracy)
    {
        // 逆順文字列（実際に入力すべき文字列）
        string reversedTarget = ReverseString(targetString);
        
        // 正答する文字数を計算
        int correctCount = Mathf.FloorToInt(reversedTarget.Length * accuracy);
        correctCount = Mathf.Clamp(correctCount, 0, reversedTarget.Length);
        
        // 正答した部分だけを返す（前方一致）
        return reversedTarget.Substring(0, correctCount);
    }
    
    /// <summary>
    /// 文字列を逆順にする
    /// </summary>
    private string ReverseString(string str)
    {
        char[] charArray = str.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }
}
