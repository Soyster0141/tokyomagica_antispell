using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// スプラッシュ画面を管理
/// </summary>
public class SplashScreen : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private string nextSceneName = "TitleScene";
    
    [Header("UI要素（オプション）")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    
    void Start()
    {
        StartCoroutine(SplashSequence());
    }
    
    IEnumerator SplashSequence()
    {
        // フェードイン
        if (canvasGroup != null)
        {
            yield return StartCoroutine(FadeIn());
        }
        
        // 表示時間
        yield return new WaitForSeconds(displayDuration);
        
        // フェードアウト
        if (canvasGroup != null)
        {
            yield return StartCoroutine(FadeOut());
        }
        
        // 次のシーンへ
        SceneTransitionManager.LoadScene(nextSceneName);
    }
    
    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        canvasGroup.alpha = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        canvasGroup.alpha = 1f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
    
    void Update()
    {
        // スペースキーやクリックでスキップ
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            SceneTransitionManager.LoadScene(nextSceneName);
        }
    }
}
