using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GameSaveData
{
    // загальні дані
    public string lastSceneName = "";
    public DateTime lastSaveTime;

    // інвентар
    public List<string> inventoryItems = new List<string>();

    // прапорці стану гри
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    // виконані головоломки
    public List<string> completedPuzzles = new List<string>();

    // позиція гравця 
    public float playerX;
    public float playerY;
}

public class GameSaveSystem : MonoBehaviour
{

    public static GameSaveSystem Instance { get; private set; }

    private GameSaveData currentSave;
    private string saveFilePath;

    /// Публічний шлях до файлу збереження (read-only)

    public string SaveFilePath => saveFilePath;
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveSystem()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        Debug.Log($"[GameSaveSystem] Шлях до збереження: {saveFilePath}");

        currentSave = new GameSaveData();
    }

    /// Зберегти поточний стан гри
    /// 
    public void SaveGame(string sceneName, Vector2? playerPosition = null)
    {
        // перевірка чи це нова гра
        int isNewGame = PlayerPrefs.GetInt("IsNewGame", 0);
        if (isNewGame == 1)
        {
            Debug.LogWarning("[GameSaveSystem] Спроба збереження під час нової гри - пропущено");
            return;
        }

        currentSave.lastSceneName = sceneName;
        currentSave.lastSaveTime = DateTime.Now;

        if (playerPosition.HasValue)
        {
            currentSave.playerX = playerPosition.Value.x;
            currentSave.playerY = playerPosition.Value.y;
        }

        try
        {
            string json = JsonUtility.ToJson(currentSave, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"[GameSaveSystem] Гру збережено у сцені '{sceneName}'");
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameSaveSystem] Помилка збереження: {e.Message}");
        }
    }

    /// Автоматичне збереження (викликати при важливих подіях)

    public void AutoSave()
    {
        // перевірка чи це нова гра
        int isNewGame = PlayerPrefs.GetInt("IsNewGame", 0);
        if (isNewGame == 1)
        {
            Debug.LogWarning("[GameSaveSystem] Автозбереження під час нової гри - пропущено");
            return;
        }

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SaveGame(currentScene);
        Debug.Log("[GameSaveSystem] Автозбереження виконано");
    }

    /// Завантажити збережену гру
    /// 
    public bool LoadGame()
    {
        if (!HasSaveFile())
        {
            Debug.LogWarning("[GameSaveSystem] Файл збереження не знайдено");
            return false;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            currentSave = JsonUtility.FromJson<GameSaveData>(json);

            Debug.Log($"[GameSaveSystem] Гру завантажено. Остання сцена: {currentSave.lastSceneName}");
            Debug.Log($"[GameSaveSystem] Інвентар: {currentSave.inventoryItems.Count} предметів");
            Debug.Log($"[GameSaveSystem] Виконані головоломки: {currentSave.completedPuzzles.Count}");

            foreach (var item in currentSave.inventoryItems)
            {
                Debug.Log($"  - Предмет в інвентарі: {item}");
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameSaveSystem] ❌ Помилка завантаження: {e.Message}");
            return false;
        }
    }

    /// Видалити ВСІ збереження (для нової гри)
    public void DeleteAllSaves()
    {
        // видалити файл на диску
        if (File.Exists(saveFilePath))
        {
            try
            {
                File.Delete(saveFilePath);
                Debug.Log("[GameSaveSystem] Файл збереження видалено");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameSaveSystem] Помилка видалення файлу: {e.Message}");
            }
        }
        else
        {
            Debug.Log("[GameSaveSystem] Файл збереження не існує");
        }

        // створити новий порожній об'єкт збереження
        currentSave = new GameSaveData();

        // очистити всі поля вручну
        currentSave.inventoryItems = new List<string>();
        currentSave.completedPuzzles = new List<string>();
        currentSave.flags = new Dictionary<string, bool>();
        currentSave.lastSceneName = "";
        currentSave.playerX = 0f;
        currentSave.playerY = 0f;

        Debug.Log("[GameSaveSystem] ✓ Дані в пам'яті повністю очищено");
        Debug.Log($"[GameSaveSystem] Інвентар: {currentSave.inventoryItems.Count} предметів");
        Debug.Log($"[GameSaveSystem] Головоломки: {currentSave.completedPuzzles.Count}");
    }

    /// Видалити файл збереження 
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("[GameSaveSystem] Збереження видалено з диску");
        }

        currentSave = new GameSaveData();
        Debug.Log("[GameSaveSystem] Дані в пам'яті очищено");
    }

    /// Перевірка наявності файлу збереження
    public bool HasSaveFile()
    {
        bool exists = File.Exists(saveFilePath);
        Debug.Log($"[GameSaveSystem] Файл збереження існує: {exists}");
        return exists;
    }

    public void AddItemToInventory(string itemId)
    {
        if (!currentSave.inventoryItems.Contains(itemId))
        {
            currentSave.inventoryItems.Add(itemId);
            Debug.Log($"[GameSaveSystem] Додано до інвентаря: {itemId}");
            AutoSave();
        }
    }

    public void RemoveItemFromInventory(string itemId)
    {
        if (currentSave.inventoryItems.Contains(itemId))
        {
            currentSave.inventoryItems.Remove(itemId);
            Debug.Log($"[GameSaveSystem] Видалено з інвентаря: {itemId}");
            AutoSave();
        }
    }

    public bool HasItemInInventory(string itemId)
    {
        bool hasItem = currentSave.inventoryItems.Contains(itemId);
        Debug.Log($"[GameSaveSystem] Перевірка предмету '{itemId}': {(hasItem ? "Є" : "Немає")}");
        return hasItem;
    }

    public List<string> GetInventoryItems()
    {
        return new List<string>(currentSave.inventoryItems);
    }

    /// Очищення інвентаря
    public void ClearInventory()
    {
        if (currentSave != null)
        {
            currentSave.inventoryItems.Clear();
            Debug.Log("[GameSaveSystem] Інвентар очищено");
        }
    }

    public void SetFlag(string flagName, bool value)
    {
        currentSave.flags[flagName] = value;
        Debug.Log($"[GameSaveSystem] Прапорець '{flagName}' = {value}");
        AutoSave();
    }

    public bool GetFlag(string flagName, bool defaultValue = false)
    {
        if (currentSave.flags.ContainsKey(flagName))
            return currentSave.flags[flagName];
        return defaultValue;
    }

    public void MarkPuzzleCompleted(string puzzleId)
    {
        if (!currentSave.completedPuzzles.Contains(puzzleId))
        {
            currentSave.completedPuzzles.Add(puzzleId);
            Debug.Log($"[GameSaveSystem] Головоломку '{puzzleId}' виконано");
            AutoSave();
        }
    }

    public bool IsPuzzleCompleted(string puzzleId)
    {
        return currentSave.completedPuzzles.Contains(puzzleId);
    }

    // дані для завантаження
    public string GetLastSceneName()
    {
        return currentSave.lastSceneName;
    }

    public Vector2 GetPlayerPosition()
    {
        return new Vector2(currentSave.playerX, currentSave.playerY);
    }

    public DateTime GetLastSaveTime()
    {
        return currentSave.lastSaveTime;
    }

    // дебаг

    [ContextMenu("Debug: Show Save Info")]
    public void DebugShowSaveInfo()
    {
        Debug.Log("=== SAVE INFO ===");
        Debug.Log($"Last Scene: {currentSave.lastSceneName}");
        Debug.Log($"Last Save: {currentSave.lastSaveTime}");
        Debug.Log($"Inventory Items: {string.Join(", ", currentSave.inventoryItems)}");
        Debug.Log($"Completed Puzzles: {string.Join(", ", currentSave.completedPuzzles)}");
        Debug.Log($"Flags: {currentSave.flags.Count}");
        foreach (var flag in currentSave.flags)
        {
            Debug.Log($"  - {flag.Key}: {flag.Value}");
        }
    }

    [ContextMenu("Debug: Delete Save")]
    public void DebugDeleteSave()
    {
        DeleteAllSaves();
    }
}