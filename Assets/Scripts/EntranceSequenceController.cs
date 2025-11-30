using UnityEngine;
using System.Collections;

public class EntranceSequenceController : MonoBehaviour
{
    [Header("References")]
    public EntranceDoor door;
    public GameObject gena;
    public GameObject dasha;
    public GameObject lera;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;
    public DialogueNode[] entranceDialogue;

    [Header("Exit Settings - використовуємо LeraAutoMove")]
    public LeraAutoMove leraAutoMove; 

    private bool entranceCompleted = false;

    void Start()
    {
        //сховати персонажів на початку
        if (gena) gena.SetActive(false);
        if (dasha) dasha.SetActive(false);

        //автоматично знайти LeraAutoMove
        if (leraAutoMove == null)
        {
            if (lera != null)
            {
                leraAutoMove = lera.GetComponent<LeraAutoMove>();
                if (leraAutoMove == null)
                    Debug.LogWarning("LeraAutoMove не знайдено на об'єкті Лєри!");
                else
                    Debug.Log("LeraAutoMove автоматично знайдено на Лєрі");
            }
            else
            {
                //шукати на сцені
                leraAutoMove = FindFirstObjectByType<LeraAutoMove>();
                if (leraAutoMove != null)
                {
                    lera = leraAutoMove.gameObject;
                    Debug.Log("LeraAutoMove знайдено в сцені");
                }
                else
                {
                    Debug.LogError("LeraAutoMove не знайдено ні на Лєрі, ні в сцені");
                }
            }
        }
        else
        {
            Debug.Log("LeraAutoMove призначено вручну в Inspector");
        }
    }

    //ВХІД: відчинення дверей, поява Гени та Даші
    public void StartEntranceSequence()
    {
        if (entranceCompleted)
        {
            Debug.Log("Послідовність входу вже виконана.");
            return;
        }

        if (GameStateManager.Instance != null && GameStateManager.Instance.IsCatSequenceCompleted())
        {
            StartCoroutine(EntranceRoutine());
        }
        else
        {
            Debug.Log("Квест з котом ще не завершено, двері заблоковані.");
        }
    }

    private IEnumerator EntranceRoutine()
    {
        Debug.Log("Початок послідовності входу...");

        // відкрити двері
        if (door != null)
        {
            door.OpenDoor();
            yield return new WaitForSeconds(0.5f);
        }

        // показати Гену та Дашу
        if (gena)
        {
            gena.SetActive(true);
            Debug.Log("Гена з'явився");
        }
        if (dasha)
        {
            dasha.SetActive(true);
            Debug.Log("Даша з'явилась");
        }

        yield return new WaitForSeconds(1f);

        // зачинити двері
        if (door != null)
        {
            door.CloseDoor();
            Debug.Log("Двері закрито");
        }

        entranceCompleted = true;

        // запустити діалог
        if (dialogueManager != null && entranceDialogue != null && entranceDialogue.Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Запуск діалогу...");
            dialogueManager.StartDialogue(entranceDialogue);
        }
        else
        {
            Debug.LogWarning("DialogueManager або діалоги не налаштовані!");
        }
    }

    //ВИХІД: контрольована послідовність зникнення всіх персонажів
    public void StartExitSequence()
    {
        StartCoroutine(ExitSequenceRoutine());
    }

    private IEnumerator ExitSequenceRoutine()
    {
        Debug.Log("Початок послідовності виходу всіх персонажів...");

        if (leraAutoMove == null)
        {
            Debug.LogError("LeraAutoMove не призначено! Спроба знайти в сцені...");

            if (lera != null)
                leraAutoMove = lera.GetComponent<LeraAutoMove>();

            if (leraAutoMove == null)
                leraAutoMove = FindFirstObjectByType<LeraAutoMove>();

            if (leraAutoMove == null)
            {
                Debug.LogError("LeraAutoMove не знайдено! Вихід неможливий.");
                yield break;
            }
            else
            {
                Debug.Log("LeraAutoMove знайдено!");
                lera = leraAutoMove.gameObject;
            }
        }

        //запустити рух Лєри
        Debug.Log("Запуск LeraAutoMove.StartExitSequence()...");
        leraAutoMove.StartExitSequence();

        // вимкнення керування (0.3s) + відкриття дверей (0.5s) + рух + запас (0.5s)
        float waitTime = 0.3f + 0.5f + 0.5f; // вазові затримки

        // додати час руху до фіксованої точки (-7, -5)
        if (lera != null)
        {
            Vector3 targetPos = leraAutoMove.targetPosition; //фіксована точка
            float distance = Vector3.Distance(lera.transform.position, targetPos);
            float moveTime = distance / leraAutoMove.moveSpeed;
            waitTime += moveTime;
            Debug.Log($"Розрахований час очікування: {waitTime:F1}s (рух: {moveTime:F1}s, відстань: {distance:F2}m до {targetPos})");
        }
        else
        {
            // якщо не можна розрахувати - дати більше часу
            waitTime += 3f;
            Debug.LogWarning($"Не можу розрахувати час руху, використовую фіксований: {waitTime:F1}s");
        }

        Debug.Log($"Чекаємо {waitTime:F1} секунд...");
        yield return new WaitForSeconds(waitTime);

        // Лєра біля дверей - ховаємо ВСІХ
        Debug.Log("Зникнення всіх персонажів...");

        if (lera)
        {
            lera.SetActive(false);
            Debug.Log("Лєра зникла");
        }

        if (gena)
        {
            gena.SetActive(false);
            Debug.Log("Гена зник");
        }

        if (dasha)
        {
            dasha.SetActive(false);
            Debug.Log("Даша зникла");
        }

        yield return new WaitForSeconds(0.3f);

        // закрити двері
        if (door != null)
        {
            door.CloseDoor();
            Debug.Log("Двері ЗАКРИТО");
        }
        else
        {
            Debug.LogWarning("Посилання на двері відсутнє в EntranceSequenceController!");
        }

        Debug.Log("Послідовність виходу завершена!");
    }
}