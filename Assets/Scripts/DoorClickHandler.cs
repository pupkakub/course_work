using UnityEngine;

public class DoorClickHandler : MonoBehaviour
{
    [Header("References")]
    public EntranceSequenceController sequenceController;

    [Header("Settings")]
    public float interactionDistance = 2f; // відстань для взаємодії

    private Transform player;
    private bool sequenceStarted = false;

    void Start()
    {
        //знайти мене 
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Не знайдено об'єкт з тегом 'Player'!");

        if (sequenceController == null)
            Debug.LogError("EntranceSequenceController не призначено!");
    }

    void OnMouseDown()
    {
        // перевірити клік по дверях
        TryInteract();
    }

    void Update()
    {
        // альтернативний варіант: натискання E біля дверей
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerNearby())
        {
            TryInteract();
        }
    }

    private bool IsPlayerNearby()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= interactionDistance;
    }

    private void TryInteract()
    {
        // перевірити чи завершено квест з котом
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager не знайдено!");
            return;
        }

        if (!GameStateManager.Instance.IsCatSequenceCompleted())
        {
            Debug.Log("Квест з котом ще не завершено. Двері заблоковані.");
            return;
        }

        // запустити послідовність входу тільки один раз
        if (!sequenceStarted && sequenceController != null)
        {
            sequenceStarted = true;
            Debug.Log("🚪 Запуск послідовності входу Гени та Даші...");
            sequenceController.StartEntranceSequence();
        }
        else
        {
            Debug.Log("⚠️ Послідовність вже запущена або sequenceController відсутній.");
        }
    }

    // візуалізація зони взаємодії в редакторі
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}