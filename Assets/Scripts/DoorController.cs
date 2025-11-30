using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public float animationDelay = 0.1f; // скільки триває анімація відкриття/закриття

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public IEnumerator OpenDoorCoroutine()
    {
        if (openSprite != null)
            sr.sprite = openSprite;

        // тут можна буде додати звук чи анімацію
        yield return new WaitForSeconds(animationDelay);
    }

    public IEnumerator CloseDoorCoroutine()
    {
        if (closedSprite != null)
            sr.sprite = closedSprite;

        yield return new WaitForSeconds(animationDelay);
    }

    public void OpenAndClose(float closeDelay = 0.3f)
    {
        StartCoroutine(OpenAndAutoClose(closeDelay));
    }

    private IEnumerator OpenAndAutoClose(float closeDelay)
    {
        if (openSprite != null)
            sr.sprite = openSprite;

        yield return new WaitForSeconds(closeDelay);

        if (closedSprite != null)
            sr.sprite = closedSprite;
    }
}
