using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクターの状態
/// </summary>
public enum CharacterState
{
    Idle,           // 待機中
    Casting,        // 詠唱中
    SpellCast,      // 魔法詠唱
    Damage,         // ダメージ
    Victory,        // 勝利
    Defeat          // 敗北
}

/// <summary>
/// キャラクター画像を表示・管理するコンポーネント
/// </summary>
public class CharacterPortrait : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private Image portraitImage;
    
    [Header("プレースホルダー色設定")]
    [SerializeField] private Color idleColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color castingColor = new Color(0.6f, 0.8f, 1.0f);
    [SerializeField] private Color spellCastColor = new Color(0.7f, 1.0f, 1.0f);
    [SerializeField] private Color damageColor = new Color(1.0f, 0.3f, 0.3f);
    [SerializeField] private Color victoryColor = new Color(1.0f, 0.9f, 0.3f);
    [SerializeField] private Color defeatColor = new Color(0.3f, 0.3f, 0.3f);
    
    private CharacterData characterData;
    private CharacterState currentState = CharacterState.Idle;
    
    /// <summary>
    /// キャラクターデータを設定
    /// </summary>
    public void SetCharacter(CharacterData character)
    {
        characterData = character;
        Debug.Log($"[CharacterPortrait] キャラクター設定: {(character != null ? character.characterName : "null")}");
        SetState(CharacterState.Idle);
    }
    
    /// <summary>
    /// キャラクターの状態を設定
    /// </summary>
    public void SetState(CharacterState state)
    {
        currentState = state;
        Debug.Log($"[CharacterPortrait] 状態変更: {state}");
        UpdatePortrait();
    }
    
    /// <summary>
    /// 画像を更新
    /// </summary>
    void UpdatePortrait()
    {
        if (portraitImage == null)
        {
            Debug.LogWarning("[CharacterPortrait] portraitImageがnullです！");
            return;
        }
        
        if (characterData == null)
        {
            Debug.LogWarning("[CharacterPortrait] characterDataがnullです！");
            return;
        }
        
        Sprite sprite = GetSpriteForState(currentState);
        
        if (sprite != null)
        {
            // スプライトがある場合は表示
            portraitImage.sprite = sprite;
            portraitImage.color = Color.white;
            Debug.Log($"[CharacterPortrait] スプライト表示: {sprite.name}");
        }
        else
        {
            // スプライトがない場合はプレースホルダー（色付き四角）
            portraitImage.sprite = null;
            Color targetColor = GetColorForState(currentState);
            portraitImage.color = targetColor;
            Debug.Log($"[CharacterPortrait] プレースホルダー表示: {currentState} - 色: {targetColor}");
        }
    }
    
    /// <summary>
    /// 状態に応じたスプライトを取得
    /// </summary>
    Sprite GetSpriteForState(CharacterState state)
    {
        if (characterData == null) return null;
        
        switch (state)
        {
            case CharacterState.Idle:
                return characterData.idleSprite;
            case CharacterState.Casting:
                return characterData.castingSprite;
            case CharacterState.SpellCast:
                return characterData.spellCastSprite;
            case CharacterState.Damage:
                return characterData.damageSprite;
            case CharacterState.Victory:
                return characterData.victorySprite;
            case CharacterState.Defeat:
                return characterData.defeatSprite;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// 状態に応じた色を取得（プレースホルダー用）
    /// </summary>
    Color GetColorForState(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Idle:
                return idleColor;
            case CharacterState.Casting:
                return castingColor;
            case CharacterState.SpellCast:
                return spellCastColor;
            case CharacterState.Damage:
                return damageColor;
            case CharacterState.Victory:
                return victoryColor;
            case CharacterState.Defeat:
                return defeatColor;
            default:
                return Color.white;
        }
    }
    
    /// <summary>
    /// 現在の状態を取得
    /// </summary>
    public CharacterState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// 一時的に状態を変更し、指定時間後に元に戻す
    /// </summary>
    public void SetStateTemporary(CharacterState state, float duration)
    {
        // 一時的な状態に変更
        SetState(state);
        
        // 指定時間後に元の状態には戻さず、そのまま維持
        // （次の状態変更はGameManagerから来る）
    }
}
