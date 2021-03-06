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
        if (GUILayout.Button("See Documentation"))
        {
            Application.OpenURL("https://drive.google.com/file/d/1Su1Xa467HHNnmvQXf20gajIQ_DHwPbgV/view?usp=sharing"); 
        }

        if (GUILayout.Button("Set Nomenclature and Path"))
        {
            if (CustomAssetImporter.infos == null)
            {
                Debug.LogWarning("No imported asset have been stored. \n" +
                    "Please import new assets, or right click -> reimport if they are already in the project"); 
                return; // BAD ERROR HANDLING : button should be greyed out in this case
            }

            IGNORE_CALLBACK = true;
            CustomAssetImporter.SetNewPath();

            infos = new (UnityEngine.Object obj, Type type, string path)[CustomAssetImporter.infos.Length];
            CustomAssetImporter.infos.CopyTo(infos, 0);
        }

        if (GUILayout.Button("End Import"))
        {
            infos = CustomAssetImporter.infos = null;
            IGNORE_CALLBACK = false;
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
