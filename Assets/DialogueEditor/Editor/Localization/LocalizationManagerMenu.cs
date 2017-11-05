using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LocalizationManagerMenu : EditorWindow {

	public const string path = "Assets/Resources/Localization/";
	private LocalizationConfig _config;

	[MenuItem("DialogueEditor/Localization/Manage")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(LocalizationManagerMenu));
	}

	void OnGUI()
	{
		if (_config == null)
			_config = LocalizationManager.Config;

		Editor editor = Editor.CreateEditor( _config );
		editor.DrawDefaultInspector();
	}
}
