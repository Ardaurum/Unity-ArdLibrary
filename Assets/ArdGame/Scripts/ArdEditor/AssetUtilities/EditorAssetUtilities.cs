﻿using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.AssetUtilities
{
    public static class EditorAssetUtilities
    {
        public static void CreateAssetAtSelection<T>(T asset, string name, bool overwrite = true, bool focus = false)
            where T : Object
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = EditorConstants.ASSET_PATH;
            }
            else
            {
                FileAttributes fileAttributes = File.GetAttributes(path);
                if ((fileAttributes & FileAttributes.Directory) == 0)
                {
                    string fileName = Path.GetFileName(path);
                    path = path.Substring(0, path.Length - fileName.Length);
                }
            }

            CreateAsset(asset, Path.Combine(path, name), overwrite, focus);
        }

        public static void CreateAsset<T>(T asset, string fullPath, bool overwrite = true, bool focus = false)
            where T : Object
        {
            string path = Path.GetDirectoryName(fullPath);
            string name = Path.GetFileName(fullPath);

            Assert.That(string.IsNullOrEmpty(path) == false, "Path can't be missing directory or filename");

            name = string.IsNullOrEmpty(name) ? typeof(T).ToString() : name;
            string finalPath = Path.Combine(path, name);
            if (AssetDatabase.LoadMainAssetAtPath(finalPath) != null && overwrite == false)
            {
                Debug.LogWarning($"File {finalPath} already exists!");
                return;
            }

            AssetDatabase.CreateAsset(asset, finalPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (focus)
            {
                EditorUtility.FocusProjectWindow();
                asset = AssetDatabase.LoadAssetAtPath<T>(finalPath);
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
        }

        public static void EnsurePathExists(string path)
        {
            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }

            if (string.IsNullOrEmpty(path) == false && Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CreateFile(string contents, string fullPath, bool overwrite = true)
        {
            string path = Path.GetDirectoryName(fullPath);
            string name = Path.GetFileName(fullPath);

            Assert.That(string.IsNullOrEmpty(path) == false
                        || string.IsNullOrEmpty(name) == false,
                "Path can't be missing directory or filename");
            EnsurePathExists(path);

            if (File.Exists(fullPath) && overwrite == false)
            {
                Debug.LogWarning($"File {fullPath} already exists!");
                return;
            }

            File.WriteAllText(fullPath, contents);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string GetPathFromAssetName(string assetName)
        {
            string[] assetGUIDs = AssetDatabase.FindAssets(assetName);
            for (var i = 0; i < assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                if (assetName == Path.GetFileNameWithoutExtension(path))
                {
                    return path;
                }
            }

            return string.Empty;
        }

        public static IReadOnlyList<T> FindAssetsOfType<T>()
            where T : Object
        {
            string[] paths = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var assets = new List<T>(paths.Length);
            for (var i = 0; i < paths.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(paths[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset == null)
                {
                    Debug.LogError($"Found asset of type {typeof(T)} at {assetPath}, but it returned null!");
                    continue;
                }

                assets.Add(asset);
            }

            return assets;
        }
    }
}