using UnityEngine;
using System.Collections;

public class CatFeedAnimation : MonoBehaviour
{
    [Header("Об'єкти")]
    public Animator handAnimator;      
    public GameObject catIdle;         
    public GameObject catFeed;         
    public SpriteRenderer catFeedRenderer; 
    public Sprite catEating;           
    public Sprite catClosedMouth;     

    [Header("Тривалість")]
    public float feedDuration = 4f; // загальна тривалість
    public float switchTime = 1f; // час показу кожного спрайта

    private Coroutine switchCoroutine;

    public void OnFeedButton()
    {
        StartCoroutine(FeedSequence());
    }

    private IEnumerator FeedSequence()
    {
        // сховати idle кота
        if (catIdle != null) catIdle.SetActive(false);

        // показати кота під час їжі
        if (catFeed != null) catFeed.SetActive(true);

        // активувати анімацію руки
        if (handAnimator != null)
        {
            handAnimator.gameObject.SetActive(true);
            handAnimator.Play("FeedWithHand");
        }

        // запустити чергування спрайтів кота
        if (catFeedRenderer != null && catEating != null && catClosedMouth != null)
        {
            if (switchCoroutine != null) StopCoroutine(switchCoroutine);
            switchCoroutine = StartCoroutine(SwitchCatSprites());
        }

        // чекати загальну тривалість
        yield return new WaitForSeconds(feedDuration);

        // зупинити чергування
        if (switchCoroutine != null)
        {
            StopCoroutine(switchCoroutine);
            switchCoroutine = null;
        }

        // повернути idle кота
        if (catFeed != null) catFeed.SetActive(false);
        if (catIdle != null) catIdle.SetActive(true);

        // вимкнути руку
        if (handAnimator != null) handAnimator.gameObject.SetActive(false);
    }

    private IEnumerator SwitchCatSprites()
    {
        while (true)
        {
            catFeedRenderer.sprite = catEating;
            yield return new WaitForSeconds(switchTime);

            catFeedRenderer.sprite = catClosedMouth;
            yield return new WaitForSeconds(switchTime);
        }
    }
}
