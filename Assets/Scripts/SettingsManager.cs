using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SettingsContext
{
    MainMenu,
    Pause
}

public class SettingsManager : MonoBehaviour
{
    [Header("Volume & Fullscreen")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    [Header("Dropdowns")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    [Header("Brightness")]
    public Slider brightnessSlider;

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    private static SettingsManager _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SettingsManager>();

                if (_instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("SettingsCanvas");
                    if (prefab != null)
                    {
                        GameObject instance = Instantiate(prefab);
                        _instance = instance.GetComponent<SettingsManager>();

                        if (_instance != null)
                        {
                            DontDestroyOnLoad(instance);
                            // деактивувати одразу після створення
                            instance.SetActive(false);
                            Debug.Log("[SettingsManager] Завантажено з Resources та деактивовано");
                        }
                    }
                    else
                    {
                        Debug.LogError("[SettingsManager] SettingsCanvas prefab не знайдено в папці Resources!");
                    }
                }
            }
            return _instance;
        }
    }

    private System.Action onBackCallback;
    private SettingsContext currentContext = SettingsContext.MainMenu;
    private bool isInitialized = false; // новий прапорець

    Resolution[] resolutions = new Resolution[]
    {
        new Resolution() { width = 1920, height = 1080 },
        new Resolution() { width = 1600, height = 900 },
        new Resolution() { width = 1280, height = 720 }
    };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // ініціалізувати одразу в Awake
        Initialize();
    }

    /// Окрема ініціалізація
    private void Initialize()
    {
        if (isInitialized) return;

        Debug.Log("[SettingsManager] Початок ініціалізації...");

        FindAllUIComponents();
        FindBrightnessOverlay();
        LoadAndApplySettings();
        SetupBackButton();

        isInitialized = true;
        Debug.Log("[SettingsManager] Ініціалізація завершена");
    }

    private void Start()
    {
        // все в Initialize()
        // Prefab вже деактивований у Instance getter
    }

    private void OnEnable()
    {
        // гарантуємо ініціалізацію при активації
        Initialize();

        if (backButton != null)
        {
            ForceActivateButton();
        }
        else
        {
            Debug.LogWarning("[SettingsManager] Back button null у OnEnable");
        }
    }

    /// знаходить всі UI компоненти
    private void FindAllUIComponents()
    {
        if (backButton == null)
        {
            Debug.LogWarning("[SettingsManager] Шукаємо Back button...");

            string[] possiblePaths = new string[]
            {
                "BackButton",
                "Panel/BackButton",
                "Panel/BottomPanel/BackButton",
                "ButtonBack",
                "UI/BackButton",
                "Background/BackButton"
            };

            foreach (string path in possiblePaths)
            {
                Transform found = transform.Find(path);
                if (found != null)
                {
                    backButton = found.GetComponent<Button>();
                    if (backButton != null)
                    {
                        Debug.Log($"[SettingsManager] Знайдено Back button за шляхом: {path}");
                        return;
                    }
                }
            }

            Button[] allButtons = GetComponentsInChildren<Button>(true);
            Debug.Log($"[SettingsManager] Знайдено {allButtons.Length} кнопок у prefab:");

            foreach (Button btn in allButtons)
            {
                string lowerName = btn.name.ToLower();
                if (lowerName.Contains("back") ||
                    lowerName.Contains("назад") ||
                    lowerName.Contains("close") ||
                    lowerName.Contains("return"))
                {
                    backButton = btn;
                    Debug.Log($"[SettingsManager] Знайдено Back button: {btn.name}");
                    return;
                }
            }

            Debug.LogError("[SettingsManager] КРИТИЧНА ПОМИЛКА: Кнопку 'Назад' не знайдено!");
        }

        if (volumeSlider == null) volumeSlider = GetComponentInChildren<Slider>(true);
        if (fullscreenToggle == null) fullscreenToggle = GetComponentInChildren<Toggle>(true);
        if (qualityDropdown == null)
        {
            TMP_Dropdown[] dropdowns = GetComponentsInChildren<TMP_Dropdown>(true);
            if (dropdowns.Length > 0) qualityDropdown = dropdowns[0];
        }
        if (resolutionDropdown == null)
        {
            TMP_Dropdown[] dropdowns = GetComponentsInChildren<TMP_Dropdown>(true);
            if (dropdowns.Length > 1) resolutionDropdown = dropdowns[1];
        }
        if (brightnessSlider == null)
        {
            Slider[] sliders = GetComponentsInChildren<Slider>(true);
            if (sliders.Length > 1) brightnessSlider = sliders[1];
        }
    }

    private void ForceActivateButton()
    {
        if (backButton == null) return;

        backButton.gameObject.SetActive(true);

        Transform parent = backButton.transform.parent;
        while (parent != null && parent != transform)
        {
            if (!parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
            }
            parent = parent.parent;
        }

        CanvasGroup cg = backButton.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void SetupBackButton()
    {
        if (backButton == null)
        {
            Debug.LogError("[SettingsManager] Back button не знайдено!");
            return;
        }

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBackButton);

        Debug.Log($"[SettingsManager] Back button '{backButton.name}' налаштовано");
    }

    private void FindBrightnessOverlay()
    {
        GameObject overlayObj = GameObject.Find("BrightnessOverlay");

        if (overlayObj == null)
        {
            overlayObj = new GameObject("BrightnessOverlay");
            Image img = overlayObj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            img.raycastTarget = false;

            Canvas canvas = overlayObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 2500;

            CanvasGroup cg = overlayObj.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg.interactable = false;

            RectTransform rt = overlayObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            DontDestroyOnLoad(overlayObj);
        }

        Image brightnessOverlay = overlayObj.GetComponent<Image>();
        if (brightnessOverlay != null)
        {
            float savedBrightness = PlayerPrefs.GetFloat("brightness", 1f);
            SetBrightnessInternal(brightnessOverlay, savedBrightness);
        }
    }

    private void LoadAndApplySettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(ApplyVolume);
        }
        ApplyVolume(savedVolume);

        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);
        }
        ApplyFullscreen(isFullscreen);

        int savedQuality = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());
        if (qualityDropdown != null)
        {
            qualityDropdown.value = savedQuality;
            qualityDropdown.onValueChanged.RemoveAllListeners();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }
        SetQuality(savedQuality);

        int savedRes = PlayerPrefs.GetInt("resolution", 0);
        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = savedRes;
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
        SetResolution(savedRes);

        float savedBrightness = PlayerPrefs.GetFloat("brightness", 1f);
        if (brightnessSlider != null)
        {
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
    }

    public void OpenSettings(System.Action backCallback = null, SettingsContext context = SettingsContext.MainMenu)
    {
        // гарантувати ініціалізацію перед відкриттям
        Initialize();

        onBackCallback = backCallback;
        currentContext = context;

        gameObject.SetActive(true);

        if (backButton != null)
        {
            ForceActivateButton();
        }

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1600;
        }

        Debug.Log($"[SettingsManager] Settings menu opened from {context}");
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
        Debug.Log("[SettingsManager] Settings menu closed");
    }

    public void OnBackButton()
    {
        Debug.Log("[SettingsManager] OnBackButton викликано!");

        CloseSettings();

        if (onBackCallback != null)
        {
            onBackCallback.Invoke();
        }
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save();
    }

    public void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            PlayerPrefs.SetInt("resolution", index);
            PlayerPrefs.Save();
        }
    }

    public void SetBrightness(float value)
    {
        GameObject overlayObj = GameObject.Find("BrightnessOverlay");
        if (overlayObj != null)
        {
            Image overlay = overlayObj.GetComponent<Image>();
            if (overlay != null)
            {
                SetBrightnessInternal(overlay, value);
            }
        }

        PlayerPrefs.SetFloat("brightness", value);
        PlayerPrefs.Save();
    }

    private void SetBrightnessInternal(Image overlay, float value)
    {
        float minBrightness = 0.2f;
        float alpha = Mathf.Clamp(1f - value, 0f, 1f - minBrightness);

        Color c = overlay.color;
        c.a = alpha;
        overlay.color = c;
    }
}