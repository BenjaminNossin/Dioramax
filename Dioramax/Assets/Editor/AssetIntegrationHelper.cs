using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;


public class AssetImporter : MonoBehaviour
{
    [MenuItem("Assets/Dioravity/Set Asset Path and Nomenclature")]
    private static void LoadAdditiveScene()
    {
        AssetIntegrationWindow.Init();
    }

    public static string oldPath, newPath, importedAssetName; 
    public static void StoreImportedAssetInfos(string _oldPath)
    {
        oldPath = _oldPath; 

        importedAssetName = oldPath[(oldPath.LastIndexOf('/') + 1)..]; 
        newPath = $"Assets/_Models/{importedAssetName}";

        Debug.Log("setting old and new path"); 
    }

    public static string SetNewPath()
    {
        AssetDatabase.MoveAsset(oldPath, newPath);
        AssetDatabase.RenameAsset(newPath, "_Model" + importedAssetName);
        return newPath;
    }
}

public class AssetIntegrationHelper : AssetPostprocessor
{
    private ModelImporter modelImporter;
    private AssetImportContext importContext;
    // Disable import of materials if the file contains
    // the @ sign marking it as an animation.
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < importedAssets.Length; i++)
        {
            Debug.Log("imported asset is : " + importedAssets[i]);

            if (importedAssets[i].Contains(".obj"))
            {
                Debug.Log("preprocessing model : " + importedAssets[i]);
                AssetImporter.StoreImportedAssetInfos(importedAssets[i]);
            }
        }
    }
}
