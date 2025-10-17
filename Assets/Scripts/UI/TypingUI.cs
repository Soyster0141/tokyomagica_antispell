using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// タイピングフェーズのUIを管理
/// </summary>
public class TypingUI : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject typingPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI targetStringText;
    [SerializeField] private TextMeshProUGUI inputStringText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider timerSlider;
    
    private string targetString = "";
    private string inputString = "";
    private float timeRemaining;
    private float totalTime;
    private bool isTyping = false;
    
    // ローマ字バッファ
    private string romajiBuffer = "";
    
    // ローマ字→カタカナ変換テーブル
    private Dictionary<string, string> romajiToKatakana = new Dictionary<string, string>()
    {
        // 清音
        {"a", "ア"}, {"i", "イ"}, {"u", "ウ"}, {"e", "エ"}, {"o", "オ"},
        {"ka", "カ"}, {"ki", "キ"}, {"ku", "ク"}, {"ke", "ケ"}, {"ko", "コ"},
        {"sa", "サ"}, {"si", "シ"}, {"shi", "シ"}, {"su", "ス"}, {"se", "セ"}, {"so", "ソ"},
        {"ta", "タ"}, {"ti", "チ"}, {"chi", "チ"}, {"tu", "ツ"}, {"tsu", "ツ"}, {"te", "テ"}, {"to", "ト"},
        {"na", "ナ"}, {"ni", "ニ"}, {"nu", "ヌ"}, {"ne", "ネ"}, {"no", "ノ"},
        {"ha", "ハ"}, {"hi", "ヒ"}, {"hu", "フ"}, {"fu", "フ"}, {"he", "ヘ"}, {"ho", "ホ"},
        {"ma", "マ"}, {"mi", "ミ"}, {"mu", "ム"}, {"me", "メ"}, {"mo", "モ"},
        {"ya", "ヤ"}, {"yu", "ユ"}, {"yo", "ヨ"},
        {"ra", "ラ"}, {"ri", "リ"}, {"ru", "ル"}, {"re", "レ"}, {"ro", "ロ"},
        {"wa", "ワ"}, {"wo", "ヲ"},
        // 濁音
        {"ga", "ガ"}, {"gi", "ギ"}, {"gu", "グ"}, {"ge", "ゲ"}, {"go", "ゴ"},
        {"za", "ザ"}, {"zi", "ジ"}, {"ji", "ジ"}, {"zu", "ズ"}, {"ze", "ゼ"}, {"zo", "ゾ"},
        {"da", "ダ"}, {"di", "ヂ"}, {"du", "ヅ"}, {"de", "デ"}, {"do", "ド"},
        {"ba", "バ"}, {"bi", "ビ"}, {"bu", "ブ"}, {"be", "ベ"}, {"bo", "ボ"},
        // 半濁音
        {"pa", "パ"}, {"pi", "ピ"}, {"pu", "プ"}, {"pe", "ペ"}, {"po", "ポ"},
        // 拗音（きゃ、しゃ、ちゃなど）
        {"kya", "キャ"}, {"kyi", "キィ"}, {"kyu", "キュ"}, {"kye", "キェ"}, {"kyo", "キョ"},
        {"sha", "シャ"}, {"shu", "シュ"}, {"she", "シェ"}, {"sho", "ショ"},
        {"sya", "シャ"}, {"syu", "シュ"}, {"sye", "シェ"}, {"syo", "ショ"},
        {"cha", "チャ"}, {"chu", "チュ"}, {"che", "チェ"}, {"cho", "チョ"},
        {"tya", "チャ"}, {"tyu", "チュ"}, {"tye", "チェ"}, {"tyo", "チョ"},
        {"nya", "ニャ"}, {"nyi", "ニィ"}, {"nyu", "ニュ"}, {"nye", "ニェ"}, {"nyo", "ニョ"},
        {"hya", "ヒャ"}, {"hyi", "ヒィ"}, {"hyu", "ヒュ"}, {"hye", "ヒェ"}, {"hyo", "ヒョ"},
        {"mya", "ミャ"}, {"myi", "ミィ"}, {"myu", "ミュ"}, {"mye", "ミェ"}, {"myo", "ミョ"},
        {"rya", "リャ"}, {"ryi", "リィ"}, {"ryu", "リュ"}, {"rye", "リェ"}, {"ryo", "リョ"},
        // 濁音の拗音
        {"gya", "ギャ"}, {"gyi", "ギィ"}, {"gyu", "ギュ"}, {"gye", "ギェ"}, {"gyo", "ギョ"},
        {"ja", "ジャ"}, {"ju", "ジュ"}, {"je", "ジェ"}, {"jo", "ジョ"},
        {"jya", "ジャ"}, {"jyu", "ジュ"}, {"jye", "ジェ"}, {"jyo", "ジョ"},
        {"zya", "ジャ"}, {"zyu", "ジュ"}, {"zye", "ジェ"}, {"zyo", "ジョ"},
        {"dya", "ヂャ"}, {"dyi", "ヂィ"}, {"dyu", "ヂュ"}, {"dye", "ヂェ"}, {"dyo", "ヂョ"},
        {"bya", "ビャ"}, {"byi", "ビィ"}, {"byu", "ビュ"}, {"bye", "ビェ"}, {"byo", "ビョ"},
        // 半濁音の拗音
        {"pya", "ピャ"}, {"pyi", "ピィ"}, {"pyu", "ピュ"}, {"pye", "ピェ"}, {"pyo", "ピョ"},
        // 小さい文字
        {"la", "ァ"}, {"xa", "ァ"}, {"li", "ィ"}, {"xi", "ィ"}, {"lu", "ゥ"}, {"xu", "ゥ"},
        {"le", "ェ"}, {"xe", "ェ"}, {"lo", "ォ"}, {"xo", "ォ"},
        {"lya", "ャ"}, {"xya", "ャ"}, {"lyu", "ュ"}, {"xyu", "ュ"}, {"lyo", "ョ"}, {"xyo", "ョ"},
        {"ltu", "ッ"}, {"xtu", "ッ"}, {"ltsu", "ッ"}, {"xtsu", "ッ"}
    };
    
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
        }
        
        typingPanel?.SetActive(false);
    }
    
    void Update()
    {
        // GameManagerの状態を毎フレーム確認
        if (GameManager.Instance != null)
        {
            GameState currentState = GameManager.Instance.GetCurrentState();
            bool shouldBeActive = currentState == GameState.Typing;
            
            if (typingPanel != null && typingPanel.activeSelf != shouldBeActive)
            {
                typingPanel.SetActive(shouldBeActive);
                
                if (shouldBeActive)
                {
                    StartTypingPhase();
                }
            }
        }
        
        if (isTyping)
        {
            HandleInput();
            UpdateTimer();
        }
    }
    
    void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Typing)
        {
            StartTypingPhase();
        }
        else
        {
            typingPanel?.SetActive(false);
            isTyping = false;
        }
    }
    
    void StartTypingPhase()
    {
        if (GameManager.Instance == null) return;
        
        TurnData turn = GameManager.Instance.GetCurrentTurn();
        targetString = turn.createdString;
        inputString = "";
        romajiBuffer = "";
        totalTime = turn.timeLimit;
        timeRemaining = totalTime;
        
        typingPanel?.SetActive(true);
        isTyping = true;
        
        // 逆順にした文字列を表示
        string reversedTarget = ReverseString(targetString);
        
        if (instructionText != null)
        {
            instructionText.text = $"Player {turn.typerPlayerNumber}: 以下の文字列を逆から入力してください！";
        }
        
        if (targetStringText != null)
        {
            targetStringText.text = targetString;
        }
        
        UpdateInputDisplay();
        UpdateTimerDisplay();
    }
    
    void HandleInput()
    {
        // キーボード入力を処理
        if (Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                // バックスペース
                if (c == '\b')
                {
                    if (inputString.Length > 0)
                    {
                        inputString = inputString.Substring(0, inputString.Length - 1);
                    }
                    else if (romajiBuffer.Length > 0)
                    {
                        romajiBuffer = romajiBuffer.Substring(0, romajiBuffer.Length - 1);
                    }
                }
                // Enter（確定）
                else if (c == '\n' || c == '\r')
                {
                    CompleteTyping();
                    return;
                }
                // 通常の文字入力
                else
                {
                    // ローマ字入力
                    if (IsRomaji(c))
                    {
                        romajiBuffer += c;
                        
                        // ローマ字バッファからカタカナに変換を試みる
                        string katakana = TryConvertRomajiToKatakana();
                        if (!string.IsNullOrEmpty(katakana))
                        {
                            inputString += katakana;
                            romajiBuffer = "";
                        }
                    }
                }
            }
            
            UpdateInputDisplay();
        }
    }
    
    bool IsRomaji(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
    
    string TryConvertRomajiToKatakana()
    {
        string lower = romajiBuffer.ToLower();
        
        // 「n」単独の特殊処理：次の文字が母音でない場合のみ「ン」に変換
        if (lower == "n")
        {
            // バッファが"n"だけの場合は、まだ変換しない（次の入力を待つ）
            return null;
        }
        
        // 「nn」の場合は「ン」に変換
        if (lower.EndsWith("nn"))
        {
            romajiBuffer = romajiBuffer.Substring(0, romajiBuffer.Length - 2);
            return "ン";
        }
        
        // "n"の後に子音が来た場合、"n"を「ン」として確定
        if (lower.Length >= 2 && lower[lower.Length - 2] == 'n')
        {
            char lastChar = lower[lower.Length - 1];
            // 母音以外の文字が来た場合
            if (lastChar != 'a' && lastChar != 'i' && lastChar != 'u' && lastChar != 'e' && lastChar != 'o' && lastChar != 'y')
            {
                // "n"を「ン」に変換して、最後の文字はバッファに残す
                romajiBuffer = romajiBuffer.Substring(romajiBuffer.Length - 1);
                return "ン";
            }
        }
        
        // 長い順に試す（例：「chi」を「c」「h」「i」より優先）
        for (int len = Mathf.Min(lower.Length, 3); len >= 1; len--)
        {
            string sub = lower.Substring(lower.Length - len);
            if (romajiToKatakana.ContainsKey(sub))
            {
                // 変換成功したら、バッファから使用した部分を削除
                romajiBuffer = romajiBuffer.Substring(0, romajiBuffer.Length - len);
                return romajiToKatakana[sub];
            }
        }
        
        return null;
    }
    
    void UpdateInputDisplay()
    {
        if (inputStringText != null)
        {
            string display = inputString;
            if (!string.IsNullOrEmpty(romajiBuffer))
            {
                display += "[" + romajiBuffer + "]";
            }
            inputStringText.text = display + "▌";
        }
    }
    
    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            CompleteTyping();
        }
        
        UpdateTimerDisplay();
    }
    
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = $"残り時間: {timeRemaining:F1}秒";
        }
        
        if (timerSlider != null)
        {
            timerSlider.value = timeRemaining / totalTime;
        }
    }
    
    void CompleteTyping()
    {
        isTyping = false;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTypingCompleted(inputString);
        }
    }
    
    string ReverseString(string str)
    {
        char[] charArray = str.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }
}
