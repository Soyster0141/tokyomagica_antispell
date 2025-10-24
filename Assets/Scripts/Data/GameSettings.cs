using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
public class GameSettings : ScriptableObject
{
    [Header("文字列設定")]
    [Tooltip("文字列の最小長")]
    public int minStringLength = 3;
    
    [Tooltip("文字列の最大長")]
    public int maxStringLength = 10;
    
    [Tooltip("使用可能な文字リスト")]
    public string availableCharacters = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポァィゥェォャュョッー";
    
    [Header("時間設定")]
    [Tooltip("初期制限時間（秒）")]
    public float initialTimeLimit = 15f;
    
    [Tooltip("ターンごとの時間減少量（秒）")]
    public float timeDecreasePerTurn = 0.5f;
    
    [Tooltip("最小制限時間（秒）")]
    public float minTimeLimit = 5f;
    
    [Header("HP設定")]
    [Tooltip("初期HP")]
    public int initialHP = 100;
    
    [Tooltip("最大ダメージ（正答率0%の時）")]
    public int maxDamage = 50;
    
    [Header("デバッグ設定")]
    [Tooltip("デバッグログを表示")]
    public bool showDebugLogs = true;
}
