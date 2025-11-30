using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public Sprite fridgeDetailSprite;
    private bool catSequenceCompleted = false;
    private bool noteCollected = false;
    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameStateManager] Створено та встановлено DontDestroyOnLoad");
        }
        else
        {
            Debug.Log("[GameStateManager] Знищено дублікат");
            Destroy(gameObject);
        }
    }

    // кіт
    public bool IsCatSequenceCompleted()
    {
        return catSequenceCompleted;
    }

    public void SetCatSequenceCompleted(bool completed)
    {
        catSequenceCompleted = completed;
        Debug.Log($"[GameStateManager] Стан квесту з котом змінено: {completed}");
    }

    // записка
    public bool IsNoteCollected()
    {
        return noteCollected;
    }

    public void SetNoteCollected(bool collected)
    {
        noteCollected = collected;
        Debug.Log($"[GameStateManager] Стан записки змінено: {collected}");
    }

    // пауза

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Debug.Log($"[GameStateManager] Пауза: {paused}");
    }

    // скинути стан

    /// Повне скидання всього стану гри (для нової гри)
    public void ResetState()
    {
        // скинути стан холодильника
        fridgeDetailSprite = null;
        Debug.Log("[GameStateManager] fridgeDetailSprite = null");

        // скинути квест з котом
        catSequenceCompleted = false;
        Debug.Log("[GameStateManager] catSequenceCompleted = false");

        // скинути стан записки
        noteCollected = false;
        Debug.Log("[GameStateManager] noteCollected = false");

        // скинути паузу
        IsPaused = false;
        Debug.Log("[GameStateManager] IsPaused = false");

        Debug.Log("[GameStateManager] СТАН ПОВНІСТЮ СКИНУТО");
    }

    // дебаг

    [ContextMenu("Debug: Show Current State")]
    public void DebugShowState()
    {
        Debug.Log("========== ПОТОЧНИЙ СТАН GameStateManager ==========");
        Debug.Log($"catSequenceCompleted: {catSequenceCompleted}");
        Debug.Log($"noteCollected: {noteCollected}");
        Debug.Log($"IsPaused: {IsPaused}");
        Debug.Log($"fridgeDetailSprite: {(fridgeDetailSprite != null ? fridgeDetailSprite.name : "NULL")}");
        Debug.Log("====================================================");
    }

    [ContextMenu("Debug: Reset State")]
    public void DebugResetState()
    {
        ResetState();
    }


}