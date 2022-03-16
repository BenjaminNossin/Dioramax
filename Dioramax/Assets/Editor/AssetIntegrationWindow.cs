using UnityEngine;
using UnityEditor;

public class AssetIntegrationWindow : EditorWindow
{
    [MenuItem("Dioravity/Asset Integrator")]
    public static void Init()
    {
        AssetIntegrationWindow window = (AssetIntegrationWindow)GetWindow(typeof(AssetIntegrationWindow), false, "Asset Integrator", true);
        window.Show();
    }


    private static (Object obj, System.Type type, string path) infos;
    void OnGUI()
    {
        GUILayout.Label("Asset Integrator", EditorStyles.boldLabel);

        if (GUILayout.Button("Set Nomenclature and Path"))
        {
            infos = CustomAssetImporter.SetNewPath();
        }

        // this should be greyed out while button has not been pushed
        EditorGUILayout.ObjectField(infos.obj, infos.type, false);
        EditorGUILayout.TextField("New Path : ", infos.path);
    }
}
