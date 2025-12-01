using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TMP_Text[] choiceButtonTexts;

    [Header("Dialogue Data")]
    public DialogueNode[] nodes;

    [Header("Sequence Controller")]
    public EntranceSequenceController entranceSequenceController; // новий контролер для керування входом/виходом Лєри

    private int currentNodeIndex = 0;
    private bool waitingForChoice = false;
    private bool eventInProgress = false;
    private bool _dialogueEnded = false;

    private Coroutine waitCoroutine = null;
    private int dialogueVersion = 0; // інкрементується при кожному запуску діалогу

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    public void StartDialogue(DialogueNode[] newNodes)
    {
        if (newNodes == null || newNodes.Length == 0) return;

        nodes = newNodes;
        currentNodeIndex = 0;
        _dialogueEnded = false;
        dialogueVersion++;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        ShowNode(currentNodeIndex);
    }

    void ShowNode(int index)
    {
        if (nodes == null || nodes.Length == 0 || index < 0 || index >= nodes.Length || _dialogueEnded) return;

        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        dialogueVersion++;

        DialogueNode node = nodes[index];
        currentNodeIndex = index;
        dialogueText.text = $"{node.speaker}: {node.message}";

        if (node.isChoice)
        {
            waitingForChoice = true;
            ShowChoices(node);
        }
        else
        {
            waitingForChoice = false;
            if (choicePanel != null) choicePanel.SetActive(false);
            waitCoroutine = StartCoroutine(NextAfterDelay(2f, dialogueVersion));
        }

        if (!string.IsNullOrEmpty(node.eventName) && !eventInProgress)
        {
            StartCoroutine(HandleEventCoroutine(node.eventName));
        }
    }

    IEnumerator NextAfterDelay(float delay, int version)
    {
        float t = 0f;
        while (t < delay)
        {
            if (_dialogueEnded || version != dialogueVersion) yield break;
            t += Time.deltaTime;
            yield return null;
        }

        while (eventInProgress)
        {
            if (_dialogueEnded || version != dialogueVersion) yield break;
            yield return null;
        }

        if (_dialogueEnded || version != dialogueVersion) yield break;

        if (!waitingForChoice)
        {
            waitCoroutine = null;
            NextNode();
        }
    }

    void ShowChoices(DialogueNode node)
    {
        if (choicePanel == null || choiceButtons == null || choiceButtons.Length == 0) return;

        choicePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = $"{node.speaker}: {node.message}\n\n<color=#FFFF00>Оберіть відповідь:</color>";

        if (node.options == null) node.options = new string[0];
        int optionCount = Mathf.Min(node.options.Length, choiceButtons.Length);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button btn = choiceButtons[i];
            if (btn == null) continue;

            if (i < optionCount)
            {
                btn.gameObject.SetActive(true);
                TMP_Text btnText = (choiceButtonTexts != null && i < choiceButtonTexts.Length && choiceButtonTexts[i] != null)
                                    ? choiceButtonTexts[i]
                                    : btn.GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = node.options[i];

                int choiceIndex = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    waitingForChoice = false;
                    if (waitCoroutine != null)
                    {
                        StopCoroutine(waitCoroutine);
                        waitCoroutine = null;
                    }
                    OnChoiceSelected(choiceIndex, node);
                });
            }
            else
            {
                btn.onClick.RemoveAllListeners();
                btn.gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(int choiceIndex, DialogueNode node)
    {
        if (choicePanel != null) choicePanel.SetActive(false);

        if (node.nextNodeIndex != null && choiceIndex < node.nextNodeIndex.Length)
        {
            int nextIndex = node.nextNodeIndex[choiceIndex];
            if (nextIndex < 0 || nextIndex >= nodes.Length)
            {
                EndDialogue();
                return;
            }
            ShowNode(nextIndex);
        }
        else
        {
            EndDialogue();
        }
    }

    public void NextNode()
    {
        if (waitingForChoice || _dialogueEnded) return;

        currentNodeIndex++;
        if (nodes != null && currentNodeIndex < nodes.Length)
        {
            ShowNode(currentNodeIndex);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if (_dialogueEnded) return;
        _dialogueEnded = true;
        dialogueVersion++;

        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (choicePanel) choicePanel.SetActive(false);
        if (dialogueText) dialogueText.text = "";

        // викликати лише Sequence Controller
        if (entranceSequenceController != null)
        {
            entranceSequenceController.StartExitSequence();
        }
        else
        {
            Debug.LogWarning("entranceSequenceController не призначено в DialogueManager!");
        }
    }

    IEnumerator HandleEventCoroutine(string eventName)
    {
        eventInProgress = true;

        if (eventName == "GenaLeave")
        {
            var gena = FindFirstObjectByType<GenaController>();
            if (gena != null)
                yield return StartCoroutine(gena.LeaveRoomSequence());
        }
        else if (eventName == "EndEpisode")
        {
            yield return new WaitForSeconds(0.5f);
            EndDialogue();
            eventInProgress = false;
            yield break;
        }
        else if (eventName == "NoChoice")
        {
            yield return new WaitForSeconds(0.5f);
        }

        eventInProgress = false;
    }
}
