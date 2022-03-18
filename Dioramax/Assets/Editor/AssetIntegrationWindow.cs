using UnityEngine;
using UnityEditor;
using System;

public class AssetIntegrationWindow : EditorWindow
{
    public static bool IGNORE_CALLBACK { get; private set; }

    [MenuItem("Dioravity/Asset Integrator")]
    public static void Init()
    {
        IGNORE_CALLBACK = false; 
        AssetIntegrationWindow window = (AssetIntegrationWindow)GetWindow(typeof(AssetIntegrationWindow), false, "Asset Integrator", true);
        window.Show();
    }

    private static (UnityEngine.Object obj, Type type, string path)[] infos;
    void OnGUI()
    {
        GUILayout.Label("Asset Integrator", EditorStyles.boldLabel);

        if (GUILayout.Button("Set Nomenclature and Path"))
        {
            if (CustomAssetImporter.infos == null)
            {
                Debug.LogWarning("No imported asset have been stored. Please import new assets, or reimport them if they are already in the project"); 
                return; // BAD ERROR HANDLING : button should be greyed out in this case
            }

            IGNORE_CALLBACK = true;
            CustomAssetImporter.SetNewPath();

            infos = new (UnityEngine.Object obj, Type type, string path)[CustomAssetImporter.infos.Length];
            CustomAssetImporter.infos.CopyTo(infos, 0);
        }

        if (GUILayout.Button("Refresh"))
        {
            infos = null;
            EditorGUILayout.TextField("New Path : NONE");
        }

        /* if (GUILayout.Button("Get References"))
        {
            CustomAssetImporter.GetNewReferences();
        } */

        // this should be greyed out while button has not been pushed
        if (infos == null) return;

        if (infos.Length != 0)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                EditorGUILayout.ObjectField(infos[i].obj, infos[i].type, false);
                EditorGUILayout.TextField("New Path : ", infos[i].path);
            }
        }
    }
}
