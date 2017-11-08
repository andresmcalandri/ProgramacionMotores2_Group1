using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Remove asset menu. Testing only
[CreateAssetMenu(fileName = "Dialogue", menuName = "DialogueEditor/New Dialogue Item", order = 2)]
public class DialogueItem : ScriptableObject {

	[HideInInspector]
	public int id;

	//Position Rect in the Dialogue editor graph
	[HideInInspector]
	public Rect rect = Rect.zero;

	[HideInInspector]
	public Rect orgRect = Rect.zero;

	public string name = "";
	public string locKey = "";
    public DialogueItem nextDialogue;

	/// <summary>
	/// The answers. If the answers are null, count is 0 or an answer is null then its the end of the line.
	/// </summary>
	public List<int> answers = new List<int>(); 

	/// <summary>
	/// Adds a new answer.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void AddAnswer(int id)
	{
		if (this.answers == null)
			answers = new List<int> ();

		answers.Add (id);
	}

	/// <summary>
	/// Removes an answer.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void RemoveAnswer(int id)
	{
		if (answers != null && answers.IndexOf (id) >= 0)
			answers.Remove (id);
	}
}
