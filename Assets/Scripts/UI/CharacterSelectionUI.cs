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
            
            if (btnText != null)
            {
                btnText.text = character.characterName;
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
