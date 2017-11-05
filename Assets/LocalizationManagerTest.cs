using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManagerTest : MonoBehaviour {

    public InputField field;
    public Text label;

    public void Localize()
    {
         label.text = LocalizationManager.Localize(field.text);
    }
}
