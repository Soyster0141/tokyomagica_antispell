using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 完全防御成功時の演出を管理
/// </summary>
public class PerfectDefenseEffect : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject effectPanel;
    [SerializeField] private TextMeshProUGUI perfectDefenseText;
    [SerializeField] private TextMeshProUGUI bannedCharsText;
    [SerializeField] private Image flashImage;
    
    [Header("アニメーション設定")]
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private Color flashColor = new Color(1f, 0.8f, 0f, 0.3f);
    
    [Header("パーティクル")]
    [SerializeField] private ParticleSystem bannedCharsParticles;
    
    void Awake()
    {
        if (effectPanel != null)
        {
            effectPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 完全防御演出を再生
    /// </summary>
    public void PlayEffect(string bannedChars)
    {
        Debug.Log($"[PerfectDefenseEffect] PlayEffect呼び出し - bannedChars: '{bannedChars}'");
        
        if (effectPanel != null)
        {
            Debug.Log($"[PerfectDefenseEffect] effectPanelがnullではありません。コルーチンを開始します。");
            StartCoroutine(PlayEffectCoroutine(bannedChars));
        }
        else
        {
            Debug.LogError("[PerfectDefenseEffect] effectPanelがnullです！");
        }
    }
    
    IEnumerator PlayEffectCoroutine(string bannedChars)
    {
        Debug.Log($"[PerfectDefenseEffect] PlayEffectCoroutine開始 - bannedChars: '{bannedChars}'");
        
        // パネルを表示
        effectPanel.SetActive(true);
        Debug.Log($"[PerfectDefenseEffect] effectPanel.SetActive(true)実行");
        
        // テキストを設定
        if (perfectDefenseText != null)
        {
            perfectDefenseText.text = "Anti Spell!!";
            perfectDefenseText.color = new Color(1f, 1f, 1f, 0f);
            Debug.Log($"[PerfectDefenseEffect] perfectDefenseText設定完了");
        }
        else
        {
            Debug.LogWarning($"[PerfectDefenseEffect] perfectDefenseTextがnullです");
        }
        
        if (bannedCharsText != null)
        {
            bannedCharsText.text = $"「{bannedChars}」の文字を封印！";
            bannedCharsText.color = new Color(1f, 1f, 1f, 0f);
        }
        
        // フラッシュ画像を初期化
        if (flashImage != null)
        {
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        }
        
        // SE再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPerfectDefense();
        }
        
        // パーティクル再生
        if (bannedCharsParticles != null)
        {
            bannedCharsParticles.Play();
        }
        
        // フェードイン
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / fadeInDuration;
            
            if (perfectDefenseText != null)
            {
                perfectDefenseText.color = new Color(1f, 1f, 1f, alpha);
            }
            
            if (bannedCharsText != null)
            {
                bannedCharsText.color = new Color(1f, 1f, 1f, alpha);
            }
            
            if (flashImage != null)
            {
                flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, flashColor.a * alpha);
            }
            
            yield return null;
        }
        
        // 表示を維持
        yield return new WaitForSeconds(displayDuration - fadeInDuration - fadeOutDuration);
        
        // フェードアウト
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeOutDuration);
            
            if (perfectDefenseText != null)
            {
                perfectDefenseText.color = new Color(1f, 1f, 1f, alpha);
            }
            
            if (bannedCharsText != null)
            {
                bannedCharsText.color = new Color(1f, 1f, 1f, alpha);
            }
            
            if (flashImage != null)
            {
                flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, flashColor.a * alpha);
            }
            
            yield return null;
        }
        
        // パネルを非表示
        effectPanel.SetActive(false);
    }
}
