using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public string currentFolder;

    //Reference to the first dialogue
    public DialogueItem initialDialogue;
    DialogueItem currentDialogue;

    public Text characterName;
    public Text dialogueText;

    public Button optionA;
    public Button optionB;
    public Button optionC;
    public Button optionD;

    List<DialogueItem> dialogues;



    void Start ()
    {
        dialogues = new List<DialogueItem>(Resources.LoadAll<DialogueItem>("Dialogues/" + currentFolder));
        if (initialDialogue==null)
        {
            
            foreach(DialogueItem dialogue in dialogues)
            {
                if (initialDialogue == null) initialDialogue = dialogue;
                if (initialDialogue.id > dialogue.id) initialDialogue = dialogue;
            }
        }
        if (initialDialogue != null) PlayDialogue(initialDialogue);

        //if (GetComponent<Canvas>().worldCamera == null) GetComponent<Canvas>().worldCamera = GameObject.FindObjectOfType(Camera);
	}
	

    void PlayDialogue(DialogueItem dialogue)
    {

        if(dialogues==null)dialogues = new List<DialogueItem>(Resources.LoadAll<DialogueItem>("Dialogues/" + currentFolder));

        if (dialogue == null) return;

        currentDialogue = dialogue;
        //The funcion should load the text of each node in the Dialogue, including the character's name
        //In case it's a question the system should spawn as many buttons as necesary and link to the required node

        //TODO: Add characterName to the DialogueItem
        
        

        if(dialogue.name!="")
        {
            characterName.text = dialogue.name + ":";
        }
        else
        {
            characterName.text = "";
        }

        dialogueText.text = LocalizationManager.Localize(dialogue.locKey);

        if (dialogue.answers.Count==1)
        {
            optionA.gameObject.SetActive(false);
            optionB.gameObject.SetActive(false);
            optionC.gameObject.SetActive(false);
            optionD.gameObject.SetActive(false);

        }
        else if (dialogue.answers.Count == 2)
        {
            optionA.gameObject.SetActive(true);

            foreach(DialogueItem dial in dialogues)
            {
                if(dial.id== dialogue.answers[0])
                {
                    optionA.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionA.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }

            

            optionB.gameObject.SetActive(true);
            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[1])
                {
                    optionB.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionB.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }


            optionC.gameObject.SetActive(false);
            optionD.gameObject.SetActive(false);
        }

        else if (dialogue.answers.Count == 3)
        {
            optionA.gameObject.SetActive(true);

            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[0])
                {
                    optionA.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionA.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }



            optionB.gameObject.SetActive(true);
            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[1])
                {
                    optionB.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionB.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }

            optionC.gameObject.SetActive(true);
            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[2])
                {
                    optionC.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionC.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }


            optionD.gameObject.SetActive(false);
        }



        else if (dialogue.answers.Count == 4)
        {
            optionA.gameObject.SetActive(true);

            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[0])
                {
                    optionA.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionA.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }



            optionB.gameObject.SetActive(true);
            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[1])
                {
                    optionB.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionB.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }

            optionC.gameObject.SetActive(true);
            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[2])
                {
                    optionC.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionC.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }


            optionD.gameObject.SetActive(true);
            foreach (DialogueItem dial in dialogues)
            {
                if (dial.id == dialogue.answers[3])
                {
                    optionD.GetComponent<ButtonAnswer>().text.text = LocalizationManager.Localize(dial.locKey);
                    optionD.GetComponent<ButtonAnswer>().answerDialogue = dial;
                }
            }


        }
        else if (dialogue.answers.Count>4)
        {
            Debug.Log("Current system only supports up to 4 answers");
        }

    }


    public void NewDialogueFolder(string dialoguePath)
    {
        currentFolder = dialoguePath;
        dialogues = new List<DialogueItem>(Resources.LoadAll<DialogueItem>("Dialogues/" + currentFolder));
    }


    public void AnswerDialogue(GameObject button)
    {
        //Debug.Log("Boton pulsado, mensaje: " + button.GetComponent<ButtonAnswer>().answerDialogue.locKey);
        PlayDialogue(button.GetComponent<ButtonAnswer>().answerDialogue);
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
            //TODO: Actualmente se pone un answer más del que debería porque cuenta uno demás debido a que cuenta el que viene hacia el mismo
            if (currentDialogue.answers.Count == 1)
            {
                EndDialogue();
            }

            if (currentDialogue.answers.Count==2)
            {
                //Debug.Log("Next dialogue");
                PlayDialogue(currentDialogue.nextDialogue);
            }
        }
    }



}
