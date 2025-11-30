using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("фони")]
    public GameObject bg1;
    public GameObject bg2;

    private PlayerMovementInputSystem player;
    private CameraFollow camFollow;

    void Start()
    {
        StartCoroutine(InitializeAfterFrame());
    }

    IEnumerator InitializeAfterFrame()
    {
        yield return new WaitForEndOfFrame();

        if (bg1 == null || bg2 == null)
        {
            Debug.LogError("bg1 або bg2 не призначені в Inspector");
            yield break;
        }

        player = FindFirstObjectByType<PlayerMovementInputSystem>();
        camFollow = FindFirstObjectByType<CameraFollow>();

        Renderer r1 = bg1.GetComponent<Renderer>();
        Renderer r2 = bg2.GetComponent<Renderer>();

        if (r1 == null || r2 == null)
        {
            Debug.LogError("bg1 або bg2 не мають Renderer");
            yield break;
        }

        Bounds b1 = r1.bounds;
        Bounds b2 = r2.bounds;

        float worldMinX = Mathf.Min(b1.min.x, b2.min.x);
        float worldMaxX = Mathf.Max(b1.max.x, b2.max.x);

        Debug.Log($"BackgroundManager: worldMinX={worldMinX}, worldMaxX={worldMaxX}");

        if (player != null)
        {
            player.SetMovementLimits(worldMinX, worldMaxX);
        }

        if (camFollow != null)
        {
            camFollow.SetBounds(worldMinX, worldMaxX);

            if (player != null)
            {
                camFollow.SetTarget(player.transform);
            }
        }
    }

    public void RegisterPlayer(PlayerMovementInputSystem newPlayer)
    {
        player = newPlayer;
        StartCoroutine(ReapplyLimits());
    }

    IEnumerator ReapplyLimits()
    {
        yield return new WaitForEndOfFrame();

        if (bg1 == null || bg2 == null)
        {
            Debug.LogWarning("bg1 або bg2 відсутні");
            yield break;
        }

        Renderer r1 = bg1.GetComponent<Renderer>();
        Renderer r2 = bg2.GetComponent<Renderer>();

        if (r1 == null || r2 == null) yield break;

        Bounds b1 = r1.bounds;
        Bounds b2 = r2.bounds;

        float worldMinX = Mathf.Min(b1.min.x, b2.min.x);
        float worldMaxX = Mathf.Max(b1.max.x, b2.max.x);

        if (player != null)
        {
            player.SetMovementLimits(worldMinX, worldMaxX);
        }

        if (camFollow == null)
        {
            camFollow = FindFirstObjectByType<CameraFollow>();
        }

        if (camFollow != null)
        {
            camFollow.SetBounds(worldMinX, worldMaxX);

            if (player != null)
            {
                camFollow.SetTarget(player.transform);
            }
        }
    }
}