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
            IGNORE_CALLBACK = true;
            CustomAssetImporter.SetNewPath();

            infos = new (UnityEngine.Object obj, Type type, string path)[CustomAssetImporter.infos.Length];
            CustomAssetImporter.infos.CopyTo(infos, 0);
        }

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
