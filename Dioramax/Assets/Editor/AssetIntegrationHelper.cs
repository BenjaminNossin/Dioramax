using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;


public class CustomAssetImporter : MonoBehaviour
{
    /// <summary>
    /// This Class filters out assets that should not be repathed (unity scenes, scripts, etc..) and then sends all allowed assets
    /// to the AssetImporter
    /// </summary>
    private class AssetIntegrationHelper : AssetPostprocessor
    {
        private static readonly List<string> oldAssetPaths = new List<string>();

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (AssetIntegrationWindow.IGNORE_CALLBACK) return; 

            if (oldAssetPaths.Count != 0)
            {
                oldAssetPaths.Clear();
            }

            for (int i = 0; i < importedAssets.Length; i++)
            {
                for (int j = 0; j < storedExtensions.Length; j++)
                {
                    if (importedAssets[i].Contains(storedExtensions[j]))
                    {
                        // Debug.Log("this asset must be repathed : " + importedAssets[i]);
                        oldAssetPaths.Add(importedAssets[i]);
                    }
                }
            }

            if (oldAssetPaths.Count != 0)
            {
                StoreImportedAssetInfos(oldAssetPaths);
            }
        }
    }

    [MenuItem("Assets/Dioravity/Set Asset Path and Nomenclature")]
    private static void SetAssetPathAndNomenclature()
    {
        AssetIntegrationWindow.Init();
    }

    private static readonly string[] storedNomenclatures = new string[] { "Model_", "T_", "M_", "LightSettings_", "Anim_", "Font_", "Sound_" };
    private static readonly string[] storedNewPaths = new string[] { "_Models", "Materials & Textures", "Graphics/Lighting",
                                                      "Animations", "User Interface/Fonts", "Sounds"};
    private static readonly string[] storedExtensions = new string[] { ".obj", ".fbx", ".png", ".jpeg", ".jpg", ".mat",
                                                         ".lighting", ".anim", ".ttf", ".mp3", ".ogg", ".wav" };
    private static readonly int[] extensionMapper = new int[] { 0, 0, 1, 1, 1, 2, 3, 4, 5, 6, 6, 6 }; // maps every extension to the proper nomenclature index
    private static readonly int[] pathMapper = new int[] { 0, 1, 1, 2, 3, 4, 5 }; // maps every extension to the proper path


    private static string[] newPaths;
    private static string[] oldPaths;
    private static string[] importedAssetNames;
    private static void StoreImportedAssetInfos(List<string> _oldPaths)
    {
        oldPaths = new string[_oldPaths.Count];
        importedAssetNames = new string[_oldPaths.Count];

        for (int i = 0; i < oldPaths.Length; i++)
        {
            oldPaths[i] = _oldPaths[i];
            importedAssetNames[i] = oldPaths[i][(oldPaths[i].LastIndexOf('/') + 1)..];
        }

        // TODO : should work on assets that have already been imported
        MapExtensionToNomenclaturePath(importedAssetNames);
    }

    static FileInfo[] fileInfos;
    static string[] loadedFilesExtensions;
    static int[] nomenclatureIndexes, pathIndexes;
    static string[] properNomenclatures;
    private static void MapExtensionToNomenclaturePath(string[] fileNames)
    {
        // CHECK IF I CAN MAKE INITIALIZATION CLEANER
        fileInfos = new FileInfo[fileNames.Length];
        loadedFilesExtensions = new string[fileNames.Length];
        nomenclatureIndexes = new int[fileNames.Length];
        pathIndexes = new int[fileNames.Length];
        properNomenclatures = new string[fileNames.Length];
        newPaths = new string[fileNames.Length];

        for (int i = 0; i < fileNames.Length; i++)
        {
            fileInfos[i] = new(fileNames[i]);
            loadedFilesExtensions[i] = fileInfos[i].Extension;
            nomenclatureIndexes[i] = Array.IndexOf(storedExtensions, loadedFilesExtensions[i]);
            properNomenclatures[i] = storedNomenclatures[extensionMapper[nomenclatureIndexes[i]]];

            pathIndexes[i] = Array.IndexOf(storedNomenclatures, properNomenclatures[i]);
            newPaths[i] = $"Assets/{storedNewPaths[pathMapper[pathIndexes[i]]]}" + "/" + importedAssetNames[i];

            Debug.Log($"index of {loadedFilesExtensions[i]} is {nomenclatureIndexes[i]}. \n" +
                $"Proper nomenclature : {properNomenclatures[i]}. New path : {newPaths[i]}");
        }
    }

    public static (UnityEngine.Object obj, Type type, string path)[] infos;
    /// <summary>
    /// This method awaits for a button input from AssetIntegrationWindow to be triggered
    /// </summary>
    /// <returns></returns>
    public static void SetNewPath()
    {
        infos = new (UnityEngine.Object obj, Type type, string path)[newPaths.Length];
        for (int i = 0; i < newPaths.Length; i++)
        {
            AssetDatabase.MoveAsset(oldPaths[i], newPaths[i]);
            AssetDatabase.RenameAsset(newPaths[i], properNomenclatures[i] + importedAssetNames[i]);

            infos[i].type = AssetDatabase.GetMainAssetTypeAtPath(newPaths[i]);
            infos[i].obj = AssetDatabase.LoadMainAssetAtPath(newPaths[i]);
            infos[i].path = newPaths[i];
            Debug.Log("setting new path and nomenclature"); 
        }
    }
}
