using UnityEngine;

[System.Serializable]

public class DialogueNode
{
    public string speaker;
    public string message;
    public bool isChoice;
    public string[] options;
    public int[] nextNodeIndex;
    public string eventName;

    public bool isEndNode = false;
}