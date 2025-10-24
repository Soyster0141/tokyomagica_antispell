using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// キャラクター選択画面のUIを管理
/// </summary>
public class CharacterSelectionUI : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Transform characterButtonContainer;
    [SerializeField] private GameObject characterButtonPrefab;
    
    [Header("キャラクター情報表示")]
    [SerializeField] private Image characterPreviewImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterHPText;
    [SerializeField] private TextMeshProUGUI characterDescriptionText;
    [SerializeField] private Button confirmButton;
    
    [Header("キャラクターデータ")]
    [SerializeField] private CharacterData[] availableCharacters;
    
    private CharacterData selectedCharacter;
    private int currentPlayerSelecting = 1;
    
    // 選択完了イベント
    public UnityEvent<int, CharacterData> OnCharacterSelected;
    
    void Awake()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
    }
    
    void Start()
    {
        CreateCharacterButtons();
        UpdateInstruction();
        selectionPanel?.SetActive(true);
    }
    
    /// <summary>
    /// キャラクター選択ボタンを生成
    /// </summary>
    void CreateCharacterButtons()
    {
        if (characterButtonContainer == null || characterButtonPrefab == null)
            return;
        
        foreach (var character in availableCharacters)
        {
            GameObject btnObj = Instantiate(characterButtonPrefab, characterButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            
            // ボタンのテキストを設定
            if (btnText != null)
            {
                btnText.text = character.characterName;
            }
            
            // ボタンにキャラクター画像を追加（あれば）
            Image btnImage = btnObj.GetComponent<Image>();
            if (btnImage != null && character.idleSprite != null)
            {
                // ボタンの背景にキャラ画像を設定することもできるが、
                // 今回は別のImageオブジェクトを使うことを前提にしている
                
                // ボタン内の"CharacterImage"という名前のImageを探す
                Transform imageTransform = btnObj.transform.Find("CharacterImage");
                if (imageTransform != null)
                {
                    Image charImage = imageTransform.GetComponent<Image>();
                    if (charImage != null)
                    {
                        charImage.sprite = character.idleSprite;
                        charImage.color = Color.white;
                    }
                }
            }
            
            CharacterData charData = character;
            btn.onClick.AddListener(() => OnCharacterButtonClicked(charData));
        }
    }
    
    /// <summary>
    /// キャラクターボタンがクリックされた時
    /// </summary>
    void OnCharacterButtonClicked(CharacterData character)
    {
        selectedCharacter = character;
        UpdateCharacterInfo();
        
        if (confirmButton != null)
        {
            confirmButton.interactable = true;
        }
    }
    
    /// <summary>
    /// キャラクター情報を更新
    /// </summary>
    void UpdateCharacterInfo()
    {
        if (selectedCharacter == null) return;
        
        // キャラクター画像を表示
        if (characterPreviewImage != null)
        {
            // 待機中の画像を表示
            if (selectedCharacter.idleSprite != null)
            {
                characterPreviewImage.sprite = selectedCharacter.idleSprite;
                characterPreviewImage.color = Color.white;
            }
            else
            {
                // 画像がない場合はプレースホルダー（灰色）
                characterPreviewImage.sprite = null;
                characterPreviewImage.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        
        if (characterNameText != null)
        {
            characterNameText.text = selectedCharacter.characterName;
        }
        
        if (characterHPText != null)
        {
            characterHPText.text = $"HP: {selectedCharacter.maxHP}";
        }
        
        if (characterDescriptionText != null)
        {
            characterDescriptionText.text = selectedCharacter.description;
        }
    }
    
    /// <summary>
    /// 確定ボタンがクリックされた時
    /// </summary>
    void OnConfirmButtonClicked()
    {
        if (selectedCharacter == null) return;
        
        // イベント発火
        OnCharacterSelected?.Invoke(currentPlayerSelecting, selectedCharacter);
        
        // 次のプレイヤーへ
        currentPlayerSelecting++;
        
        if (currentPlayerSelecting <= 2)
        {
            // まだプレイヤー2が選択していない
            ResetSelection();
            UpdateInstruction();
        }
        else
        {
            // 両プレイヤーの選択完了
            selectionPanel?.SetActive(false);
        }
    }
    
    /// <summary>
    /// 選択状態をリセット
    /// </summary>
    void ResetSelection()
    {
        selectedCharacter = null;
        
        // 画像をクリア
        if (characterPreviewImage != null)
        {
            characterPreviewImage.sprite = null;
            characterPreviewImage.color = new Color(0.3f, 0.3f, 0.3f);
        }
        
        if (characterNameText != null)
            characterNameText.text = "キャラクターを選択してください";
        
        if (characterHPText != null)
            characterHPText.text = "";
        
        if (characterDescriptionText != null)
            characterDescriptionText.text = "";
        
        if (confirmButton != null)
            confirmButton.interactable = false;
    }
    
    /// <summary>
    /// 指示テキストを更新
    /// </summary>
    void UpdateInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text = $"Player {currentPlayerSelecting}: キャラクターを選択してください";
        }
    }
}
