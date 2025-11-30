using UnityEngine;
using System.Collections;

public class PetButtonHandler : MonoBehaviour
{
    [Header("Об'єкти")]
    public Animator handAnimator;      
    public GameObject catIdle;        
    public GameObject catPet;          
    public SpriteRenderer catPetSprite; 
    public Sprite heartsFew;            
    public Sprite heartsMany;          

    [Header("Тривалість")]
    public float petDuration = 4f;
    public float switchTime = 2f; // час показу кожного спрайту

    public void OnPetButton()
    {
        StartCoroutine(PetSequence());
    }

    private IEnumerator PetSequence()
    {
        // сховати idle кота
        if (catIdle != null) catIdle.SetActive(false);

        // показати кота з сердечками
        if (catPet != null) catPet.SetActive(true);

        // активувати анімацію руки
        if (handAnimator != null)
        {
            handAnimator.gameObject.SetActive(true);
            handAnimator.Play("PetWithHand");
        }

        // чергування сердечок
        float elapsed = 0f;
        while (elapsed < petDuration)
        {
            if (catPetSprite != null && heartsFew != null && heartsMany != null)
            {
                catPetSprite.sprite = heartsFew;
                yield return new WaitForSeconds(switchTime);

                catPetSprite.sprite = heartsMany;
                yield return new WaitForSeconds(switchTime);
            }
            elapsed += switchTime * 2f;
        }

        // вимкнути кота з сердечками
        if (catPet != null) catPet.SetActive(false);

        // повернути idle кота
        if (catIdle != null) catIdle.SetActive(true);

        // вимкнути анімацію руки
        if (handAnimator != null) handAnimator.gameObject.SetActive(false);
    }
}
