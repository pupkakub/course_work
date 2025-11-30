using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FridgeSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image fridgeImage;
    public Button closeButton;
    public GameObject noteObject; 

    [Header("Назва попередньої сцени")]
    public string previousSceneName = "MainScene";

    void Start()
    {
        // встановити спрайт холодильника
        if (GameStateManager.Instance != null && fridgeImage != null)
        {
            fridgeImage.sprite = GameStateManager.Instance.fridgeDetailSprite;
        }

        // перевірити чи записка вже забрана
        bool noteCollected = GameSaveSystem.Instance.HasItemInInventory("FridgeNote");

        if (noteObject != null)
        {
            // якщо записка вже забрана — сховати її
            noteObject.SetActive(!noteCollected);
        }

        // налаштувати кнопку закриття
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseScene);
        }

        // налаштувати клік по записці
        SetupNoteInteraction();
    }

    private void SetupNoteInteraction()
    {
        if (noteObject == null) return;

        Button noteButton = noteObject.GetComponent<Button>();
        if (noteButton != null)
        {
            noteButton.onClick.AddListener(CollectNote);
            return;
        }
    }

    public void CollectNote()
    {
        Debug.Log("[FridgeSceneManager] Записку забрано!");

        //  додати записку до інвентаря
        GameSaveSystem.Instance.AddItemToInventory("FridgeNote");

        // встановити прапорець у GameStateManager
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetNoteCollected(true);

        // сховати записку
        if (noteObject != null)
            noteObject.SetActive(false);

        // автозбереження
        GameSaveSystem.Instance.AutoSave();

        Debug.Log("[FridgeSceneManager] Гру автоматично збережено після взяття записки");
    }

    public void CloseScene()
    {
        SceneManager.LoadScene(previousSceneName);
    }
}