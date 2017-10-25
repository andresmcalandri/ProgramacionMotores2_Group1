using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    //Reference to the first dialogue
    public DialogueItem initialDialogue;
    DialogueItem currentDialogue;

    public Text characterName;
    public Text dialogueText;

    public Button optionA;
    public Button optionB;


    void Start ()
    {
        if (initialDialogue != null) PlayDialogue(initialDialogue);
        //if (GetComponent<Canvas>().worldCamera == null) GetComponent<Canvas>().worldCamera = GameObject.FindObjectOfType(Camera);
	}
	

    void PlayDialogue(DialogueItem dialogue)
    {
        currentDialogue = dialogue;
        //The funcion should load the text of each node in the Dialogue, including the character's name
        //In case it's a question the system should spawn as many buttons as necesary and link to the required node

        //TODO: Add characterName to the DialogueItem
        characterName.text = "";
        dialogueText.text = dialogue.name;

        if(dialogue.answers.Count==0)
        {
            optionA.gameObject.SetActive(false);
            optionB.gameObject.SetActive(false);
        }

    }


}
