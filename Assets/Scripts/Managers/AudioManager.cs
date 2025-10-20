using UnityEngine;

/// <summary>
/// ゲーム全体のサウンドを管理
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("文字列作成フェーズのSE")]
    [SerializeField] private AudioClip characterButtonClickSE;  // 文字ボタンクリック
    [SerializeField] private AudioClip backspaceButtonClickSE;  // バックスペースクリック
    [SerializeField] private AudioClip confirmButtonClickSE;    // 確定ボタンクリック
    [SerializeField] private AudioClip spellCastSE;             // 呪文詠唱エフェクト
    
    [Header("音量設定")]
    [SerializeField] [Range(0f, 1f)] private float seVolume = 1.0f;
    
    private AudioSource audioSource;
    
    // シングルトン
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // AudioSource コンポーネントを追加
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    
    /// <summary>
    /// SEを再生
    /// </summary>
    private void PlaySE(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, seVolume);
        }
    }
    
    /// <summary>
    /// 文字ボタンクリック音を再生
    /// </summary>
    public void PlayCharacterButtonClick()
    {
        PlaySE(characterButtonClickSE);
    }
    
    /// <summary>
    /// バックスペースボタンクリック音を再生
    /// </summary>
    public void PlayBackspaceButtonClick()
    {
        PlaySE(backspaceButtonClickSE);
    }
    
    /// <summary>
    /// 確定ボタンクリック音を再生
    /// </summary>
    public void PlayConfirmButtonClick()
    {
        PlaySE(confirmButtonClickSE);
    }
    
    /// <summary>
    /// 呪文詠唱エフェクト音を再生
    /// </summary>
    public void PlaySpellCast()
    {
        PlaySE(spellCastSE);
    }
}
