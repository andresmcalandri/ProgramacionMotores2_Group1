using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class LocalizationManager {

	public const string SAVE_PATH = "Assets/Resources/Localization/";

    private static LocalizableLanguage _currentLanguage;

    public static string CurrentLanguage
    {
        get
        {
			return _currentLanguage.id;
        }
        set
        {
            if (IsLanguagesEmpty)
            {
                throw new Exception("There is no localizable languages");
            }
            else
            {
                for (int i = 0; i < Config.languages.Length; i++)
                {
                    if (Config.languages[i].id == value)
						_currentLanguage = Config.languages[i];
                }

                if (_currentLanguage.id != value)
                {
                    throw new Exception("The language " + value + " does not exist" );
                }
            }
        }
    }

	private static void SetDefaultLanguage()
    {
        if (!string.IsNullOrEmpty(Config.defaultLanguage))
            _currentLanguage = GetLanguage(Config.defaultLanguage);
        else
			_currentLanguage = Config.languages.Length <= 0 ? new LocalizableLanguage() : Config.languages[0];
    }

    public static string Localize(string key)
    {
		if (_currentLanguage.id == null)
			SetDefaultLanguage ();
		
		if (!string.IsNullOrEmpty(_currentLanguage.id)) {
            string assetPath = AssetDatabase.GetAssetPath(_currentLanguage.file);
            StreamReader reader = new StreamReader(assetPath);
            string line, fileKey;
            string[] splittedLine;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                splittedLine = line.Split('=');
                if(splittedLine.Length > 1)
                {
                    fileKey = splittedLine[0].Trim();
                    if (key == fileKey)
                    {
                        reader.Close();
                        return line.Remove(0, splittedLine[0].Length + 1).TrimStart();
                    }
                }
            }
        }

        return key;
    }

    public static LocalizableLanguage GetLanguage(string id)
    {
        LocalizableLanguage language = new LocalizableLanguage();
        if (!IsLanguagesEmpty)
        {
			for (int i = 0; i < Config.languages.Length; i++)
            {
				if (Config.languages[i].id == id)
					return Config.languages[i];
            }
        }

        return language;
    }

    private static bool IsLanguagesEmpty
    {
        get
        {
			return Config.languages == null || Config.languages.Length <= 0;
        }
    }

	private static LocalizationConfig _config;
	public static LocalizationConfig Config
	{
		get{
			if (_config == null) {
				if (!File.Exists (SAVE_PATH + "LocalizationConfig.asset")) {
					_config = ScriptableObject.CreateInstance<LocalizationConfig> ();
					Directory.CreateDirectory (SAVE_PATH);
					AssetDatabase.CreateAsset(_config, SAVE_PATH + "LocalizationConfig.asset");
					EditorUtility.DisplayDialog ("Warning", "A new localization config file was created. Please fill in " +
					"the display Languages. The file can be found under: " + SAVE_PATH, "Ok");
					AssetDatabase.SaveAssets ();
				}
					
				_config = AssetDatabase.LoadAssetAtPath<LocalizationConfig> (SAVE_PATH + "LocalizationConfig.asset");
			}

			return _config;
		}
	}
}
