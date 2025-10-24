using UnityEngine;
using System.Collections;

/// <summary>
/// CPU AIのメインコントローラー
/// </summary>
public class CPUController : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private CPUDifficulty.Level difficultyLevel = CPUDifficulty.Level.Normal;
    
    private CPUDifficulty difficulty;
    private CPUTypingCalculator typingCalculator;
    private CharacterData cpuCharacter;
    private GameSettings gameSettings;
    
    void Awake()
    {
        // 難易度プリセットを取得
        difficulty = CPUDifficulty.GetPreset(difficultyLevel);
        typingCalculator = new CPUTypingCalculator(difficulty);
    }
    
    /// <summary>
    /// CPUの初期化
    /// </summary>
    public void Initialize(CharacterData character, GameSettings settings, CPUDifficulty.Level level)
    {
        cpuCharacter = character;
        gameSettings = settings;
        difficultyLevel = level;
        
        difficulty = CPUDifficulty.GetPreset(level);
        typingCalculator = new CPUTypingCalculator(difficulty);
        
        Debug.Log($"[CPUController] 初期化完了 - キャラクター: {character.characterName}, 難易度: {level}");
    }
    
    /// <summary>
    /// CPU が文字列を作成する
    /// </summary>
    public IEnumerator CreateString(System.Action<string> onComplete)
    {
        Debug.Log("[CPUController] 文字列作成開始");
        
        // 思考時間
        yield return new WaitForSeconds(difficulty.thinkTime);
        
        string createdString = "";
        
        // 固有文字列を使用するか判定
        if (cpuCharacter.signatureStrings != null && 
            cpuCharacter.signatureStrings.Length > 0 &&
            Random.value < difficulty.signatureStringProbability)
        {
            // 固有文字列からランダムに選択
            createdString = cpuCharacter.signatureStrings[Random.Range(0, cpuCharacter.signatureStrings.Length)];
            Debug.Log($"[CPUController] 固有文字列を使用: {createdString}");
        }
        else
        {
            // ランダムに文字列を生成
            createdString = GenerateRandomString();
            Debug.Log($"[CPUController] ランダム文字列を生成: {createdString}");
        }
        
        onComplete?.Invoke(createdString);
    }
    
    /// <summary>
    /// ランダムな文字列を生成
    /// </summary>
    private string GenerateRandomString()
    {
        if (cpuCharacter == null || string.IsNullOrEmpty(cpuCharacter.availableCharacters))
        {
            Debug.LogError("[CPUController] 使用可能文字が設定されていません");
            return "ア";
        }
        
        // 現在の最大文字数を取得（魔力システム考慮）
        int maxLength = difficulty.maxStringLength;
        if (GameManager.Instance != null)
        {
            ManaGrowthSystem manaSystem = GameManager.Instance.GetPlayerManaSystem(2); // CPU は Player2
            if (manaSystem != null)
            {
                maxLength = Mathf.Min(difficulty.maxStringLength, manaSystem.CurrentMaxStringLength);
            }
        }
        
        // 文字数を決定
        int length = Random.Range(difficulty.minStringLength, maxLength + 1);
        
        // ランダムに文字を選択
        string result = "";
        string availableChars = cpuCharacter.availableCharacters;
        
        for (int i = 0; i < length; i++)
        {
            char randomChar = availableChars[Random.Range(0, availableChars.Length)];
            result += randomChar;
        }
        
        return result;
    }
    
    /// <summary>
    /// CPU がタイピングをシミュレート
    /// </summary>
    public IEnumerator SimulateTyping(string targetString, float timeLimit, System.Action<string> onComplete)
    {
        Debug.Log($"[CPUController] タイピングシミュレーション開始 - 対象: {targetString}");
        
        // タイピング中の演出時間（リアリティのため）
        float typingDuration = Mathf.Min(timeLimit * 0.7f, 2.0f); // 制限時間の70%または2秒
        yield return new WaitForSeconds(typingDuration);
        
        // 正答率を計算
        float accuracy = typingCalculator.CalculateFinalAccuracy(targetString, timeLimit);
        Debug.Log($"[CPUController] 計算された正答率: {accuracy:P1}");
        
        // タイピング結果を生成
        string typedString = typingCalculator.SimulateTyping(targetString, accuracy);
        Debug.Log($"[CPUController] タイピング結果: {typedString}");
        
        onComplete?.Invoke(typedString);
    }
    
    /// <summary>
    /// 難易度を変更
    /// </summary>
    public void SetDifficulty(CPUDifficulty.Level level)
    {
        difficultyLevel = level;
        difficulty = CPUDifficulty.GetPreset(level);
        typingCalculator = new CPUTypingCalculator(difficulty);
        
        Debug.Log($"[CPUController] 難易度変更: {level}");
    }
    
    /// <summary>
    /// 現在の難易度を取得
    /// </summary>
    public CPUDifficulty.Level GetDifficultyLevel()
    {
        return difficultyLevel;
    }

    /// <summary>
    /// CPU が文字列を作成する（コールバック版：1文字ずつUIに通知）
    /// </summary>
    public IEnumerator CreateStringWithCallback(StringCreationUI ui)
    {
        Debug.Log("[CPUController] 文字列作成開始（コールバック版）");

        // 思考時間
        yield return new WaitForSeconds(difficulty.thinkTime);

        string targetString = "";

        //固有文字列からランダムに取得
        string sigString = cpuCharacter.signatureStrings[Random.Range(0, cpuCharacter.signatureStrings.Length)];

        // 固有文字列を使用するか判定
        if (cpuCharacter.signatureStrings != null &&
            cpuCharacter.signatureStrings.Length > 0 &&
            Random.value < difficulty.signatureStringProbability &&
            sigString.Length <= difficulty.maxStringLength)
        {
            // 固有文字列からランダムに選択
            targetString = sigString;
            Debug.Log($"[CPUController] 固有文字列を使用: {targetString}");
        }
        else
        {
            // ランダムに文字列を生成
            targetString = GenerateRandomString();
            Debug.Log($"[CPUController] ランダム文字列を生成: {targetString}");
        }

        // 1文字ずつ追加
        foreach (char c in targetString)
        {
            ui.CPUAddCharacter(c);
            yield return new WaitForSeconds(difficulty.buttonClickInterval);
        }

        Debug.Log($"[CPUController] 文字列作成完了: {targetString}");
    }

}
