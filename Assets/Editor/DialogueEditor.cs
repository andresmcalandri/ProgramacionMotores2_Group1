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

    List<Rect> controls = new List<Rect>();
    void OnGUI()
    {
        if (GUILayout.Button("Create Window"))
            CreateControl();

        BeginWindows();

        for (int i = 0; i < controls.Count; i++)
        {
            controls[i] = GUI.Window(i, controls[i], DoMyWindow, "Window_" + i);
        }

        EndWindows();
    }

    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(190, 0, 10, 10), "X"))
        {
            DeleteControl(windowID);
        }

        if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
            Debug.Log("Hello World");

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    void CreateControl()
    {
        controls.Add(new Rect(0, 0, 200, 200));
    }

    void DeleteControl(int windowId)
    {
        controls.RemoveAt(windowId);
    }
}
