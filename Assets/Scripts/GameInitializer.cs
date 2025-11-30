using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// Ініціалізує стан гри при завантаженні (для Continue)

public class GameInitializer : MonoBehaviour
{
    [Header("Посилання")]
    public Transform playerTransform;

    [Header("Налаштування")]
    [Tooltip("Відновлювати позицію гравця при Continue")]
    public bool restorePlayerPosition = false;

    void Start()
    {
        StartCoroutine(InitializeGameState());
    }

    private IEnumerator InitializeGameState()
    {
        // чекати на ініціалізацію системи збереження
        while (GameSaveSystem.Instance == null)
            yield return null;

        // спочатку перевірити чи це нова гра
        int isNewGame = PlayerPrefs.GetInt("IsNewGame", 0);
        Debug.Log($"[GameInitializer] Прапорець IsNewGame = {isNewGame}");

        if (isNewGame == 1)
        {
            Debug.LogWarning("[GameInitializer] Це НОВА ГРА - пропускаємо відновлення стану");
            
            Debug.Log("[GameInitializer] Встановлено прапорець FirstLoadAfterNewGame = 1");

            // скинути IsNewGame
            PlayerPrefs.SetInt("IsNewGame", 0);
            PlayerPrefs.Save();
            Debug.Log("[GameInitializer] Прапорець IsNewGame скинуто на 0");

            yield break; // ніц не відновлювати
        }

        // перевірити чи є взагалі збереження
        if (!GameSaveSystem.Instance.HasSaveFile())
        {
            Debug.Log("[GameInitializer] Немає збереження - пропускаємо відновлення");
            yield break;
        }

        Debug.Log("[GameInitializer] Це ПРОДОВЖЕННЯ гри - відновлюємо стан...");

        // відновити інвентар
        RestoreInventory();

        // відновитио стан головоломок
        RestorePuzzleStates();

        // відновити позицію гравця 
        if (restorePlayerPosition && playerTransform != null)
        {
            RestorePlayerPosition();
        }

        // відновити прапорці
        RestoreFlags();

        Debug.Log("[GameInitializer] Ініціалізацію завершено!");
    }

    private void RestoreInventory()
    {
        var items = GameSaveSystem.Instance.GetInventoryItems();
        Debug.Log($"[GameInitializer] Відновлено інвентар: {items.Count} предметів");

        foreach (var item in items)
        {
            Debug.Log($"  - {item}");
        }
    }

    private void RestorePuzzleStates()
    {
        Debug.Log("[GameInitializer] Перевірка головоломок...");

        // перевірити головоломку з котом
        if (GameSaveSystem.Instance.IsPuzzleCompleted("CatPuzzle"))
        {
            Debug.Log("[GameInitializer] Головоломка з котом виконана");

            // якщо є GameStateManager - встановити стан
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetCatSequenceCompleted(true);
            }

            // якщо дзвінок був активований - активувати його знову
            bool doorbellTriggered = GameSaveSystem.Instance.GetFlag("DoorbellTriggered");
            if (doorbellTriggered)
            {
                Debug.Log("[GameInitializer] Дзвінок був активований раніше");

                // знайти і відтворити аудіо дзвінка 
                var doorbell = GameObject.Find("DoorbellAudio")?.GetComponent<AudioSource>();
                if (doorbell != null)
                {
                    doorbell.Play();
                    Debug.Log("[GameInitializer] Дзвінок відтворено");
                }
            }
        }
    }

    private void RestorePlayerPosition()
    {
        Vector2 savedPosition = GameSaveSystem.Instance.GetPlayerPosition();
        if (savedPosition != Vector2.zero)
        {
            playerTransform.position = new Vector3(savedPosition.x, savedPosition.y, playerTransform.position.z);
            Debug.Log($"[GameInitializer] Позицію гравця відновлено: {savedPosition}");
        }
    }

    private void RestoreFlags()
    {
        Debug.Log("[GameInitializer] Перевірка прапорців...");

        // перевірити чи двері відчинені
        bool doorsUnlocked = GameSaveSystem.Instance.GetFlag("DoorsUnlocked");
        if (doorsUnlocked)
        {
            Debug.Log("[GameInitializer] Двері відчинені");
        }

        // перевірити чи записка забрана
        bool noteCollected = GameSaveSystem.Instance.GetFlag("NoteCollected");
        if (noteCollected && GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetNoteCollected(true);
            Debug.Log("[GameInitializer] Записка забрана");
        }
    }
}