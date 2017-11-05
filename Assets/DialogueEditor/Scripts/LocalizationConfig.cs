using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalizationConfig : ScriptableObject {

	public string defaultLanguage = "";
	public LocalizableLanguage[] languages;
}

[Serializable]
public struct LocalizableLanguage
{
	public string id;
	public TextAsset file;
}