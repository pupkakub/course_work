using UnityEngine;

public class DontDestroyUI : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.name = "PersistentInventoryCanvas"; // щоб легко знаходити
    }
}