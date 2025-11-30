using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GlobalPauseMenu : MonoBehaviour
{
    public static GlobalPauseMenu Instance;

    [Header("Pause Menu UI")]
    public GameObject pauseMenuCanvas;
    public Button pauseButton;
    public Sprite pauseIcon;
    public Sprite playIcon;

    [Header("Other Canvas to Hide During Pause")]
    public Canvas[] canvasesToHide;

    [Header("Pause Menu Navigation")]
    public Button mainMenuButton;
    public string mainMenuSceneName = "MainMenuScene";

    [Header("UI Management")]
    public GameObject[] uiToHideInMainMenu; // масив UI для приховування в головному меню
    public string[] gameplaySceneNames; // список сцен де показувати UI

    private bool isPaused = false;
    private bool settingsOpen = false;
    private bool isInMainMenu = false;

    private void Awake()
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SetupPauseMenu();
        SetupPauseButton();
        StartCoroutine(InitializeSettingsManagerCoroutine());

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // перевірити поточну сцену
        CheckCurrentScene();
    }

    private void GoToMainMenu()
    {
        // закрити налаштування, якщо відкриті
        if (settingsOpen && SettingsManager.Instance != null)
        {
            SettingsManager.Instance.CloseSettings();
            settingsOpen = false;
        }

        // закрити паузу
        if (isPaused)
        {
            isPaused = false;
        }

        // скинути TimeScale
        Time.timeScale = 1f;

        // позначити що йдемо в головне меню
        isInMainMenu = true;

        // сховати всі UI елементи ПЕРЕД завантаженням сцени
        HideGameplayUI();

        //завантажити сцену головного меню
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("[GlobalPauseMenu] mainMenuSceneName не задано!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //перевірити в якій сцені знаходимось
        CheckCurrentScene();

        SetupPauseMenu();
        SetupPauseButton();
        StartCoroutine(InitializeSettingsManagerCoroutine());
    }

    /// Перевіряє поточну сцену і керує видимістю UI
    private void CheckCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // перевірити чи це головне меню
        isInMainMenu = (currentSceneName == mainMenuSceneName);

        if (isInMainMenu)
        {
            // сховати весь геймплейний UI
            HideGameplayUI();

            // закрити меню паузи
            if (pauseMenuCanvas != null)
                pauseMenuCanvas.SetActive(false);
        }
        else
        {
            // перевірити чи це геймплейна сцена
            bool isGameplayScene = IsGameplayScene(currentSceneName);

            if (isGameplayScene)
            {
                ShowGameplayUI();
            }
            else
            {
                HideGameplayUI();
            }
        }
    }

    /// Перевірити чи є поточна сцена геймплейною
    private bool IsGameplayScene(string sceneName)
    {
        if (gameplaySceneNames == null || gameplaySceneNames.Length == 0)
        {
            // якщо не вказано список, вважаємо що всі сцени крім головного меню - геймплейні
            return sceneName != mainMenuSceneName;
        }

        foreach (string gameplayScene in gameplaySceneNames)
        {
            if (sceneName == gameplayScene)
                return true;
        }

        return false;
    }

    /// ховати геймплейний UI (кнопка паузи, інвентар тощо)
    private void HideGameplayUI()
    {
        // ховати кнопку паузи
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);

        // ховати додаткові UI елементи
        if (uiToHideInMainMenu != null)
        {
            foreach (GameObject ui in uiToHideInMainMenu)
            {
                if (ui != null)
                    ui.SetActive(false);
            }
        }
    }

    /// Показує геймплейний UI
    private void ShowGameplayUI()
    {
        // показати кнопку паузи
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true);

        // показати додаткові UI елементи
        if (uiToHideInMainMenu != null)
        {
            foreach (GameObject ui in uiToHideInMainMenu)
            {
                if (ui != null)
                    ui.SetActive(true);
            }
        }
    }

    private IEnumerator InitializeSettingsManagerCoroutine()
    {
        yield return null;

        if (SettingsManager.Instance != null)
        {
            Debug.Log("[GlobalPauseMenu] SettingsManager готовий");
        }
        else
        {
            Debug.LogError("[GlobalPauseMenu] SettingsManager не вдалося ініціалізувати!");
        }
    }

    private void SetupPauseMenu()
    {
        if (pauseMenuCanvas == null) return;

        var canvas = pauseMenuCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1500;
        }

        pauseMenuCanvas.SetActive(false);
    }

    private void SetupPauseButton()
    {
        if (pauseButton == null)
        {
            Debug.LogWarning("[GlobalPauseMenu] PauseButton not assigned!");
            return;
        }

        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(TogglePause);

        if (pauseIcon != null)
            pauseButton.image.sprite = pauseIcon;

        var buttonCanvas = pauseButton.GetComponentInParent<Canvas>();
        if (buttonCanvas != null)
        {
            buttonCanvas.overrideSorting = true;
            buttonCanvas.sortingOrder = 2000;
        }

        // налаштувати видимість залежно від сцени
        if (isInMainMenu)
        {
            pauseButton.gameObject.SetActive(false);
        }
        else
        {
            pauseButton.gameObject.SetActive(true);
            pauseButton.interactable = true;
        }
    }

    public void TogglePause()
    {
        // не дозволяти паузу в головному меню
        if (isInMainMenu)
            return;

        if (settingsOpen)
        {
            CloseSettingsAndResume();
            return;
        }

        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        // не дозволяти паузу в головному меню
        if (isInMainMenu)
            return;

        isPaused = true;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

        if (pauseButton != null && playIcon != null)
            pauseButton.image.sprite = playIcon;

        foreach (Canvas c in canvasesToHide)
        {
            if (c != null)
                c.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;

        var player = FindFirstObjectByType<PlayerMovementInputSystem>();
        if (player != null)
            player.EnableControl(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        settingsOpen = false;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        if (pauseButton != null && pauseIcon != null)
            pauseButton.image.sprite = pauseIcon;

        foreach (Canvas c in canvasesToHide)
        {
            if (c != null)
                c.gameObject.SetActive(true);
        }

        Time.timeScale = 1f;

        var players = FindObjectsOfType<PlayerMovementInputSystem>();
        foreach (var player in players)
            player.EnableControl(true);
    }

    public static bool IsGamePaused()
    {
        return Instance != null && Instance.isPaused;
    }

    public void ContinueGame()
    {
        if (isPaused)
            ResumeGame();
    }

    public void OpenSettings()
    {
        StartCoroutine(OpenSettingsCoroutine());
    }

    private IEnumerator OpenSettingsCoroutine()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsManager не знайдено! Чекаємо ініціалізації...");

            float waitTime = 0f;
            while (SettingsManager.Instance == null && waitTime < 1f)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                waitTime += 0.1f;
            }

            if (SettingsManager.Instance == null)
            {
                Debug.LogError("SettingsManager так і не ініціалізувався!");
                yield break;
            }
        }

        yield return null;

        settingsOpen = true;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        SettingsManager.Instance.OpenSettings(() => BackToPauseMenu(), SettingsContext.Pause);

        Debug.Log("Settings opened from pause menu");
    }

    private void BackToPauseMenu()
    {
        settingsOpen = false;

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

        Debug.Log("Back to pause menu");
    }

    private void CloseSettingsAndResume()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.CloseSettings();

        ResumeGame();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void QuitGame()
    {
        Debug.Log("QUIT pressed");

        // розморозити час
        Time.timeScale = 1f;

#if UNITY_EDITOR
        // закрити гру в редакторі
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // закрити гру в збірці
    Application.Quit();
#endif
    }

}