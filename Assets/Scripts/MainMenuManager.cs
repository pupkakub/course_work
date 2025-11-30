using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Reflection;


public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Назва сцени, з якої починається нова гра")]
    public string firstGameSceneName = "MainScene";

    [Header("UI References")]
    public GameObject mainMenuCanvas;

    [Header("Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button quitButton;


    // захист проти подвійного завантаження
    private bool isLoading = false;

    private void Awake()
    {
        Debug.Log("[MainMenuManager] ========== AWAKE ==========");
        Time.timeScale = 1f;
        DebugCurrentState();
    }

    private void Start()
    {
        Debug.Log("[MainMenuManager] ========== START ==========");

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);

        SetupButtons();
        UpdateContinueButtonVisibility();
    }

    private void DebugCurrentState()
    {
        Debug.Log("========== ДІАГНОСТИКА СТАНУ ==========");

        if (GameStateManager.Instance != null)
        {
            Debug.Log($"[ДІАГНОСТИКА] GameStateManager існує");
            Debug.Log($"[ДІАГНОСТИКА] Cat Quest: {GameStateManager.Instance.IsCatSequenceCompleted()}");
            Debug.Log($"[ДІАГНОСТИКА] Note Collected: {GameStateManager.Instance.IsNoteCollected()}");
        }
        else
        {
            Debug.Log($"[ДІАГНОСТИКА] GameStateManager == NULL");
        }

        if (GameSaveSystem.Instance != null)
        {
            Debug.Log($"[ДІАГНОСТИКА] GameSaveSystem існує");
            Debug.Log($"[ДІАГНОСТИКА] Has Save File: {GameSaveSystem.Instance.HasSaveFile()}");
            Debug.Log($"[ДІАГНОСТИКА] Inventory Count: {GameSaveSystem.Instance.GetInventoryItems().Count}");
        }
        else
        {
            Debug.Log($"[ДІАГНОСТИКА] GameSaveSystem ще не створено (нормально)");
        }

        int isNewGame = PlayerPrefs.GetInt("IsNewGame", -999);
        int firstLoad = PlayerPrefs.GetInt("FirstLoadAfterNewGame", -999);
        Debug.Log($"[ДІАГНОСТИКА] PlayerPrefs IsNewGame: {isNewGame}");
        Debug.Log($"[ДІАГНОСТИКА] PlayerPrefs FirstLoadAfterNewGame: {firstLoad}");

    }

    private void SetupButtons()
    {
        Debug.Log("[MainMenuManager] Налаштування кнопок...");

        if (newGameButton == null)
            newGameButton = FindButtonByName("NewGameButton");

        if (continueButton == null)
            continueButton = FindButtonByName("ContinueButton");

        if (settingsButton == null)
            settingsButton = FindButtonByName("SettingsButton");

        if (quitButton == null)
            quitButton = FindButtonByName("QuitButton");

        // повністю скинути onClick (включно з persistent / інспекторними)
        if (newGameButton != null)
        {
            newGameButton.onClick = new Button.ButtonClickedEvent(); // повністю очистити всі listeners (runtime + persistent)
            newGameButton.onClick.AddListener(NewGame);
            Debug.Log("[MainMenuManager] NewGameButton підключено (listeners очищено)");
        }
        else
        {
            Debug.LogError("[MainMenuManager] NewGameButton не знайдено!");
        }

        if (continueButton != null)
        {
            continueButton.onClick = new Button.ButtonClickedEvent();
            continueButton.onClick.AddListener(ContinueGame);
            Debug.Log("[MainMenuManager] ContinueButton підключено (listeners очищено)");
        }

        if (settingsButton != null)
        {
            settingsButton.onClick = new Button.ButtonClickedEvent();
            settingsButton.onClick.AddListener(OpenSettings);
            Debug.Log("[MainMenuManager] SettingsButton підключено (listeners очищено)");
        }

        if (quitButton != null)
        {
            quitButton.onClick = new Button.ButtonClickedEvent();
            quitButton.onClick.AddListener(QuitGame);
            Debug.Log("[MainMenuManager] QuitButton підключено (listeners очищено)");
        }
    }

    private Button FindButtonByName(string name)
    {
        var go = GameObject.Find(name);
        if (go != null)
        {
            Debug.Log($"[MainMenuManager] Знайдено кнопку: {name}");
            return go.GetComponent<Button>();
        }
        Debug.LogWarning($"[MainMenuManager] Кнопку '{name}' не знайдено!");
        return null;
    }

    private void UpdateContinueButtonVisibility()
    {
        if (continueButton != null)
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
            bool hasSave = File.Exists(saveFilePath);

            continueButton.gameObject.SetActive(hasSave);

            Debug.Log($"[MainMenuManager] Кнопка Continue: {(hasSave ? "ПОКАЗАНА" : "ПРИХОВАНА")}");
            Debug.Log($"[MainMenuManager] Шлях до файлу: {saveFilePath}");
        }
    }

    // очищення статичних даних

    private void ResetPlayerMovementData()
    {
        Debug.Log("[MainMenuManager] [NEW] Очищення статичних даних PlayerMovementInputSystem...");

        var playerType = Type.GetType("PlayerMovementInputSystem");
        if (playerType == null)
        {
            Debug.LogWarning("[MainMenuManager] PlayerMovementInputSystem не знайдено");
            return;
        }

        FieldInfo savedPositionField = playerType.GetField("savedPosition",
            BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo hasSavedPositionField = playerType.GetField("hasSavedPosition",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (savedPositionField != null)
            savedPositionField.SetValue(null, Vector3.zero);

        if (hasSavedPositionField != null)
            hasSavedPositionField.SetValue(null, false);

        Debug.Log("[MainMenuManager] Статичні дані PlayerMovementInputSystem очищені");
    }

    private void ResetInventoryData()
    {
        Debug.Log("[MainMenuManager] [NEW] Очищення даних InventoryManager...");

        var inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.Log("[MainMenuManager] InventoryManager ще не створено (нормально)");
            return;
        }

        FieldInfo itemsField = typeof(InventoryManager).GetField("items",
            BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo descriptionsField = typeof(InventoryManager).GetField("itemDescriptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (itemsField != null)
        {
            var items = itemsField.GetValue(inventoryManager) as System.Collections.Generic.List<Sprite>;
            if (items != null)
            {
                items.Clear();
                Debug.Log("[MainMenuManager] Список предметів очищен");
            }
        }

        if (descriptionsField != null)
        {
            var descriptions = descriptionsField.GetValue(inventoryManager)
                as System.Collections.Generic.Dictionary<Sprite, string>;
            if (descriptions != null)
            {
                descriptions.Clear();
                Debug.Log("[MainMenuManager] Словник описань очищен");
            }
        }

        for (int i = 0; i < inventoryManager.slots.Count; i++)
        {
            if (inventoryManager.slots[i] != null)
                inventoryManager.slots[i].sprite = inventoryManager.emptySlotSprite;
        }
        Debug.Log("[MainMenuManager] UI слоти очищені");
    }

    // основна функція нової гри

    // стартова сцена

    public void NewGame()
    {
        Debug.Log("[SimpleMainMenu] Старт нової гри...");

        // скидання PlayerPrefs
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.SetInt("FirstLoadAfterNewGame", 1);
        PlayerPrefs.Save();

        // скидання GameState та Inventory
        if (GameStateManager.Instance != null) GameStateManager.Instance.ResetState();
        if (GameSaveSystem.Instance != null) GameSaveSystem.Instance.DeleteAllSaves();
        if (InventoryManager.Instance != null) InventoryManager.Instance.ResetInventory();

        // підписка на завантаження сцени
        SceneManager.sceneLoaded += OnNewGameSceneLoaded;

        // завантаження стартової сцени
        SceneManager.LoadScene(firstGameSceneName);
    }

    private void OnNewGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != firstGameSceneName) return;

        // відновити позицію гравця
        PlayerMovementInputSystem player = FindObjectOfType<PlayerMovementInputSystem>();
        if (player != null)
            player.RestoreStartPosition(new Vector3(-7f, -5f, 0f));

        // інвентар (якщо потрібне додаткове оновлення UI)
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ResetInventory();

        // відписка
        SceneManager.sceneLoaded -= OnNewGameSceneLoaded;

        Debug.Log("Нова гра запущена");
    }




    public void ContinueGame()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("[MainMenuManager] Немає збереженої гри!");
            return;
        }

        Time.timeScale = 1f;

        PlayerPrefs.SetInt("IsNewGame", 0);
        PlayerPrefs.Save();
        Debug.Log("[MainMenuManager] Прапорець IsNewGame скинуто на 0");

        try
        {
            string json = File.ReadAllText(saveFilePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            string savedScene = saveData.lastSceneName;

            if (!string.IsNullOrEmpty(savedScene))
            {
                Debug.Log($"[MainMenuManager] Завантаження збереженої сцени: '{savedScene}'");
                SceneManager.LoadScene(savedScene);
            }
            else
            {
                Debug.LogWarning("[MainMenuManager] Немає збереженої сцени, завантажуємо першу");
                SceneManager.LoadScene(firstGameSceneName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[MainMenuManager] Помилка читання збереження: {e.Message}");
            SceneManager.LoadScene(firstGameSceneName);
        }
    }

    public void OpenSettings()
    {
        Debug.Log("[MainMenuManager] Відкриття Settings...");
        SettingsManager.Instance.OpenSettings(
            backCallback: OnSettingsClose,
            context: SettingsContext.MainMenu
        );
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == firstGameSceneName)
        {
            PlayerMovementInputSystem player = FindObjectOfType<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.RestoreStartPosition(new Vector3(-7f, -5f, 0f)); // початкова позиція
                Debug.Log("Стартова позиція гравця відновлена");
            }
        }
    }

    private void OnSettingsClose()
    {
        Debug.Log("[MainMenuManager] Повернення з Settings");
        UpdateContinueButtonVisibility();
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenuManager] Вихід з гри!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}