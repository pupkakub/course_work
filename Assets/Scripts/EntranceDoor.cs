using UnityEngine;
using System.Collections;

public class EntranceDoor : MonoBehaviour
{
    [Header("Door Sprites")]
    public SpriteRenderer doorSpriteRenderer;
    public Sprite closedDoorSprite;
    public Sprite openDoorSprite;

    public bool IsOpen { get; private set; }

    public void OpenDoor()
    {
        if (doorSpriteRenderer && openDoorSprite)
            doorSpriteRenderer.sprite = openDoorSprite;

        IsOpen = true;
        Debug.Log("Door opened");
    }

    public void CloseDoor()
    {
        if (doorSpriteRenderer && closedDoorSprite)
            doorSpriteRenderer.sprite = closedDoorSprite;

        IsOpen = false;
        Debug.Log("Door closed");
    }

    //корутина для плавного відчинення дверей
    public IEnumerator OpenDoorRoutine(float delay = 0.0f)
    {
        if (doorSpriteRenderer != null && openDoorSprite != null)
            doorSpriteRenderer.sprite = openDoorSprite;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        else
            yield return null;
    }

    //корутина для плавного зачинення дверей
    public IEnumerator CloseDoorRoutine(float delay = 0.0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (doorSpriteRenderer != null && closedDoorSprite != null)
            doorSpriteRenderer.sprite = closedDoorSprite;

        yield return null;
    }
}
