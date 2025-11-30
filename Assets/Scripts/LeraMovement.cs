using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovementInputSystem : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private float moveX;
    public float leftLimit = -10f;
    public float rightLimit = 10f;
    private float halfWidth;
    private bool canMove = true;

    private static Vector3 savedPosition;
    private static bool hasSavedPosition = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        var col = GetComponent<Collider2D>();
        if (col != null) halfWidth = col.bounds.extents.x;

        if (gameObject.CompareTag("Player"))
        {
            var existing = GameObject.FindObjectsByType<PlayerMovementInputSystem>(FindObjectsSortMode.None)
                                     .FirstOrDefault(p => p.CompareTag("Player") && p != this);
            if (existing != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(transform.root.gameObject);

            if (hasSavedPosition && SceneManager.GetActiveScene().name == "MainScene")
                transform.position = savedPosition;
        }
    }
    public void RestoreStartPosition(Vector3 startPos)
    {
        transform.position = startPos;
        Debug.Log($"Позиція гравця відновлено: {startPos}");
    }
    public static void ResetSavedPosition()
    {
        savedPosition = Vector3.zero;
        hasSavedPosition = false;
        Debug.Log("[PlayerMovementInputSystem] Статична позиція гравця скинута");
    }
    void Start()
    {
        UpdatePlayerVisibility(SceneManager.GetActiveScene().name == "MainScene");
        if (SceneManager.GetActiveScene().name == "MainScene")
            RegisterWithBackgroundManager();
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene prev, Scene next)
    {
        bool isMainScene = next.name == "MainScene";
        if (prev.name == "MainScene" && !isMainScene)
        {
            savedPosition = transform.position;
            hasSavedPosition = true;
            Debug.Log($"Збережено позицію персонажа: {savedPosition}");
        }

        UpdatePlayerVisibility(isMainScene);
        canMove = isMainScene;

        if (isMainScene)
        {
            if (hasSavedPosition)
            {
                transform.position = savedPosition;
                Debug.Log($"Відновлено позицію персонажа: {savedPosition}");
            }
            RegisterWithBackgroundManager();
        }
    }

    private void UpdatePlayerVisibility(bool visible)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = visible;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = visible;

        if (animator != null) animator.enabled = visible;

        var allSprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in allSprites) sprite.enabled = visible;

        var allCanvas = GetComponentsInChildren<Canvas>(true);
        foreach (var canvas in allCanvas) canvas.enabled = visible;

        var allAnimators = GetComponentsInChildren<Animator>(true);
        foreach (var anim in allAnimators) anim.enabled = visible;

        Debug.Log($"Персонаж видимість встановлено: {visible} (сцена: {SceneManager.GetActiveScene().name})");
    }

    private void RegisterWithBackgroundManager()
    {
        var bgManager = FindFirstObjectByType<BackgroundManager>();
        if (bgManager != null)
            bgManager.RegisterPlayer(this);
        else
            Debug.LogWarning("BackgroundManager не знайдено на MainScene!");
    }

    void Update()
    {
        moveX = 0f;

        if (GlobalPauseMenu.IsGamePaused())
        {
            EnableControl(false);
            return;
        }

        if (!canMove) return;

        if (!canMove)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        if (Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

        if (moveX != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);

        if (GameStateManager.Instance != null && GameStateManager.Instance.IsPaused)
            return;
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        Vector2 newPos = rb.position + new Vector2(moveX, 0) * speed * Time.fixedDeltaTime;
        newPos.x = Mathf.Clamp(newPos.x, leftLimit + halfWidth, rightLimit - halfWidth);
        rb.MovePosition(newPos);

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(moveX));
    }

    public void SetMovementLimits(float left, float right)
    {
        leftLimit = left;
        rightLimit = right;
        Debug.Log($"Player limits set: left={left}, right={right}");
    }

    // вмикання/вимкання контролю (не вимикає компонент повністю)
    public void EnableControl(bool enable)
    {
        canMove = enable;
        if (!enable)
        {
            moveX = 0f;
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0f);
        }
    }
}
