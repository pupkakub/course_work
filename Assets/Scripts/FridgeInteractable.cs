using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class FridgeInteractable : MonoBehaviour
{
    [Header("Сцена з холодильником")]
    public string fridgeSceneName = "FridgeScene";

    [Header("Спрайти холодильника")]
    public Sprite fridgeWithNote;
    public Sprite fridgeWithoutNote;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError($"[{name}] SpriteRenderer не знайдено на GameObject!");
    }

    void Start()
    {
        // чекати поки GameSaveSystem буде готовий
        StartCoroutine(InitializeSprite());
    }

    private IEnumerator InitializeSprite()
    {
        while (GameSaveSystem.Instance == null)
            yield return null;

        yield return new WaitForSeconds(0.05f);

        // оновити спрайт на основі збережених даних
        UpdateFridgeSprite();
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
        if (scene.name == "MainScene")
        {
            // після завантаження MainScene оновити спрайт холодильника
            StartCoroutine(UpdateSpriteAfterSceneLoad());
        }
    }

    private IEnumerator UpdateSpriteAfterSceneLoad()
    {
        while (GameSaveSystem.Instance == null || GameStateManager.Instance == null)
            yield return null;

        yield return new WaitForSeconds(0.05f);

        // оновити спрайт
        UpdateFridgeSprite();
    }

    private void UpdateFridgeSprite()
    {
        if (spriteRenderer == null) return;

        bool noteCollected = false;

        // взяти стан записки з GameStateManager, якщо він є
        if (GameStateManager.Instance != null)
            noteCollected = GameStateManager.Instance.IsNoteCollected();
        else if (GameSaveSystem.Instance != null)
            noteCollected = GameSaveSystem.Instance.HasItemInInventory("FridgeNote");

        if (noteCollected)
        {
            if (fridgeWithoutNote != null)
            {
                spriteRenderer.sprite = fridgeWithoutNote;
                Debug.Log($"[{name}] Оновлено спрайт: без записки");
            }
        }
        else
        {
            if (fridgeWithNote != null)
            {
                spriteRenderer.sprite = fridgeWithNote;
                Debug.Log($"[{name}] Оновлено спрайт: з запискою");
            }
        }
    }

    void Update()
    {
        if (GlobalPauseMenu.IsGamePaused())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (GameSaveSystem.Instance == null)
                {
                    Debug.LogWarning("[FridgeInteractable] GameSaveSystem не готовий!");
                    return;
                }

                bool noteCollected = GameSaveSystem.Instance.HasItemInInventory("FridgeNote");

                // встановити спрайт у GameStateManager
                if (GameStateManager.Instance != null)
                    GameStateManager.Instance.fridgeDetailSprite = noteCollected ? fridgeWithoutNote : fridgeWithNote;

                Debug.Log($"[FridgeInteractable] Відкриваємо холодильник {(noteCollected ? "БЕЗ" : "З")} записки");

                SceneManager.LoadScene(fridgeSceneName);
            }
        }
    }
}
