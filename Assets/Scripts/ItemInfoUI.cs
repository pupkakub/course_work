using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI Instance;

    [Header("UI Elements")]
    public GameObject panel; // панель ItemInfo
    public Image itemImage;           
    public TMP_Text itemDescription;  
    public Button backButton;         

    private void Awake()
    {
        // створюємо singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // перевірка компонентів
        if (panel == null || itemImage == null || itemDescription == null || backButton == null)
        {
            Debug.LogError("ItemInfoUI: призначте всі UI елементи в Inspector!");
            return;
        }

        // сховати панель на старті
        panel.SetActive(false);

        // додати кнопку назад
        backButton.onClick.AddListener(() =>
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.SetPaused(false);
        });
    }

   
    public void ShowInfo(Sprite sprite, string description)
    {
        itemImage.sprite = sprite;
        itemDescription.text = description ?? "Без опису";

        panel.SetActive(true);

        // зупинити гру
        Time.timeScale = 0f;

        // поставити флаг паузи
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetPaused(true);
    }

}

