using UnityEngine;
using System.Collections;

public class CatPetHearts : MonoBehaviour
{
    [Header("Спрайти кота під час погладжування")]
    public SpriteRenderer catSpriteRenderer; // SpriteRenderer самого GameObject catPet
    public Sprite heartsFew; 
    public Sprite heartsMany; 

    public float switchTime = 2f; // час показу кожного спрайта

    private Coroutine switchCoroutine;

    // запуск чергування
    public void StartHeartsSwitch()
    {
        if (switchCoroutine != null)
            StopCoroutine(switchCoroutine);

        switchCoroutine = StartCoroutine(SwitchHearts());
    }

    // зупинити чергування
    public void StopHeartsSwitch()
    {
        if (switchCoroutine != null)
            StopCoroutine(switchCoroutine);

        switchCoroutine = null;
    }

    private IEnumerator SwitchHearts()
    {
        if (catSpriteRenderer == null) yield break;

        while (true)
        {
            catSpriteRenderer.sprite = heartsFew;
            yield return new WaitForSeconds(switchTime);

            catSpriteRenderer.sprite = heartsMany;
            yield return new WaitForSeconds(switchTime);
        }
    }
}
