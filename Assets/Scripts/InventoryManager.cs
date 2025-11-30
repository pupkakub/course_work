using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Slots")]
    public List<Image> slots = new List<Image>();
    public Sprite emptySlotSprite;

    [Header("Item Sprites")]
    public Sprite noteSprite;
    public Sprite potionSprite;
    public Sprite keySprite;

    private List<Sprite> items = new List<Sprite>();
    private Dictionary<Sprite, string> itemDescriptions = new Dictionary<Sprite, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // прив'язати кнопки до слотів
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            slots[i].sprite = emptySlotSprite;
            int index = i;

            Button btn = slots[i].GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnSlotClicked(index));
        }

        // додати предмети з описом
        AddItem(noteSprite,
            "Це записка від Даші. Хто зна, можливо якщо все виконати правильно, вони з Геною прийдуть?");
    }

   

    public void RemoveItem(Sprite itemSprite)
    {
        int index = items.IndexOf(itemSprite);
        if (index < 0) return;

        items.RemoveAt(index);
        itemDescriptions.Remove(itemSprite);

        if (index < slots.Count)
            slots[index].sprite = emptySlotSprite;

        for (int i = index; i < items.Count; i++)
        {
            slots[i].sprite = items[i];
        }
    }
   
    public void OnSlotClicked(int index)
    {
        if (index < 0 || index >= slots.Count) return;

        Sprite itemSprite = slots[index].sprite;
        if (itemSprite == null || itemSprite == emptySlotSprite) return;

        string description = itemDescriptions.ContainsKey(itemSprite)
            ? itemDescriptions[itemSprite]
            : "Без опису";

        ItemInfoUI.Instance.ShowInfo(itemSprite, description);
    }
    [System.Serializable]
    public class InventoryItem
    {
        public Sprite sprite;
        public string description;
    }

    // ДОДАЙТЕ ЦІ МЕТОДИ ДО ВАШОГО КЛАСУ InventoryManager:

    /// <summary>
    /// Отримати всі предмети з інвентаря (для збереження)
    /// </summary>
    public List<InventoryItem> GetAllItems()
    {
        List<InventoryItem> result = new List<InventoryItem>();

        // Припускаючи, що у вас є List<Sprite> items та Dictionary<Sprite, string> itemDescriptions
        foreach (var sprite in items)
        {
            if (sprite != null)
            {
                InventoryItem item = new InventoryItem
                {
                    sprite = sprite,
                    description = itemDescriptions.ContainsKey(sprite) ? itemDescriptions[sprite] : ""
                };
                result.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// Очищення інвентаря (для нової гри)
    /// </summary>
    public void ResetInventory()
    {
        if (items != null)
            items.Clear();

        if (itemDescriptions != null)
            itemDescriptions.Clear();

        // Очищаємо UI слоти
        if (slots != null)
        {
            foreach (var slot in slots)
            {
                if (slot != null && emptySlotSprite != null)
                {
                    slot.sprite = emptySlotSprite;
                }
            }
        }

        Debug.Log("[InventoryManager] Інвентар очищено");
    }

    /// <summary>
    /// Додавання предмету (публічний метод)
    /// </summary>
    public void AddItem(Sprite itemSprite, string description = "")
    {
        if (itemSprite == null || items == null)
            return;

        items.Add(itemSprite);

        if (!string.IsNullOrEmpty(description))
        {
            itemDescriptions[itemSprite] = description;
        }

        UpdateInventoryUI();
        Debug.Log($"[InventoryManager] Додано предмет: {itemSprite.name}");
    }

    private void UpdateInventoryUI()
    {
        // Оновлення UI слотів
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slots[i].sprite = items[i];
            }
            else
            {
                slots[i].sprite = emptySlotSprite;
            }
        }
    }
}
