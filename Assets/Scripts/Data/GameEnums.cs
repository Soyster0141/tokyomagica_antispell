/// <summary>
/// ゲームの状態を表す列挙型
/// </summary>
public enum GameState
{
    Initialization,    // 初期化中
    StringCreation,    // 文字列作成フェーズ
    Typing,           // タイピングフェーズ
    DamageCalculation, // ダメージ計算中
    TurnTransition,   // ターン切り替え中
    GameOver          // ゲーム終了
}

/// <summary>
/// ターン情報
/// </summary>
[System.Serializable]
public class TurnData
{
    public int turnNumber;
    public int creatorPlayerNumber; // 文字列を作成するプレイヤー番号
    public int typerPlayerNumber;   // タイピングするプレイヤー番号
    public string createdString;    // 作成された文字列
    public float timeLimit;         // 制限時間
    
    public TurnData(int turn, int creator, int typer, float time)
    {
        turnNumber = turn;
        creatorPlayerNumber = creator;
        typerPlayerNumber = typer;
        timeLimit = time;
        createdString = "";
    }
}
