using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;


public class AssetImporter : MonoBehaviour
{
    public static void PlaceAssetAtPath(string oldPath)
    {
        /* string[] fileNames = new[] { "" };

        for (int i = 0; i < fileNames.Length; ++i)
        {
            var unimportedFileName = fileNames[i];
            var assetsPath = Application.dataPath + "/_Models/" + unimportedFileName;
            File.WriteAllText(assetsPath, "Testing 123");
        }

        var relativePath = $"Assets/_Models/";
        AssetDatabase.ImportAsset(relativePath); */

        string importedAssetNmame = oldPath[(oldPath.LastIndexOf('/') + 1)..]; 
        string newPath = $"Assets/_Models/{importedAssetNmame}"; 
        AssetDatabase.MoveAsset(oldPath, newPath);
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
                AssetImporter.PlaceAssetAtPath(importedAssets[i]);
            }
        }

        // Debug.Log("bundle name : " + modelImporter.assetBundleName);
        // Test();
    }

    private static void Test()
    {
        AssetIntegrationWindow.Init();
    }
}
