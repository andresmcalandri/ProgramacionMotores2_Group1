using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueCanvasCreator : EditorWindow
{
    [MenuItem("DialogueEditor/Create Dialogue Cavnas")]
    static void Create()
    {
        GameObject dialogueCavnas = (GameObject)Resources.Load("Prefabs/DialogueCanvas");
        dialogueCavnas.name = "Dialogue Cavnas";
        Instantiate(dialogueCavnas);

    }
}
