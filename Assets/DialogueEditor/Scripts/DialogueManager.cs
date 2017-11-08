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
        
        characterName.text = dialogue.name+":";
        dialogueText.text = LocalizationManager.Localize(dialogue.locKey);

        if (dialogue.answers.Count<2)
        {
            optionA.gameObject.SetActive(false);
            optionB.gameObject.SetActive(false);
        }
        else
        {
            optionA.gameObject.SetActive(true);
            optionB.gameObject.SetActive(true);
        }


    }

    void EndDialogue()
    {
        gameObject.SetActive(false);
        /*
        characterName.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        optionA.gameObject.SetActive(false);
        optionB.gameObject.SetActive(false);
        */
        Debug.Log("End of the Dialogue");
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(currentDialogue.nextDialogue!=null)
            {
                PlayDialogue(currentDialogue.nextDialogue);
            }
            else
            {
                EndDialogue();
            }
        }
    }



}
