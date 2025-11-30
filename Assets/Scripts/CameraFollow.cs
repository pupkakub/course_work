using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 0.125f;
    private float minX = -10f;
    private float maxX = 10f;
    private float cameraHalfWidth;

    void Start()
    {
        // знайти персонажа при старті
        FindPlayer();

        if (Camera.main != null)
        {
            cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        }
    }

    private void FindPlayer()
    {
        var player = FindFirstObjectByType<PlayerMovementInputSystem>();
        if (player != null)
        {
            target = player.transform;
        }
    }

    void LateUpdate()
    {
        // якщо таргет втрачено, шукати знову
        if (target == null)
        {
            FindPlayer();
            if (target == null) return;
        }

        Vector3 desiredPos = new Vector3(target.position.x, transform.position.y, transform.position.z);
        desiredPos.x = Mathf.Clamp(desiredPos.x, minX + cameraHalfWidth, maxX - cameraHalfWidth);

        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }

    public void SetBounds(float left, float right)
    {
        minX = left;
        maxX = right;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}