using UnityEngine;
using System.Collections;

public class PlayButtonHandler : MonoBehaviour
{
    [Header("Об'єкти кота")]
    public GameObject catPlay;       
    public GameObject catIdle;    
    public Animator handAnimator;   
    public CatEyesFollow eyesFollow; 

    [Header("Тривалість гри")]
    public float playDuration = 4f;

    public void OnPlayButton()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // початок гри
        if (catPlay != null) catPlay.SetActive(true);
        if (catIdle != null) catIdle.SetActive(false);

        if (handAnimator != null) handAnimator.gameObject.SetActive(true);
        if (eyesFollow != null) eyesFollow.enabled = true;

        // запустити анімацію руки
        if (handAnimator != null)
            handAnimator.Play("play_with_cat"); 

        // чекати задану тривалість
        yield return new WaitForSeconds(playDuration);
        // повернення у спокійний режим
        if (catPlay != null) catPlay.SetActive(false);
        if (catIdle != null) catIdle.SetActive(true);

        if (handAnimator != null) handAnimator.gameObject.SetActive(false);

        // очі кота у спокійному режимі залишаються активними для кліпання
        if (eyesFollow != null) eyesFollow.enabled = true;

    }

    private void Start()
    {
        // за замовчуванням показуємо idle
        if (catIdle != null) catIdle.SetActive(true);
        if (catPlay != null) catPlay.SetActive(false);

        if (handAnimator != null) handAnimator.gameObject.SetActive(false);
    }

}
