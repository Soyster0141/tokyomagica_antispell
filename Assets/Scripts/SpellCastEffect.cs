using UnityEngine;

/// <summary>
/// 呪文詠唱エフェクトを管理
/// </summary>
public class SpellCastEffect : MonoBehaviour
{
    [Header("パーティクルシステム")]
    [SerializeField] private ParticleSystem castParticles;
    
    [Header("キャラクターごとの色")]
    [SerializeField] private Color alanColor = new Color(0.3f, 0.5f, 1f); // 青
    [SerializeField] private Color uraraColor = new Color(0.3f, 1f, 0.5f); // 緑
    [SerializeField] private Color irumaColor = new Color(1f, 0.3f, 0.5f); // 赤
    
    void Start()
    {
        if (castParticles != null)
        {
            castParticles.Stop();
        }
    }
    
    /// <summary>
    /// 詠唱エフェクトを再生
    /// </summary>
    public void PlayCastEffect(string characterName, Vector3 position)
    {
        if (castParticles == null)
        {
            Debug.LogWarning("SpellCastEffect: castParticlesがnullです！");
            return;
        }
        
        Debug.Log($"SpellCastEffect: エフェクト再生 - {characterName} at {position}");
        
        // キャラクターに応じた色を設定
        Color effectColor = GetCharacterColor(characterName);
        Debug.Log($"SpellCastEffect: 色 = {effectColor}");
        
        var main = castParticles.main;
        main.startColor = effectColor;
        
        // 位置は初期位置のまま使用（Z: -10に設定済み）
        
        // エフェクト再生
        castParticles.Play();
        Debug.Log("SpellCastEffect: Play()呼び出し完了");
    }
    
    /// <summary>
    /// キャラクター名から色を取得
    /// </summary>
    Color GetCharacterColor(string characterName)
    {
        if (characterName.Contains("アラン"))
            return alanColor;
        else if (characterName.Contains("ウララ"))
            return uraraColor;
        else if (characterName.Contains("イルマ"))
            return irumaColor;
        else
            return Color.white;
    }
    
    /// <summary>
    /// パーティクルシステムを設定
    /// </summary>
    public void SetParticleSystem(ParticleSystem particles)
    {
        castParticles = particles;
    }
}
