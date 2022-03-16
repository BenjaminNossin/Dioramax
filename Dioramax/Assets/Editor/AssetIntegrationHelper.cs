using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public class CustomAssetImporter : MonoBehaviour
{
    
    /// <summary>
    /// This Class filters out assets that should not be repathed (unity scenes, scripts, etc..) and then sends all allowed assets
    /// to the AssetImporter
    /// </summary>
    private class AssetIntegrationHelper : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                Debug.Log("imported asset is : " + importedAssets[i]);
                for (int j = 0; j < extensions.Length; j++)
                {
                    if (importedAssets[i].Contains(extensions[j]))
                    {
                        Debug.Log("this asset must be repathed : " + importedAssets[i]);
                        StoreImportedAssetInfos(importedAssets[i]);
                    }
                }
            }
        }
    }

    [MenuItem("Assets/Dioravity/Set Asset Path and Nomenclature")]
    private static void SetAssetPathAndNomenclature()
    {
        AssetIntegrationWindow.Init();
    }

    private static string[] nomenclatures = new string[] { "Model_", "T_", "M_", "LightSettings_", "Anim_", "Font_", "Sound_" };
    private static string[] newPaths = new string[] { "_Models", "Materials & Textures", "Graphics/Lighting", 
                                                      "Animations", "User Interface/Fonts", "Sounds"};
    internal static string[] extensions = new string[] { ".obj", ".fbx", ".png", ".jpeg", ".jpg", ".mat", 
                                                         ".lighting", ".anim", ".ttf", ".mp3", ".ogg", ".wav" };
    private static int[] extensionMapper = new int[] { 0, 0, 1, 1, 1, 2, 3, 4, 5, 6, 6, 6 }; // maps every extension to the proper nomenclature index 
    private static int[] pathMapper = new int[] { 0, 1, 1, 2, 3, 4, 5 }; // maps every extension to the proper path


    public static string oldPath, newPath, importedAssetName;
    public static void StoreImportedAssetInfos(string _oldPath)
    {
        oldPath = _oldPath;

        importedAssetName = oldPath[(oldPath.LastIndexOf('/') + 1)..];
        Debug.Log("setting old and new path");

        MapExtensionToNomenclaturePath(importedAssetName);
    }

    static FileInfo fileInfo;
    static string _extensions;
    static int nomenclatureIndex, pathIndex; 
    static string properNomenclature; 
    private static void MapExtensionToNomenclaturePath(string fileName)
    {
        fileInfo = new(fileName);
        _extensions = fileInfo.Extension;
        nomenclatureIndex = Array.IndexOf(extensions, _extensions);
        properNomenclature = nomenclatures[extensionMapper[nomenclatureIndex]];

        pathIndex = Array.IndexOf(nomenclatures, properNomenclature);
        newPath = $"Assets/{newPaths[pathMapper[pathIndex]]}" + "/" + importedAssetName;

        Debug.Log($"index of {_extensions} is {nomenclatureIndex}. Proper nomenclature : {properNomenclature}. New path : {newPath}");
    }

    private static (UnityEngine.Object obj, Type type, string path) infos = new();  
    /// <summary>
    /// This method awaits for a button input from AssetIntegrationWindow to be triggered
    /// </summary>
    /// <returns></returns>
    public static (UnityEngine.Object obj, Type type, string path) SetNewPath()
    {
        AssetDatabase.MoveAsset(oldPath, newPath);
        AssetDatabase.RenameAsset(newPath, properNomenclature + importedAssetName);

        infos.type = AssetDatabase.GetMainAssetTypeAtPath(newPath);
        infos.obj = AssetDatabase.LoadMainAssetAtPath(newPath);
        infos.path = newPath; 
        return infos;
    }
}
