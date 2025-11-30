using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public DialogueManager manager;
    public DialoguePreset_Dasha dialoguePreset;
    public TMP_Text interactHintText;

    private bool playerInRange = false;
    private bool dialogueTriggered = false; //прапорець одноразової активації

    private void Start()
    {
        if (interactHintText != null)
            interactHintText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            //показ підказки тільки якщо діалог ще не був активований
            if (interactHintText != null && !dialogueTriggered)
                interactHintText.gameObject.SetActive(true);

            Debug.Log("Player entered trigger zone");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactHintText != null)
                interactHintText.gameObject.SetActive(false);
            Debug.Log("Player exited trigger zone");
        }
    }

    private void Update()
    {
        //діалог активується тільки якщо він ще не був запущений
        if (playerInRange && !dialogueTriggered && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E pressed - trying to start dialogue");

            if (manager == null)
            {
                Debug.LogError("DialogueManager not assigned!");
                return;
            }

            if (dialoguePreset == null || dialoguePreset.nodes == null || dialoguePreset.nodes.Length == 0)
            {
                Debug.LogError("DialoguePreset is missing or empty!");
                return;
            }

            //сховати підказку
            if (interactHintText != null)
                interactHintText.gameObject.SetActive(false);

            //встановити прапорець, що діалог вже активовано
            dialogueTriggered = true;

            Debug.Log("Calling StartDialogue() with preset: " + dialoguePreset.name);
            manager.StartDialogue(dialoguePreset.nodes);
        }
    }

    //опціональний метод для скидання (якщо потрібно буде повторно активувати діалог)
    public void ResetTrigger()
    {
        dialogueTriggered = false;
        Debug.Log("DialogueTrigger reset - can be triggered again");
    }
}