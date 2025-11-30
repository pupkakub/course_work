using UnityEngine;
using System.Collections;

public class LeraAutoMove : MonoBehaviour
{
    [Header("Target & Movement")]
    [SerializeField] private Vector3 fixedTargetPosition = new Vector3(-7f, -5f, 0f); // фіксована цільова точка
    [SerializeField] private float _moveSpeed = 2f;

    [Header("Animation")]
    [SerializeField] private Animator animator; 
    [SerializeField] private string walkAnimationParameter = "lera_walk"; 

    [Header("Door Reference")]
    [SerializeField] private EntranceDoor _entranceDoor;

    [Header("Player Controller")]
    [SerializeField] private MonoBehaviour _playerMovementController;
    private Transform _targetPosition;

    public Vector3 targetPosition
    {
        get { return fixedTargetPosition; }
        set { fixedTargetPosition = value; }
    }

    public float moveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    public EntranceDoor entranceDoor
    {
        get { return _entranceDoor; }
        set { _entranceDoor = value; }
    }

    public MonoBehaviour playerMovementController
    {
        get { return _playerMovementController; }
        set { _playerMovementController = value; }
    }

    private Rigidbody2D rb;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log($"[{gameObject.name}] LeraAutoMove готовий (фінальна ініціалізація при виклику StartExitSequence)");
    }

    public void StartExitSequence()
    {
        if (isMoving)
        {
            Debug.Log($"[{gameObject.name}] вже рухається!");
            return;
        }

        // ініціалізація прямо перед стартом
        InitializeReferences();
        StartCoroutine(ExitSequence());
    }

    private void InitializeReferences()
    {
        // пошук дверей
        if (_entranceDoor == null)
        {
            _entranceDoor = FindFirstObjectByType<EntranceDoor>();
            if (_entranceDoor != null)
                Debug.Log($"[{gameObject.name}] EntranceDoor знайдено: {_entranceDoor.gameObject.name}");
            else
                Debug.LogError($"[{gameObject.name}] EntranceDoor НЕ ЗНАЙДЕНО в сцені!");
        }

        // створення цільової точки
        if (_targetPosition == null)
        {
            GameObject targetObj = new GameObject($"{gameObject.name}_ExitTarget");
            targetObj.transform.position = fixedTargetPosition;
            _targetPosition = targetObj.transform;
            Debug.Log($"[{gameObject.name}] Створено targetPosition: {_targetPosition.position}");
        }


        // перевірка Rigidbody2D
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        Debug.Log($"[{gameObject.name}] Перевірка посилань:");
        Debug.Log($"   Target: {(_targetPosition != null ? $"{_targetPosition.position}" : "MISSING")}");
        Debug.Log($"   Door: {(_entranceDoor != null ? _entranceDoor.gameObject.name : "MISSING")}");
        Debug.Log($"   Controller: {(_playerMovementController != null ? _playerMovementController.GetType().Name : "MISSING")}");
    }

    private IEnumerator ExitSequence()
    {
        isMoving = true;

        // вимкнути керування гравцем
        if (_playerMovementController != null)
            _playerMovementController.enabled = false;

        yield return new WaitForSeconds(0.3f);

        // відкрити двері
        if (_entranceDoor != null)
        {
            _entranceDoor.OpenDoor();
            Debug.Log($"[{gameObject.name}] Двері відкрито");
            yield return new WaitForSeconds(0.5f);
        }

        // рух до цільової точки
        if (_targetPosition != null)
        {
            // примусово включити ходьбу через speed
            if (animator != null)
                animator.SetFloat("Speed", 5f); // >0.1, щоб перейти у Walk

            Vector3 target = _targetPosition.position;
            Vector3 direction = (target - transform.position).normalized;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                if (rb != null)
                    rb.linearVelocity = direction * _moveSpeed; // рух через Rigidbody2D
                else
                    transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);

                yield return null;
            }

            // зупинити
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // повернути Animator у Idle
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] targetPosition не задано!");
        }

        yield return new WaitForSeconds(0.3f);

        isMoving = false;
        Debug.Log($"[{gameObject.name}] ExitSequence завершено");
    }

    public void RestorePlayerControl()
    {
        if (_playerMovementController != null)
        {
            _playerMovementController.enabled = true;
            Debug.Log($"[{gameObject.name}] Керування відновлено");
        }
    }
}