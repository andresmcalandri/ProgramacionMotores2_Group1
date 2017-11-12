using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueItemWindow  {

	DialogueEditor _parent;
	DialogueItem _dialogue;

	string _localizedText;
	bool triedToLocalize = false;
	List<int> _fromIds = new List<int>();

	System.Action<DialogueItemWindow> _onConnectionClicked;

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

	public Rect orgRect {
		get {
			return _dialogue.orgRect;
		}
		set { 
			_dialogue.orgRect = value;
		}
	}

	public DialogueItemWindow(DialogueItem dialogueItem, DialogueEditor parent, System.Action<DialogueItemWindow> onConnectionClicked) 
	{
		_dialogue = dialogueItem;
		_parent = parent;
		_onConnectionClicked = onConnectionClicked;

		//Set default rect if its unitialized
		if(_dialogue.rect == Rect.zero)
			_dialogue.rect = new Rect (10, 10, 200, 150);
	}

	public void Draw(int windowID)
	{
		if (GUI.Button(new Rect(_dialogue.rect.width - 10, 0, 10, 10), "X"))
		{
			_parent.DeleteControl(this);
		}

		DrawLinks ();

		GUILayout.BeginArea (new Rect (30, 20, dialogue.rect.width - 60, dialogue.rect.height));
		EditorGUIUtility.labelWidth = 30;
		dialogue.locKey = EditorGUILayout.TextField ("Key", dialogue.locKey);

		if (!triedToLocalize) {
			_localizedText = LocalizationManager.Localize (dialogue.locKey);
			if (_localizedText == dialogue.locKey && !string.IsNullOrEmpty(dialogue.locKey))
				EditorUtility.DisplayDialog ("Localization Error", "Couldn't find key " + dialogue.locKey, "Ok");

			triedToLocalize = true;
		}
			
		EditorGUILayout.LabelField (_localizedText);

		GUILayout.EndArea ();

		GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}

	Rect _fromRect = new Rect (0, 20, 20, 20);
	Rect _toRect = new Rect (-20, 20, 20, 20);

	public void DrawLinks()
	{
		Rect fromRect = new Rect (_fromRect);

		for (int i = 0; i < _fromIds.Count; i++) {
			GUI.Button (fromRect, "0");
			fromRect.y += fromRect.height + 5;
		}

		if (GUI.Button (fromRect, "0") && _onConnectionClicked != null) {
			_onConnectionClicked (this);
		}

		Rect toRect = new Rect (rect.width + _toRect.x, _toRect.y, _toRect.width, _toRect.height);

		for (int i = 0; i < dialogue.answers.Count; i++) {
			if (GUI.Button (toRect, "0") && _onConnectionClicked != null)
				_onConnectionClicked (this);

			toRect.y += toRect.height + 5;
		}

		if (GUI.Button (toRect, "0") && _onConnectionClicked != null)
			_onConnectionClicked (this);
	}

	public Rect GetAnswerRect(int dialogueId)
	{
		if (!_fromIds.Contains (dialogueId))
			_fromIds.Add (dialogueId);

		int dialogueIndex = _fromIds.IndexOf (dialogueId);

		return new Rect (rect.x + _fromRect.x, rect.y + _fromRect.y + (dialogueIndex * (_fromRect.height + 5)), _fromRect.width, _fromRect.height);
	}

	public Rect GetQuestionRect(int dialogueId)
	{
		int dialogueIndex = dialogue.answers.Contains (dialogueId) ? dialogue.answers.IndexOf (dialogueId) : dialogue.answers.Count;	
		return new Rect (rect.x + rect.width + _toRect.x, rect.y + _toRect.y + (dialogueIndex * (_toRect.height + 5)), _toRect.width, _toRect.height);
	}

	public void LocalizeText()
	{
		triedToLocalize = false;
	}
}