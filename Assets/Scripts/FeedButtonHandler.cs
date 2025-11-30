using UnityEngine;
using System.Collections;

public class FeedButtonHandler : MonoBehaviour
{
    [Header("Об'єкти")]
    public Animator handAnimator; // аніматор руки з їжею
    public GameObject catIdle;          
    public GameObject catFeed;          
    public SpriteRenderer catFeedSprite; 
    public Sprite catEating;            
    public Sprite catClosedMouth;  

    [Header("Тривалість")]
    public float feedDuration = 4f; // загальна тривалість
    public float switchTime = 1f; // швидкість чергування спрайтів

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

        // чергування спрайтів кота
        float elapsed = 0f;
        while (elapsed < feedDuration)
        {
            if (catFeedSprite != null && catEating != null && catClosedMouth != null)
            {
                catFeedSprite.sprite = catEating;
                yield return new WaitForSeconds(switchTime);

                catFeedSprite.sprite = catClosedMouth;
                yield return new WaitForSeconds(switchTime);
            }
            elapsed += switchTime * 2f;
        }

        // вимкнути кота під час їжі
        if (catFeed != null) catFeed.SetActive(false);

        // повернути idle кота
        if (catIdle != null) catIdle.SetActive(true);

        // вимкнути анімацію руки
        if (handAnimator != null) handAnimator.gameObject.SetActive(false);
    }
}
