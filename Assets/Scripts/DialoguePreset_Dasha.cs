using UnityEngine;

public class DialoguePreset_Dasha : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public DialogueNode[] nodes = new DialogueNode[]
    {
        // node 0
        new DialogueNode {
            speaker="Даша",
            message="Хай. Ти покормила кота?",
            isChoice=false
        },
        
        // node 1
        new DialogueNode {
            speaker="Я",
            message="Так. Шо ви?",
            isChoice=false
        },
        
        // node 2 - Гена йде (викликається подія)
        new DialogueNode {
            speaker="Гена",
            message="Нічо. Я йду грати в Спайдермена.",
            isChoice=false,
            eventName="GenaLeave"
        },
        
        // node 3 - питання, вибір
        new DialogueNode {
            speaker="Даша",
            message="Нам треба продукти купити. Підеш зі мною в Новус?",
            isChoice=true,
            options=new string[] {"Та, пішли", "Та нє"},
            nextNodeIndex=new int[] {4, 5}  // варіант 1 > node 4, варіант 2 > node 5
        },
        
        // node 4 - відповідь "Та, пішли" (кінець)
        new DialogueNode {
            speaker="Я",
            message="Летс гоу",
            isChoice=false,
            eventName="LeraLeave",
            isEndNode=true   
        },

        // node 5 - Відповідь "Та нє" > nochoice > enddialogue
        new DialogueNode {
            speaker="Даша",
            message="У тебе немає вибору. Пішли",
            isChoice=false,
            eventName="LeraLeave",
            isEndNode=true   
        }
    };
}
