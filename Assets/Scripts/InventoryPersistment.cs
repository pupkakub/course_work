using UnityEngine;

public class InventoryPersistent : MonoBehaviour
{
    private static InventoryPersistent instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Canvas залишатиметься між сценами
        }
        else
        {
            Destroy(gameObject); // щоб не створювати дублікат
        }
    }
}
