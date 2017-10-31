using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class DialogueEditor : EditorWindow {
    
    [MenuItem("DialogueEditor/Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor));
    }

	//TODO We should show this in some kind of options
	public const string SAVE_PATH = "Assets/Resources/Dialogues/";


	//TODO Create a button to create a new dialogue
	Dialogue dialogue = null;
	List<DialogueItemWindow> controls = new List<DialogueItemWindow>();
	List<int> itemsToDelete = new List<int>();

	//TODO Create an id management that actually works.
	int dialogueCount = 0;

    //Grid Vars
    int gridSpace = 0;
    int gridZoom = 0;
    Vector2 offset = new Vector2();

    void OnGUI()
    {
        DrawContextualMenus();
        OnGUIDialogue();

        BeginWindows();
        for (int i = 0; i < controls.Count; i++)
        {
			controls[i].rect = GUI.Window(i, controls[i].rect, controls[i].Draw, "Window_" + i);
        }

		//ZoomOld ();
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
        if (dialogue != null)
        {
            dialogue.name = EditorGUILayout.TextField("Name", dialogue.name);

            if (GUILayout.Button("Create Window"))
                CreateControl();
        }
    }

    void DrawContextualMenus()
    {
        if (GUILayout.Button(" File", GUILayout.MaxWidth(60)))
        {
            ToggleFileMenu();
        }
    }

    void ToggleFileMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("New"), false, CreateDialogue);

        if (dialogue != null)
            menu.AddItem(new GUIContent("Save"), false, SaveDialogue);
        else
            menu.AddDisabledItem(new GUIContent("Save"));

        menu.AddItem(new GUIContent("Load"), false, LoadDialogue);
        menu.AddSeparator("");
        menu.ShowAsContext();
    }


    void CreateControl(DialogueItem item = null)
    {
		DialogueItem dialogueItem = item;
		if (item == null) 
		{
			dialogueItem = ScriptableObject.CreateInstance<DialogueItem> ();
			dialogueItem.id = dialogueCount++;
		}
			
		DialogueItemWindow newWindow = new DialogueItemWindow (dialogueItem, this);
		//Default Dialogue Window Rect Size Reference for Zooming
		_defaultRectSize = new Vector2 (newWindow.rect.width, newWindow.rect.height);
		_defaultRectPos = new Vector2 (newWindow.rect.x, newWindow.rect.y);
		//
		newWindow.rect = new Rect (
			newWindow.rect.x,
			newWindow.rect.y,
			_defaultRectSize.x * _zoomLevel,
			_defaultRectSize.y * _zoomLevel
		);
		controls.Add(newWindow);
    }

	public void DeleteControl(DialogueItemWindow window)
    {
		itemsToDelete.Add (window.dialogue.id);
		controls.Remove(window);
    }


	//TODO Move the load to a menu
	string dialogueToLoad = "";
	void LoadDialogue()
	{
        dialogueToLoad = EditorUtility.OpenFolderPanel("Load", SAVE_PATH, "");
        if (Directory.Exists(dialogueToLoad) && dialogueToLoad != SAVE_PATH)
        {
            controls.Clear();
            itemsToDelete.Clear();

            string dialogueName = dialogueToLoad.Split('/').Last();
            CreateDialogue();
            dialogue.name = dialogueName;
            dialogueCount = 0;
            string[] files = Directory.GetFiles(dialogueToLoad);

            DialogueItem dialogueItem;
            string fileName;
            foreach (string file in files)
            {
                if (file.Contains(".meta"))
                    continue;

                fileName = file.Split('/').Last();
                dialogueItem = AssetDatabase.LoadAssetAtPath<DialogueItem>(SAVE_PATH + fileName);
                if (dialogueItem != null)
                { 
                    CreateControl(dialogueItem);

                    if (dialogueItem.id > dialogueCount)
                        dialogueCount = dialogueItem.id;
                }
            }
        }
	}

	void SaveDialogue()
	{
        if (dialogue == null)
        {
            EditorUtility.DisplayDialog("Error","Please create a Dialogue First and set a name", "Ok");
            return;
        }

		string path = SAVE_PATH + dialogue.name;
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		DialogueItem dialogueItem;
		string fullPath;
		
        foreach (DialogueItemWindow itemWindow in controls)
        {
            dialogueItem = itemWindow.dialogue;
            if (dialogueItem != null)
            {
				fullPath = path + "/" + dialogueItem.id.ToString () + ".asset";
				if (!File.Exists (fullPath))
					AssetDatabase.CreateAsset (dialogueItem, fullPath);
            }
        }

		foreach (int id in itemsToDelete) {
			fullPath = path + "/" + id.ToString ();
			if (File.Exists (fullPath + ".asset"))
				File.Delete (fullPath + ".asset");

			if (File.Exists (fullPath + ".meta"))
				File.Delete (fullPath + ".meta");
		}

		itemsToDelete.Clear ();

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

	float _zoomLevel = 1;
	const float _minZoom = 0.6f;
	const float _maxZoom = 3f;
	const float _zoomSensi = 0.1f;
	Vector2 _defaultRectSize = Vector2.zero;
	Vector2 _defaultRectPos = Vector2.zero;

	void Zoom(){
		Debug.Log ("Zoom Level: " + _zoomLevel);
		Event e = Event.current;
		if (e.type == EventType.scrollWheel) {
			if (-e.delta.y > 0f) {
				_zoomLevel += _zoomSensi;
				_zoomLevel = Mathf.Clamp (_zoomLevel, _minZoom, _maxZoom);
			} else if (-e.delta.y < 0f) {
				_zoomLevel -= _zoomSensi;
				_zoomLevel = Mathf.Clamp (_zoomLevel, _minZoom, _maxZoom);
			}
			foreach (var window in controls) {
				window.rect = new Rect (
					//window.rect.x - (((_defaultRectSize.x * _zoomLevel) - window.rect.width) / 2),
					//window.rect.y - (((_defaultRectSize.y * _zoomLevel) - window.rect.height) / 2),
					window.rect.x + ((window.rect.width - (_defaultRectSize.x * _zoomLevel)) / 2),
					window.rect.y + ((window.rect.height - (_defaultRectSize.y * _zoomLevel)) / 2),
					_defaultRectSize.x * _zoomLevel,
					_defaultRectSize.y * _zoomLevel
				);
			}
			Repaint ();
		}
	}

	float _zoomSensitivity = 3f;
	float _xScale = 0f;
	float _yScale = 0f;
	float _xDisplacement = 0f;
	float _yDisplacement = 0f;
	float _minXScale = 100f;
	float _minYScale = 75f;
	float _maxXScale = 400f;
	float _maxYScale = 300f;

	Vector2 _winPos = Vector2.zero;

	void ZoomOld(){
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
