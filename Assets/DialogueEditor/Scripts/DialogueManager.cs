using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    //Reference to the first dialogue
    public Dialogue initialDialogue;
    Dialogue currentDialogue;

	void Start ()
    {
        if (initialDialogue != null) PlayDialogue(initialDialogue);
		
	}
	

    void PlayDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        //The funcion should load the text of each node in the Dialogue, including the character's name
        //In case it's a question the system should spawn as many buttons as necesary and link to the required node
    }


}
