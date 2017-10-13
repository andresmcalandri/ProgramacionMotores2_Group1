using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DialogueEditor : EditorWindow {
    
    [MenuItem("DialogueEditor/Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor));
    }

	//TODO Create a button to create a new dialogue
	Dialogue dialogue = null;
	List<DialogueItemWindow> controls = new List<DialogueItemWindow>();

	//TODO Create an id management that actually works.
	int dialogueCount = 0;

    void OnGUI()
    {
        if (GUILayout.Button("Create Window"))
            CreateControl();

        BeginWindows();

        for (int i = 0; i < controls.Count; i++)
        {
			controls[i].rect = GUI.Window(i, controls[i].rect, controls[i].Draw, "Window_" + i);
        }

        EndWindows();
    }
		

    public void CreateControl()
    {
		DialogueItem dialogueItem = ScriptableObject.CreateInstance<DialogueItem> ();
		dialogueItem.id = dialogueCount++;
			
		DialogueItemWindow newWindow = new DialogueItemWindow (dialogueItem, this);
		controls.Add(newWindow);
    }

	public void DeleteControl(DialogueItemWindow window)
    {
		controls.Remove(window);
    }


	//TODO Load a dialogue asset
	void LoadDialogue()
	{

	}

	//TODO Save a dialogue asset
	void SaveDialogue()
	{
		
	}
}
