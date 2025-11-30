using UnityEngine;
using UnityEngine.InputSystem;

public class NoteInteraction : MonoBehaviour
{
    [Header("Item Info")]
    public Sprite noteSprite;

    [TextArea(3, 10)]
    public string noteDescription = "Це записка від Даші. Хто зна, можливо якщо все виконати правильно, вони з Геною прийдуть?"; // Текст опису записки

    void Start()
    {
        // перевірка, чи записка вже зібрана
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsNoteCollected())
        {
            gameObject.SetActive(false);
            Debug.Log("Записка вже зібрана раніше - ховаємо");
            return;
        }

        Debug.Log($"NoteInteraction запущено на об'єкті: {gameObject.name}");

        var col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogError("Collider2D НЕ знайдено!");

        if (noteSprite == null)
            Debug.LogError("Note Sprite НЕ призначено в Inspector!");
        if (string.IsNullOrEmpty(noteDescription))
            Debug.LogWarning("Note Description пустий!");
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("Клік по записці!");
                CollectNote();
            }
        }
    }

    void CollectNote()
    {
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager НЕ знайдено!");
            return;
        }

        if (noteSprite == null)
        {
            Debug.LogError("noteSprite не призначено в Inspector!");
            return;
        }

        // додати в інвентар разом з описом
        inventoryManager.AddItem(noteSprite, string.IsNullOrEmpty(noteDescription)
            ? "Без опису"
            : noteDescription);

        // зберегти стан (GameState)
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetNoteCollected(true);
        }

        // сховати записку
        gameObject.SetActive(false);
        Debug.Log("Записка додана в інвентар і схована!");
    }
}
