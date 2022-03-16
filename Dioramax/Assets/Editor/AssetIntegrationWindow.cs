using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetIntegrationWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Dioravity/Asset Integrator")]
    public static void Init()
    {
        AssetIntegrationWindow window = (AssetIntegrationWindow)GetWindow(typeof(AssetIntegrationWindow));
        window.Show();
    }

    private string newPath; 
    void OnGUI()
    {
        GUILayout.Label("Asset Integrator", EditorStyles.boldLabel);

        if (GUILayout.Button("Set Nomenclature and Path"))
        {
            newPath = AssetImporter.SetNewPath();
        }

        EditorGUILayout.TextField("New Path : ", newPath);
    }
}
