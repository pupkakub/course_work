using UnityEngine;
using System.Collections;

public class GenaController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;           
    public Transform leaveTarget;       
    public DoorController door;         

    [Header("Settings")]
    public float walkSpeed = 2f; // швидкість ходьби
    public float doorPauseTime = 0.3f; // пауза перед відкриттям дверей

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // викликається з DialogueManager через eventName="GenaLeave"
    public IEnumerator LeaveRoomSequence()
    {
        Debug.Log("Gena starts leaving sequence");

        // примусово увімкнути стан ходьби
        if (animator != null)
        {
            //  одразу перейти в цей стан (уникнути переходів з Exit Time)
            animator.Play("Walk", 0, 0f); 
            animator.SetBool("isWalking", true);
            Debug.Log("Animator forced to Walk state");

            // примусово оновити animator один раз до фіксованого апдейту,
            // щоб візуально відобразити ходьбу перед першим переміщенням.
            // виклик animator.Update(0f) тут дозволяє відразу застосувати стан.
            animator.Update(0f);
        }

        // дати фізиці/анімації час — краще чекати FixedUpdate перед початком руху
        yield return new WaitForFixedUpdate();

        // йти до leaveTarget 
        if (leaveTarget != null)
        {
            Debug.Log($"Moving towards door at {leaveTarget.position}");

            // якщо об'єкті є Rigidbody2D — використовувати MovePosition для плавної роботи з фізикою
            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            Rigidbody rb3d = GetComponent<Rigidbody>();

            if (rb2d != null)
            {
                // використовувати MovePosition у циклі з WaitForFixedUpdate для точності фізики
                while (Vector2.Distance(rb2d.position, (Vector2)leaveTarget.position) > 0.05f)
                {
                    Vector2 newPos = Vector2.MoveTowards(rb2d.position, (Vector2)leaveTarget.position, walkSpeed * Time.fixedDeltaTime);
                    rb2d.MovePosition(newPos);
                    yield return new WaitForFixedUpdate();
                }
                Debug.Log("Reached door (Rigidbody2D)");
            }
            else if (rb3d != null)
            {
                while (Vector3.Distance(rb3d.position, leaveTarget.position) > 0.05f)
                {
                    Vector3 newPos = Vector3.MoveTowards(rb3d.position, leaveTarget.position, walkSpeed * Time.fixedDeltaTime);
                    rb3d.MovePosition(newPos);
                    yield return new WaitForFixedUpdate();
                }
                Debug.Log("Reached door (Rigidbody)");
            }
            else
            {
                // без Rigidbody — використовувати transform, але теж чекати FixedUpdate щоб синхронізувати з анімацією
                while (Vector2.Distance(transform.position, leaveTarget.position) > 0.05f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, leaveTarget.position, walkSpeed * Time.deltaTime);
                    yield return null;
                }
                Debug.Log("Reached door (Transform)");
            }
        }
        else
        {
            Debug.LogWarning("leaveTarget not assigned!");
        }

        // зупинити анімацію ходьби
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            // можна відразу переключити в Idle
            animator.Play("Idle", 0, 0f); // якщо є такий state
            animator.Update(0f);
            Debug.Log("Walking animation stopped");
        }

        // коротка пауза перед відкриттям дверей
        yield return new WaitForSeconds(doorPauseTime);

        // відчинити двері 
        if (door != null)
        {
            Debug.Log("Opening door");
            door.OpenAndClose(0.3f);
        }
        else
        {
            Debug.LogWarning("Door controller not assigned!");
        }

        // коротка пауза
        yield return new WaitForSeconds(0.2f);

        // приховати Гену 
        gameObject.SetActive(false);
        Debug.Log("Gena hidden (left the room)");
    }


}
