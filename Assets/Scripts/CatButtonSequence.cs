using UnityEngine;
using UnityEngine.UI;

public class CatButtonSequence : MonoBehaviour
{
    [Header("Кнопки")]
    public Button petButton;
    public Button playButton;
    public Button feedButton;

    [Header("Аудіо")]
    public AudioSource doorbell;

    [Header("UI виконаної головоломки")]
    public GameObject completedUI; // панель що показується коли головоломка виконана
    public GameObject puzzleUI; // панель з кнопками головоломки

    private int[] correctSequence = { 0, 1, 2 }; // правильна послідовність
    private int currentStep = 0;
    private bool puzzleCompleted = false;

    void Awake()
    {
        if (petButton == null || playButton == null || feedButton == null)
        {
            Debug.LogError("CatButtonSequence: Не всі кнопки підключені в Inspector!");
            return;
        }
        if (doorbell == null)
        {
            Debug.LogWarning("CatButtonSequence: AudioSource для дзвінка не підключено!");
        }
    }

    void Start()
    {
        // чи головоломка вже виконана
        if (GameSaveSystem.Instance.IsPuzzleCompleted("CatPuzzle"))
        {
            Debug.Log("Головоломка з котом вже виконана!");
            puzzleCompleted = true;
            ShowCompletedState();
            return;
        }

        // прив'язати кнопки
        petButton.onClick.AddListener(() => OnButtonPressed(0));
        playButton.onClick.AddListener(() => OnButtonPressed(1));
        feedButton.onClick.AddListener(() => OnButtonPressed(2));

        Debug.Log("CatButtonSequence готовий до роботи!");
    }

    void OnButtonPressed(int buttonIndex)
    {
        if (puzzleCompleted) return; // якщо вже виконано - ігнорувати

        Debug.Log($"Натиснуто кнопку {buttonIndex}");

        if (buttonIndex == correctSequence[currentStep])
        {
            Debug.Log($"Кнопка {buttonIndex} натиснута правильно! Крок {currentStep + 1}/{correctSequence.Length}");
            currentStep++;

            // якщо виконано всю послідовність
            if (currentStep >= correctSequence.Length)
            {
                Debug.Log("Правильна послідовність завершена!");
                CompletePuzzle();
            }
        }
        else
        {
            Debug.Log("Неправильна кнопка! Скидаємо послідовність.");
            currentStep = 0;
        }
    }

    void CompletePuzzle()
    {
        puzzleCompleted = true;

        // зберегти виконання головоломки
        GameSaveSystem.Instance.MarkPuzzleCompleted("CatPuzzle");

        // встановити прапорець для дзвінка
        GameSaveSystem.Instance.SetFlag("DoorbellTriggered", true);

        // відтворити дзвінок
        TriggerDoorBell();

        // показати стан виконання
        ShowCompletedState();

        // оновити GameStateManager
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetCatSequenceCompleted(true);
        }
    }

    void TriggerDoorBell()
    {
        if (doorbell != null)
        {
            doorbell.Play();
            Debug.Log("Дзвінок відтворюється!");
        }
        else
        {
            Debug.LogWarning("AudioSource не підключено!");
        }
    }

    void ShowCompletedState()
    {
        // сховати кнопки головоломки
        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        // показати UI виконання
        if (completedUI != null)
            completedUI.SetActive(true);

        // вимкнути кнопки
        if (petButton != null) petButton.interactable = false;
        if (playButton != null) playButton.interactable = false;
        if (feedButton != null) feedButton.interactable = false;
    }
}