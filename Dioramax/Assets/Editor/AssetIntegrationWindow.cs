using UnityEngine;
using UnityEditor;

public class AssetIntegrationWindow : EditorWindow
{
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
            newPath = CustomAssetImporter.SetNewPath();
        }

        EditorGUILayout.TextField("New Path : ", newPath);
    }
}
