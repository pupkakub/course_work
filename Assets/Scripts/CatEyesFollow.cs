using UnityEngine;

public class CatEyesFollow : MonoBehaviour
{
    [Header("Ока кота")]
    public Transform leftEye;
    public Transform rightEye;

    [Header("Обмеження руху очей")]
    public float maxDistance = 0.1f; // наскільки очі можуть рухатися від центру

    [Header("Ціль для спостереження")]
    public Transform target; // за чим дивиться кіт (рука)

    private Vector3 leftEyeStartPos;
    private Vector3 rightEyeStartPos;

    void Start()
    {
        if (leftEye == null || rightEye == null || target == null)
        {
            Debug.LogError("CatEyesFollow: Не всі Transform призначені!");
            return;
        }

        // зберігаємо початкові позиції очей
        leftEyeStartPos = leftEye.localPosition;
        rightEyeStartPos = rightEye.localPosition;
    }

    void Update()
    {
        Vector3 direction = target.position - transform.position;
        direction = Vector3.ClampMagnitude(direction, 1f); // нормалізація напрямку

        // множимо на maxDistance, щоб очі не рухались занадто далеко
        Vector3 offset = new Vector3(direction.x, direction.y, 0) * maxDistance;

        // оновлюємо позиції очей відносно стартових
        leftEye.localPosition = leftEyeStartPos + offset;
        rightEye.localPosition = rightEyeStartPos + offset;
    }
}
