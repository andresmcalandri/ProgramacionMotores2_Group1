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

		Zoom ();

        BeginWindows();
        for (int i = 0; i < controls.Count; i++)
        {
			controls[i].rect = GUI.Window(i, controls[i].rect, controls[i].Draw, "Window_" + i);
        }
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

		if (dialogue != null && controls.Count > 0)
			menu.AddItem(new GUIContent("Localize"), false, LocalizeAll);
		else
			menu.AddDisabledItem(new GUIContent("Localize"));

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
		newWindow.orgRect = new Rect(
			newWindow.rect.x, 
			newWindow.rect.y,
			newWindow.rect.width, 
			newWindow.rect.height
		);
		//
		//Adapt to zoomed size if adding a window after zooming
		newWindow.rect = new Rect (
			newWindow.rect.x,
			newWindow.rect.y,
			newWindow.rect.width * _zoomLevel,
			newWindow.rect.height * _zoomLevel
		);
		//
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

	void LocalizeAll()
	{
		foreach (DialogueItemWindow control in controls) {
			control.LocalizeText ();
		}
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
	DialogueItemWindow _focusedWindow;
	const float MIN_ZOOM = 0.6f;
	const float MAX_ZOOM = 3f;
	const float ZOOM_SENSI = 0.1f;
	const float DISPL_RATE = 0.1f;

	void Zoom(){
		_focusedWindow = null;
		Event e = Event.current;
		if (e.type == EventType.MouseUp) {
			foreach (var window in controls) {
				window.orgRect = new Rect (
					window.rect.x / _zoomLevel,
					window.rect.y / _zoomLevel,
					window.orgRect.width,
					window.orgRect.height
				);
			}
		}
		if (e.type == EventType.MouseDrag) {
			foreach (var window in controls) {
				if (e.mousePosition.x > window.rect.x &&
				    e.mousePosition.x < window.rect.x + window.rect.width &&
				    e.mousePosition.y > window.rect.y &&
				    e.mousePosition.y < window.rect.y + window.rect.height) {
					_focusedWindow = window;
					break;
				}
			}
			if (_focusedWindow == null){
				foreach (var window in controls) {
					window.orgRect = new Rect (
						window.orgRect.x + e.delta.x,
						window.orgRect.y + e.delta.y,
						window.orgRect.width,
						window.orgRect.height
					);

					window.rect = new Rect (
						window.orgRect.x * _zoomLevel,
						window.orgRect.y * _zoomLevel,
						window.rect.width,
						window.rect.height
					);
				}
				Repaint ();
			}
		}
		if (e.type == EventType.scrollWheel) {
			if (-e.delta.y > 0f) {
				_zoomLevel += ZOOM_SENSI;
				_zoomLevel = Mathf.Clamp (_zoomLevel, MIN_ZOOM, MAX_ZOOM);
			} else if (-e.delta.y < 0f) {
				_zoomLevel -= ZOOM_SENSI;
				_zoomLevel = Mathf.Clamp (_zoomLevel, MIN_ZOOM, MAX_ZOOM);
			}
			foreach (var window in controls) {
				window.rect = new Rect (
					window.orgRect.x * _zoomLevel,
					window.orgRect.y * _zoomLevel,
					window.orgRect.width * _zoomLevel,
					window.orgRect.height * _zoomLevel
				);
			}
			Repaint ();
		}
	}
	//END ZOOM CONTROL
}
