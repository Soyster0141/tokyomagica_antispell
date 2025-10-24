using UnityEngine;

/// <summary>
/// CPU難易度の設定
/// </summary>
[System.Serializable]
public class CPUDifficulty
{
    public enum Level
    {
        Easy,
        Normal,
        Hard
    }
    
    [Header("基本設定")]
    public Level difficulty;
    
    [Header("攻撃設定（文字列作成）")]
    [Tooltip("最小文字数")]
    public int minStringLength = 2;
    
    [Tooltip("最大文字数")]
    public int maxStringLength = 5;
    
    [Tooltip("固有文字列を使用する確率（0.0～1.0）")]
    [Range(0f, 1f)]
    public float signatureStringProbability = 0.3f;
    
    [Tooltip("思考時間（秒）")]
    public float thinkTime = 1.0f;
    
    [Tooltip("ボタンクリック間隔（秒）")]
    public float buttonClickInterval = 0.3f;
    
    [Header("防御設定（タイピング）")]
    [Tooltip("基本正答率（0.0～1.0）")]
    [Range(0f, 1f)]
    public float baseAccuracy = 0.8f;
    
    [Tooltip("時間プレッシャー感度")]
    public float timeSensitivity = 0.04f;
    
    [Tooltip("最低正答率")]
    [Range(0f, 1f)]
    public float minAccuracy = 0.7f;
    
    /// <summary>
    /// 難易度プリセットを取得
    /// </summary>
    public static CPUDifficulty GetPreset(Level level)
    {
        CPUDifficulty preset = new CPUDifficulty();
        preset.difficulty = level;
        
        switch (level)
        {
            case Level.Easy:
                preset.minStringLength = 2;
                preset.maxStringLength = 5;
                preset.signatureStringProbability = 0.2f;
                preset.thinkTime = 2.0f;
                preset.buttonClickInterval = 0.5f;  // 遅い
                preset.baseAccuracy = 0.6f;
                preset.timeSensitivity = 0.02f;
                preset.minAccuracy = 0.9f;
                break;
                
            case Level.Normal:
                preset.minStringLength = 3;
                preset.maxStringLength = 8;
                preset.signatureStringProbability = 0.3f;
                preset.thinkTime = 1.5f;
                preset.buttonClickInterval = 0.3f;
                preset.baseAccuracy = 0.8f;
                preset.timeSensitivity = 0.04f;
                preset.minAccuracy = 0.8f;
                break;
                
            case Level.Hard:
                preset.minStringLength = 4;
                preset.maxStringLength = 10;
                preset.signatureStringProbability = 0.4f;
                preset.thinkTime = 0.5f;
                preset.buttonClickInterval = 0.15f;  // 速い
                preset.baseAccuracy = 0.95f;
                preset.timeSensitivity = 0.06f;
                preset.minAccuracy = 0.7f;
                break;
        }
        
        return preset;
    }
}
