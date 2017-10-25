using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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

    //Grid Vars
    int gridSpace = 0;
    int gridZoom = 0;
    Vector2 offset = new Vector2();


    void OnGUI()
    {
        if (GUILayout.Button("Create new Dialogue"))
            CreateDialogue();

        OnGUIDialogue();

        BeginWindows();

        for (int i = 0; i < controls.Count; i++)
        {
			controls[i].rect = GUI.Window(i, controls[i].rect, controls[i].Draw, "Window_" + i);
        }

		Zoom ();
        EndWindows();

        BeginGrid();

    }

    void CreateDialogue()
    {
        dialogue = ScriptableObject.CreateInstance<Dialogue>();
    }

    void OnGUIDialogue()
    {
        if (dialogue == null)
            GUI.enabled = false;
        else
        {
            dialogue.name = EditorGUILayout.TextField("Name", dialogue.name);

            if (GUILayout.Button("Create Window"))
                CreateControl();

            if (GUILayout.Button("Save"))
                SaveDialogue();
        }


        GUI.enabled = true;
    }

    void CreateControl()
    {
		DialogueItem dialogueItem = ScriptableObject.CreateInstance<DialogueItem> ();
		dialogueItem.id = dialogueCount++;
			
		DialogueItemWindow newWindow = new DialogueItemWindow (dialogueItem, this);
		controls.Add(newWindow);

		//Default Dialogue Window Rect Size Reference for Zooming
		_defaultRectSize = new Vector2 (newWindow.rect.width, newWindow.rect.height);
		//
    }

	public void DeleteControl(DialogueItemWindow window)
    {
		controls.Remove(window);
    }


	//TODO Load a dialogue asset
	void LoadDialogue()
	{

	}

	//TODO Finish it like it should be
	void SaveDialogue()
	{
        if (dialogue == null)
        {
            EditorUtility.DisplayDialog("Error","Please create a Dialogue First and set a name", "Ok");
            return;
        }

        DialogueItem dialogueItem;
        foreach (DialogueItemWindow itemWindow in controls)
        {
            dialogueItem = itemWindow.dialogue;
            if (dialogueItem != null)
            {
                string path = "Assets/Resources/Dialogues/" + dialogue.name;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                AssetDatabase.CreateAsset(dialogueItem, path +  "/" + dialogueItem.id.ToString() + ".asset");
            }
        }

        AssetDatabase.SaveAssets();
	}

    void BeginGrid()
    {
        //DrawGrid
        gridSpace = 25 + gridZoom;
        var lastRect = GUILayoutUtility.GetLastRect();
        var rect = new Rect(0, lastRect.yMax, position.width, position.height - lastRect.yMax);
        GUI.BeginGroup(rect);
        //TODO: Descomentar cuando se pueda poner por detras de nuevos objetos y se ajuste correctamente al espacio
        //DrawGrid(rect);
        GUI.EndGroup();
    }

    void DrawGrid(Rect rect)
    {
        Handles.BeginGUI();
        for (int x = (int)rect.xMin; x < rect.width; x += gridSpace)
        {
            Handles.DrawLine(new Vector2(x, rect.yMin) + offset, new Vector2(x, rect.yMax) + offset);
        }
        for (int y = (int)rect.yMin; y < rect.height; y += gridSpace)
        {
            Handles.DrawLine(new Vector2(rect.xMin, y) + offset, new Vector2(rect.xMax, y) + offset);
        }
        Handles.EndGUI();
    }


    //START ZOOM CONTROL

    float _zoomSensitivity = 3f;
	float _xScale = 0f;
	float _yScale = 0f;
	float _xDisplacement = 0f;
	float _yDisplacement = 0f;
	float _minXScale = 100f;
	float _minYScale = 75f;
	float _maxXScale = 400f;
	float _maxYScale = 300f;
	Vector2 _defaultRectSize = Vector2.zero;
	Vector2 _winPos = Vector2.zero;

	void Zoom(){
		Event e = Event.current;
		if (e.type == EventType.scrollWheel) {

			if (-e.delta.y > 0f) {
				_xScale = _zoomSensitivity;
				_yScale = _zoomSensitivity * (_defaultRectSize.y / _defaultRectSize.x);
                gridZoom++;
			} else if (-e.delta.y < 0f) {
				_xScale = -_zoomSensitivity;
				_yScale = -_zoomSensitivity * (_defaultRectSize.y / _defaultRectSize.x);
                gridZoom--;
            }

			foreach (var window in controls) {
				if (window.rect.width < _maxXScale && window.rect.height < _maxYScale &&
				    window.rect.width > _minXScale && window.rect.height > _minYScale) {

					_winPos = new Vector2 (window.rect.x + (window.rect.width / 2f), window.rect.y + (window.rect.height / 2f));

					if (e.mousePosition.x < _winPos.x) {
						_xDisplacement = _xScale;
					} else if (e.mousePosition.x > _winPos.x) {
						_xDisplacement = -_xScale;
					}

					if (e.mousePosition.y < _winPos.y) {
						_yDisplacement = _yScale;
					} else if (e.mousePosition.y > _winPos.y) {
						_yDisplacement = -_yScale;
					}

				} else {
					_xDisplacement = 0f;
					_yDisplacement = 0f;
				}
				window.rect = new Rect (
					window.rect.x + _xDisplacement * Vector2.Distance(e.mousePosition, window.rect.position) / window.rect.width,
					window.rect.y + _yDisplacement * Vector2.Distance(e.mousePosition, window.rect.position) / window.rect.height,
					Mathf.Clamp (window.rect.width + _xScale, _minXScale, _maxXScale),
					Mathf.Clamp (window.rect.height + _yScale, _minYScale, _maxYScale)
				);
			}
		}
		Repaint ();
	}
	//END ZOOM CONTROL
}
