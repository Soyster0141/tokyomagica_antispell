using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// シーン遷移を管理
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    /// <summary>
    /// 指定したシーンに遷移
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// 遅延後にシーンに遷移
    /// </summary>
    public static void LoadSceneWithDelay(string sceneName, float delay, MonoBehaviour caller)
    {
        caller.StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }
    
    private static IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// ゲームを終了
    /// </summary>
    public static void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
