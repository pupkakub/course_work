using UnityEngine;

public class CatBlink : MonoBehaviour
{
    [Header("Спрайти кота")]
    public Sprite catOpenEyes;  
    public Sprite catClosedEyes; 

    [Header("Інтервали")]
    public float openEyesDuration = 4f;   
    public float closedEyesDuration = 1f; 

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool eyesOpen = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("CatBlink: На GameObject немає SpriteRenderer!");
        }

        spriteRenderer.sprite = catOpenEyes;
        timer = 0f;
        eyesOpen = true;
    }

    void Update()
    {
        if (catOpenEyes == null || catClosedEyes == null) return;

        timer += Time.deltaTime;

        if (eyesOpen && timer >= openEyesDuration)
        {
            // міняти на закриті очі
            spriteRenderer.sprite = catClosedEyes;
            eyesOpen = false;
            timer = 0f;
        }
        else if (!eyesOpen && timer >= closedEyesDuration)
        {
            // повертати на відкриті очі
            spriteRenderer.sprite = catOpenEyes;
            eyesOpen = true;
            timer = 0f;
        }
    }
}
