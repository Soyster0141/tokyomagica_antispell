using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ゲームシーンを自動的にセットアップするエディタースクリプト
/// </summary>
public class GameSceneSetup : EditorWindow
{
    [MenuItem("Game/Setup Scene")]
    public static void SetupScene()
    {
        if (EditorUtility.DisplayDialog("シーンセットアップ", 
            "現在のシーンにゲームオブジェクトを自動生成します。よろしいですか？", 
            "はい", "いいえ"))
        {
            CreateGameScene();
            EditorUtility.DisplayDialog("完了", "シーンのセットアップが完了しました！", "OK");
        }
    }
    
    static void CreateGameScene()
    {
        // GameManagerの作成
        CreateGameManager();
        
        // Canvasの作成
        GameObject canvas = CreateCanvas();
        
        // UI要素の作成
        CreateTurnInfoUI(canvas);
        CreatePlayerHPUIs(canvas);
        CreateStringCreationUI(canvas);
        CreateTypingUI(canvas);
        CreateGameOverUI(canvas);
        
        // EventSystemの確認
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        Debug.Log("シーンセットアップ完了！");
    }
    
    static void CreateGameManager()
    {
        GameObject gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        
        Debug.Log("GameManager作成完了");
    }
    
    static GameObject CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        Debug.Log("Canvas作成完了");
        return canvasObj;
    }
    
    static void CreateTurnInfoUI(GameObject canvas)
    {
        GameObject turnInfoObj = new GameObject("TurnInfoUI");
        turnInfoObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rt = turnInfoObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -20);
        rt.sizeDelta = new Vector2(400, 100);
        
        TurnInfoUI turnInfo = turnInfoObj.AddComponent<TurnInfoUI>();
        
        // ターン番号テキスト
        GameObject turnNumberObj = new GameObject("TurnNumber");
        turnNumberObj.transform.SetParent(turnInfoObj.transform, false);
        TextMeshProUGUI turnNumberText = turnNumberObj.AddComponent<TextMeshProUGUI>();
        turnNumberText.text = "ターン 1";
        turnNumberText.fontSize = 32;
        turnNumberText.alignment = TextAlignmentOptions.Center;
        RectTransform turnNumberRT = turnNumberObj.GetComponent<RectTransform>();
        turnNumberRT.anchorMin = new Vector2(0, 0.5f);
        turnNumberRT.anchorMax = new Vector2(1, 1);
        turnNumberRT.sizeDelta = Vector2.zero;
        
        // フェーズ情報テキスト
        GameObject phaseInfoObj = new GameObject("PhaseInfo");
        phaseInfoObj.transform.SetParent(turnInfoObj.transform, false);
        TextMeshProUGUI phaseInfoText = phaseInfoObj.AddComponent<TextMeshProUGUI>();
        phaseInfoText.text = "準備中...";
        phaseInfoText.fontSize = 24;
        phaseInfoText.alignment = TextAlignmentOptions.Center;
        RectTransform phaseInfoRT = phaseInfoObj.GetComponent<RectTransform>();
        phaseInfoRT.anchorMin = new Vector2(0, 0);
        phaseInfoRT.anchorMax = new Vector2(1, 0.5f);
        phaseInfoRT.sizeDelta = Vector2.zero;
        
        // TurnInfoUIにアサイン
        SerializedObject so = new SerializedObject(turnInfo);
        so.FindProperty("turnNumberText").objectReferenceValue = turnNumberText;
        so.FindProperty("phaseInfoText").objectReferenceValue = phaseInfoText;
        so.ApplyModifiedProperties();
        
        Debug.Log("TurnInfoUI作成完了");
    }
    
    static void CreatePlayerHPUIs(GameObject canvas)
    {
        // Player1 HP
        CreatePlayerHPUI(canvas, 1, new Vector2(50, -50));
        
        // Player2 HP
        CreatePlayerHPUI(canvas, 2, new Vector2(-50, -50));
        
        Debug.Log("PlayerHPUI作成完了");
    }
    
    static void CreatePlayerHPUI(GameObject canvas, int playerNumber, Vector2 position)
    {
        GameObject hpUIObj = new GameObject($"Player{playerNumber}HP");
        hpUIObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rt = hpUIObj.AddComponent<RectTransform>();
        rt.anchorMin = playerNumber == 1 ? new Vector2(0, 1) : new Vector2(1, 1);
        rt.anchorMax = playerNumber == 1 ? new Vector2(0, 1) : new Vector2(1, 1);
        rt.pivot = playerNumber == 1 ? new Vector2(0, 1) : new Vector2(1, 1);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(300, 100);
        
        PlayerHPUI hpUI = hpUIObj.AddComponent<PlayerHPUI>();
        
        // プレイヤー名テキスト
        GameObject nameObj = new GameObject("PlayerName");
        nameObj.transform.SetParent(hpUIObj.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = $"Player {playerNumber}";
        nameText.fontSize = 24;
        nameText.alignment = TextAlignmentOptions.Left;
        RectTransform nameRT = nameObj.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0, 0.6f);
        nameRT.anchorMax = new Vector2(1, 1);
        nameRT.sizeDelta = Vector2.zero;
        
        // HPテキスト
        GameObject hpTextObj = new GameObject("HPText");
        hpTextObj.transform.SetParent(hpUIObj.transform, false);
        TextMeshProUGUI hpText = hpTextObj.AddComponent<TextMeshProUGUI>();
        hpText.text = "HP: 100 / 100";
        hpText.fontSize = 20;
        hpText.alignment = TextAlignmentOptions.Left;
        RectTransform hpTextRT = hpTextObj.GetComponent<RectTransform>();
        hpTextRT.anchorMin = new Vector2(0, 0.3f);
        hpTextRT.anchorMax = new Vector2(1, 0.6f);
        hpTextRT.sizeDelta = Vector2.zero;
        
        // HPスライダー
        GameObject sliderObj = new GameObject("HPSlider");
        sliderObj.transform.SetParent(hpUIObj.transform, false);
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRT = sliderObj.GetComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0, 0);
        sliderRT.anchorMax = new Vector2(1, 0.3f);
        sliderRT.sizeDelta = Vector2.zero;
        
        // スライダーの背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform bgRT = bgObj.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;
        
        // スライダーのFill Area
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.sizeDelta = new Vector2(-10, -10);
        
        // スライダーのFill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.green;
        RectTransform fillRT = fillObj.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;
        
        slider.fillRect = fillRT;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        // PlayerHPUIにアサイン
        SerializedObject so = new SerializedObject(hpUI);
        so.FindProperty("playerNumber").intValue = playerNumber;
        so.FindProperty("playerNameText").objectReferenceValue = nameText;
        so.FindProperty("hpText").objectReferenceValue = hpText;
        so.FindProperty("hpSlider").objectReferenceValue = slider;
        so.FindProperty("hpFillImage").objectReferenceValue = fillImage;
        so.ApplyModifiedProperties();
    }
    
    static void CreateStringCreationUI(GameObject canvas)
    {
        GameObject panelObj = new GameObject("StringCreationPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        RectTransform panelRT = panelObj.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.sizeDelta = Vector2.zero;
        
        StringCreationUI creationUI = panelObj.AddComponent<StringCreationUI>();
        
        // 説明文
        GameObject instructionObj = new GameObject("Instruction");
        instructionObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI instructionText = instructionObj.AddComponent<TextMeshProUGUI>();
        instructionText.text = "文字を選んで文字列を作成してください";
        instructionText.fontSize = 32;
        instructionText.alignment = TextAlignmentOptions.Center;
        RectTransform instructionRT = instructionObj.GetComponent<RectTransform>();
        instructionRT.anchorMin = new Vector2(0.1f, 0.8f);
        instructionRT.anchorMax = new Vector2(0.9f, 0.95f);
        instructionRT.sizeDelta = Vector2.zero;
        
        // 現在の文字列表示
        GameObject currentStringObj = new GameObject("CurrentString");
        currentStringObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI currentStringText = currentStringObj.AddComponent<TextMeshProUGUI>();
        currentStringText.text = "（文字列）";
        currentStringText.fontSize = 48;
        currentStringText.alignment = TextAlignmentOptions.Center;
        RectTransform currentStringRT = currentStringObj.GetComponent<RectTransform>();
        currentStringRT.anchorMin = new Vector2(0.1f, 0.65f);
        currentStringRT.anchorMax = new Vector2(0.9f, 0.8f);
        currentStringRT.sizeDelta = Vector2.zero;
        
        // 文字ボタンコンテナ（ScrollView）
        GameObject scrollViewObj = new GameObject("CharacterScrollView");
        scrollViewObj.transform.SetParent(panelObj.transform, false);
        RectTransform scrollViewRT = scrollViewObj.AddComponent<RectTransform>();
        scrollViewRT.anchorMin = new Vector2(0.1f, 0.25f);
        scrollViewRT.anchorMax = new Vector2(0.9f, 0.6f);
        scrollViewRT.sizeDelta = Vector2.zero;
        
        ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
        Image scrollImage = scrollViewObj.AddComponent<Image>();
        scrollImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        
        // Content
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollViewObj.transform, false);
        RectTransform contentRT = contentObj.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(0, 1);
        contentRT.pivot = new Vector2(0, 1);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(800, 300);
        
        GridLayoutGroup grid = contentObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(60, 60);
        grid.spacing = new Vector2(10, 10);
        grid.startAxis = GridLayoutGroup.Axis.Horizontal; // 横方向に並べる
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 10; // 10列固定
        grid.childAlignment = TextAnchor.UpperRight; // 右寄せ
        
        scrollRect.content = contentRT;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        
        // 文字ボタンプレハブ（ダミー）
        GameObject buttonPrefab = CreateCharacterButtonPrefab();
        
        // バックスペースボタン
        GameObject backspaceObj = CreateButton(panelObj, "BackspaceButton", "←削除", new Vector2(0.3f, 0.15f), new Vector2(0.45f, 0.2f));
        Button backspaceBtn = backspaceObj.GetComponent<Button>();
        backspaceBtn.onClick.AddListener(() => {
            if (creationUI != null) creationUI.OnBackspaceButtonClicked();
        });
        
        // 確定ボタン
        GameObject confirmObj = CreateButton(panelObj, "ConfirmButton", "確定", new Vector2(0.55f, 0.15f), new Vector2(0.7f, 0.2f));
        Button confirmBtn = confirmObj.GetComponent<Button>();
        confirmBtn.interactable = false;
        
        // StringCreationUIにアサイン
        SerializedObject so = new SerializedObject(creationUI);
        so.FindProperty("creationPanel").objectReferenceValue = panelObj;
        so.FindProperty("instructionText").objectReferenceValue = instructionText;
        so.FindProperty("currentStringText").objectReferenceValue = currentStringText;
        so.FindProperty("confirmButton").objectReferenceValue = confirmBtn;
        so.FindProperty("characterButtonContainer").objectReferenceValue = contentObj.transform;
        so.FindProperty("characterButtonPrefab").objectReferenceValue = buttonPrefab;
        so.ApplyModifiedProperties();
        
        panelObj.SetActive(false);
        Debug.Log("StringCreationUI作成完了");
    }
    
    static GameObject CreateCharacterButtonPrefab()
    {
        GameObject prefab = new GameObject("CharacterButtonPrefab");
        
        RectTransform rt = prefab.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 60);
        
        Button btn = prefab.AddComponent<Button>();
        Image btnImage = prefab.AddComponent<Image>();
        btnImage.color = new Color(0.3f, 0.3f, 0.8f);
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(prefab.transform, false);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "ア";
        text.fontSize = 32;
        text.alignment = TextAlignmentOptions.Center;
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        
        return prefab;
    }
    
    static void CreateTypingUI(GameObject canvas)
    {
        GameObject panelObj = new GameObject("TypingPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        RectTransform panelRT = panelObj.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.sizeDelta = Vector2.zero;
        
        TypingUI typingUI = panelObj.AddComponent<TypingUI>();
        
        // 説明文
        GameObject instructionObj = new GameObject("Instruction");
        instructionObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI instructionText = instructionObj.AddComponent<TextMeshProUGUI>();
        instructionText.text = "以下の文字列を逆から入力してください！";
        instructionText.fontSize = 32;
        instructionText.alignment = TextAlignmentOptions.Center;
        RectTransform instructionRT = instructionObj.GetComponent<RectTransform>();
        instructionRT.anchorMin = new Vector2(0.1f, 0.8f);
        instructionRT.anchorMax = new Vector2(0.9f, 0.9f);
        instructionRT.sizeDelta = Vector2.zero;
        
        // ターゲット文字列
        GameObject targetObj = new GameObject("TargetString");
        targetObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI targetText = targetObj.AddComponent<TextMeshProUGUI>();
        targetText.text = "アイウエオ";
        targetText.fontSize = 56;
        targetText.alignment = TextAlignmentOptions.Center;
        RectTransform targetRT = targetObj.GetComponent<RectTransform>();
        targetRT.anchorMin = new Vector2(0.2f, 0.65f);
        targetRT.anchorMax = new Vector2(0.8f, 0.8f);
        targetRT.sizeDelta = Vector2.zero;
        
        // 入力文字列
        GameObject inputObj = new GameObject("InputString");
        inputObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI inputText = inputObj.AddComponent<TextMeshProUGUI>();
        inputText.text = "▌";
        inputText.fontSize = 48;
        inputText.alignment = TextAlignmentOptions.Center;
        inputText.color = Color.yellow;
        RectTransform inputRT = inputObj.GetComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0.2f, 0.5f);
        inputRT.anchorMax = new Vector2(0.8f, 0.65f);
        inputRT.sizeDelta = Vector2.zero;
        
        // タイマーテキスト
        GameObject timerTextObj = new GameObject("TimerText");
        timerTextObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI timerText = timerTextObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "残り時間: 15.0秒";
        timerText.fontSize = 32;
        timerText.alignment = TextAlignmentOptions.Center;
        RectTransform timerTextRT = timerTextObj.GetComponent<RectTransform>();
        timerTextRT.anchorMin = new Vector2(0.3f, 0.35f);
        timerTextRT.anchorMax = new Vector2(0.7f, 0.45f);
        timerTextRT.sizeDelta = Vector2.zero;
        
        // タイマースライダー
        GameObject sliderObj = new GameObject("TimerSlider");
        sliderObj.transform.SetParent(panelObj.transform, false);
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRT = sliderObj.GetComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0.2f, 0.25f);
        sliderRT.anchorMax = new Vector2(0.8f, 0.35f);
        sliderRT.sizeDelta = Vector2.zero;
        
        // スライダー背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform bgRT = bgObj.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;
        
        // Fill Area
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRT = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.sizeDelta = new Vector2(-10, -10);
        
        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.cyan;
        RectTransform fillRT = fillObj.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;
        
        slider.fillRect = fillRT;
        slider.maxValue = 1;
        slider.value = 1;
        slider.interactable = false;
        
        // TypingUIにアサイン
        SerializedObject so = new SerializedObject(typingUI);
        so.FindProperty("typingPanel").objectReferenceValue = panelObj;
        so.FindProperty("instructionText").objectReferenceValue = instructionText;
        so.FindProperty("targetStringText").objectReferenceValue = targetText;
        so.FindProperty("inputStringText").objectReferenceValue = inputText;
        so.FindProperty("timerText").objectReferenceValue = timerText;
        so.FindProperty("timerSlider").objectReferenceValue = slider;
        so.ApplyModifiedProperties();
        
        panelObj.SetActive(false);
        Debug.Log("TypingUI作成完了");
    }
    
    static void CreateGameOverUI(GameObject canvas)
    {
        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.95f);
        
        RectTransform panelRT = panelObj.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.sizeDelta = Vector2.zero;
        
        GameOverUI gameOverUI = panelObj.AddComponent<GameOverUI>();
        
        // 結果テキスト
        GameObject resultObj = new GameObject("ResultText");
        resultObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI resultText = resultObj.AddComponent<TextMeshProUGUI>();
        resultText.text = "Player 1 の勝利！";
        resultText.fontSize = 64;
        resultText.alignment = TextAlignmentOptions.Center;
        RectTransform resultRT = resultObj.GetComponent<RectTransform>();
        resultRT.anchorMin = new Vector2(0.2f, 0.6f);
        resultRT.anchorMax = new Vector2(0.8f, 0.8f);
        resultRT.sizeDelta = Vector2.zero;
        
        // リスタートボタン
        GameObject restartObj = CreateButton(panelObj, "RestartButton", "もう一度", new Vector2(0.3f, 0.4f), new Vector2(0.7f, 0.5f));
        Button restartBtn = restartObj.GetComponent<Button>();
        
        // 終了ボタン
        GameObject quitObj = CreateButton(panelObj, "QuitButton", "終了", new Vector2(0.3f, 0.25f), new Vector2(0.7f, 0.35f));
        Button quitBtn = quitObj.GetComponent<Button>();
        
        // GameOverUIにアサイン
        SerializedObject so = new SerializedObject(gameOverUI);
        so.FindProperty("gameOverPanel").objectReferenceValue = panelObj;
        so.FindProperty("resultText").objectReferenceValue = resultText;
        so.FindProperty("restartButton").objectReferenceValue = restartBtn;
        so.FindProperty("quitButton").objectReferenceValue = quitBtn;
        so.ApplyModifiedProperties();
        
        panelObj.SetActive(false);
        Debug.Log("GameOverUI作成完了");
    }
    
    static GameObject CreateButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = Vector2.zero;
        
        Button btn = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.5f, 0.8f);
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = text;
        btnText.fontSize = 32;
        btnText.alignment = TextAlignmentOptions.Center;
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        
        return btnObj;
    }
}
