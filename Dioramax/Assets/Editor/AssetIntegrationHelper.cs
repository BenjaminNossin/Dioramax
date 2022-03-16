using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;


public class CustomAssetImporter : MonoBehaviour
{
    private static List<string> oldAssetPaths = new List<string>();
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
                for (int j = 0; j < extensions.Length; j++)
                {
                    if (importedAssets[i].Contains(extensions[j]))
                    {
                        Debug.Log("this asset must be repathed : " + importedAssets[i]);
                        oldAssetPaths.Add(importedAssets[i]); 
                    }
                }
            }

            StoreImportedAssetInfos(oldAssetPaths);
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


    private static string oldPath, newPath;
    private static string[] oldPathlist; 
    private static string[] importedAssetName;
    public static void StoreImportedAssetInfos(List<string> _oldPaths)
    {
        oldPathlist = new string[_oldPaths.Count];
        importedAssetName = new string[_oldPaths.Count]; 

        for (int i = 0; i < oldPathlist.Length; i++)
        {
            oldPathlist[i] = _oldPaths[i];
            string assetName = oldPathlist[i];
            importedAssetName[i] = assetName[(assetName.LastIndexOf('/') + 1)..];
        }

        // FINISHI TO TURN THIS INTO ARRAYS
        // MapExtensionToNomenclaturePath(importedAssetName);
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
