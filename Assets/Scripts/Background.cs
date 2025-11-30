using UnityEngine;

public class FitBackground : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;

    void Start()
    {
        if (backgroundSprite == null) return;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = backgroundSprite.bounds.size.x / backgroundSprite.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = backgroundSprite.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = backgroundSprite.bounds.size.y / 2 * differenceInSize;
        }
    }
}
