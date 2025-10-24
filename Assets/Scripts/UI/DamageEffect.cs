using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// ダメージを受けた時のエフェクトを管理
/// </summary>
public class DamageEffect : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private RectTransform characterImageTransform;  // キャラクター画像
    [SerializeField] private Image damageFlashImage;                 // ダメージフラッシュ用の画像
    [SerializeField] private TextMeshProUGUI damageNumberText;       // ダメージ数値
    
    [Header("振動設定")]
    [SerializeField] private float shakeIntensityPerDamage = 0.5f;   // ダメージ1あたりの振動強度
    [SerializeField] private float maxShakeIntensity = 20f;          // 最大振動強度
    [SerializeField] private float shakeDuration = 0.5f;             // 振動時間
    
    [Header("フラッシュ設定")]
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.5f);  // 赤色
    [SerializeField] private float flashDuration = 0.3f;
    
    [Header("ダメージ数値設定")]
    [SerializeField] private float numberMoveDuration = 1.0f;        // 数値が移動する時間
    [SerializeField] private float numberMoveDistance = 50f;         // 数値が移動する距離
    [SerializeField] private Color numberColor = Color.red;
    
    private bool isShaking = false;
    
    void Awake()
    {
        // ダメージフラッシュ画像を初期化
        if (damageFlashImage != null)
        {
            damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
            damageFlashImage.raycastTarget = false;
        }
        
        // ダメージ数値を非表示
        if (damageNumberText != null)
        {
            damageNumberText.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// ダメージエフェクトを再生
    /// </summary>
    public void PlayDamageEffect(int damage)
    {
        if (damage <= 0) return;
        
        Debug.Log($"[DamageEffect] PlayDamageEffect - damage: {damage}");
        
        // 振動エフェクト
        if (characterImageTransform != null && !isShaking)
        {
            StartCoroutine(ShakeEffect(damage));
        }
        
        // フラッシュエフェクト
        if (damageFlashImage != null)
        {
            StartCoroutine(FlashEffect());
        }
        
        // ダメージ数値表示
        if (damageNumberText != null)
        {
            StartCoroutine(ShowDamageNumber(damage));
        }
        
        // SE再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamage();
        }
    }
    
    /// <summary>
    /// 振動エフェクト
    /// </summary>
    IEnumerator ShakeEffect(int damage)
    {
        isShaking = true;
        
        // 現在の位置を取得（振動開始時）
        Vector3 startPosition = characterImageTransform.localPosition;
        
        // ダメージ量に応じて振動強度を計算
        float intensity = Mathf.Min(damage * shakeIntensityPerDamage, maxShakeIntensity);
        
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            // ランダムな方向に振動
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            
            characterImageTransform.localPosition = startPosition + new Vector3(x, y, 0f);
            
            elapsed += Time.deltaTime;
            
            // 徐々に振動を弱める
            intensity = Mathf.Lerp(intensity, 0f, elapsed / shakeDuration);
            
            yield return null;
        }
        
        // 元の位置に戻す（振動開始時の位置）
        characterImageTransform.localPosition = startPosition;
        isShaking = false;
    }
    
    /// <summary>
    /// フラッシュエフェクト
    /// </summary>
    IEnumerator FlashEffect()
    {
        float elapsed = 0f;
        
        // フェードイン
        while (elapsed < flashDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float alpha = (elapsed / (flashDuration / 2f)) * flashColor.a;
            damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }
        
        // フェードアウト
        elapsed = 0f;
        while (elapsed < flashDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float alpha = (1f - (elapsed / (flashDuration / 2f))) * flashColor.a;
            damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }
        
        // 完全に透明にする
        damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }
    
    /// <summary>
    /// ダメージ数値を表示
    /// </summary>
    IEnumerator ShowDamageNumber(int damage)
    {
        damageNumberText.gameObject.SetActive(true);
        damageNumberText.text = $"-{damage}";
        damageNumberText.color = numberColor;
        
        Vector3 startPos = damageNumberText.rectTransform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, numberMoveDistance, 0f);
        
        float elapsed = 0f;
        
        while (elapsed < numberMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / numberMoveDuration;
            
            // 位置を移動
            damageNumberText.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            
            // 徐々に透明にする
            float alpha = 1f - t;
            damageNumberText.color = new Color(numberColor.r, numberColor.g, numberColor.b, alpha);
            
            yield return null;
        }
        
        // 非表示にする
        damageNumberText.gameObject.SetActive(false);
        
        // 位置をリセット
        damageNumberText.rectTransform.localPosition = startPos;
    }
}
