using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneAutoSave : MonoBehaviour
{
    [Header("Налаштування")]
    public bool saveOnSceneLoad = true;
    public bool savePlayerPosition = false;
    public Transform playerTransform;
    private bool hasSavedThisScene = false;
    void Awake()
    {
        Debug.Log($"[SceneAutoSave] AWAKE в сцені: {SceneManager.GetActiveScene().name}");
        PrintAllPlayerPrefs();
    }

    void Start()
    {
        Debug.Log($"[SceneAutoSave] ========== START ВИКЛИКАНО ==========");
        Debug.Log($"[SceneAutoSave] Сцена: {SceneManager.GetActiveScene().name}");
        Debug.Log($"[SceneAutoSave] saveOnSceneLoad = {saveOnSceneLoad}");
        PrintAllPlayerPrefs();

        if (saveOnSceneLoad)
        {
            PerformAutoSave();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneAutoSave] ========== OnSceneLoaded ВИКЛИКАНО ==========");
        Debug.Log($"[SceneAutoSave] Сцена: {scene.name}, Mode: {mode}");
        PrintAllPlayerPrefs();

        if (IsMenuScene(scene.name))
        {
            Debug.Log($"[SceneAutoSave] Пропускаємо (меню сцена)");
            return;
        }

        if (saveOnSceneLoad)
        {
            PerformAutoSave();
        }
    }

    private void PerformAutoSave()
    {
        if (hasSavedThisScene)
            return;

        hasSavedThisScene = true;

        Debug.Log("[SceneAutoSave] ========== PerformAutoSave ВИКЛИКАНО ==========");
        PrintAllPlayerPrefs();

        if (GameSaveSystem.Instance == null)
        {
            Debug.LogError("[SceneAutoSave] GameSaveSystem NULL!");
            return;
        }

        // Перевіряємо прапорці
        int firstLoadAfterNewGame = PlayerPrefs.GetInt("FirstLoadAfterNewGame", -999);
        int isNewGame = PlayerPrefs.GetInt("IsNewGame", -999);

        Debug.Log($"[SceneAutoSave] FirstLoadAfterNewGame = {firstLoadAfterNewGame}");
        Debug.Log($"[SceneAutoSave] IsNewGame = {isNewGame}");

        // Якщо це перший запуск після нової гри
        if (firstLoadAfterNewGame == 1)
        {
            Debug.LogWarning("[SceneAutoSave] ПЕРШИЙ ЗАПУСК ПІСЛЯ 'НОВА ГРА' - СКИПАЄМО!");

            PlayerPrefs.SetInt("FirstLoadAfterNewGame", 0);
            PlayerPrefs.Save();

            return;
        }

        // Якщо IsNewGame все ще 1
        if (isNewGame == 1)
        {
            Debug.LogWarning("[SceneAutoSave] IsNewGame = 1 - СКИПАЄМО!");
            return;
        }

        // НОРМАЛЬНЕ ЗБЕРЕЖЕННЯ
        Debug.Log($"[SceneAutoSave] ЗБЕРЕЖЕННЯ ДОЗВОЛЕНО!");

        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneAutoSave] Зберігаємо сцену: {currentScene}");

        GameSaveSystem.Instance.SaveGame(currentScene, null);

        Debug.Log($"[SceneAutoSave] Збереження завершено!");
    }

    private bool IsMenuScene(string sceneName)
    {
        string[] menuScenes = { "MainMenu", "Settings", "LoadingScreen", "Menu" };
        foreach (string menu in menuScenes)
        {
            if (sceneName.Contains(menu))
                return true;
        }
        return false;
    }

    private void PrintAllPlayerPrefs()
    {
        Debug.Log("[SceneAutoSave] ===== ВЕСЬ PlayerPrefs =====");
        Debug.Log($"IsNewGame: {PlayerPrefs.GetInt("IsNewGame", -999)}");
        Debug.Log($"FirstLoadAfterNewGame: {PlayerPrefs.GetInt("FirstLoadAfterNewGame", -999)}");
        Debug.Log("[SceneAutoSave] =============================");
    }

    public void ManualSave()
    {
        Debug.Log("[SceneAutoSave] РУЧНЕ ЗБЕРЕЖЕННЯ");

        if (GameSaveSystem.Instance == null)
        {
            Debug.LogError("[SceneAutoSave] GameSaveSystem NULL!");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        GameSaveSystem.Instance.SaveGame(currentScene, null);
        Debug.Log($"[SceneAutoSave] Ручне збереження: {currentScene}");
    }
}