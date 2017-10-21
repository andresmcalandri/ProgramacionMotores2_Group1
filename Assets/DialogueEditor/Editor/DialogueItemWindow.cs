using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueItemWindow  {

	DialogueEditor _parent;
	DialogueItem _dialogue;

	public int id {
		get {
			return _dialogue.id;
		}
	}
		
	public Rect rect {
		get {
			return _dialogue.rect;
		}
		set {
			_dialogue.rect = value;
		}
	}

    public DialogueItem dialogue {
        get {
            return _dialogue;
        }
    }

	public DialogueItemWindow(DialogueItem dialogueItem, DialogueEditor parent) 
	{
		_dialogue = dialogueItem;
		_parent = parent;

		//Set default rect if its unitialized
		if(_dialogue.rect == Rect.zero)
			_dialogue.rect = new Rect (10, 10, 200, 150);
	}

	public void Draw(int windowID)
	{
		if (GUI.Button(new Rect(190, 0, 10, 10), "X"))
		{
			_parent.DeleteControl(this);
		}

		if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
			Debug.Log("Hello World");

		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}
}