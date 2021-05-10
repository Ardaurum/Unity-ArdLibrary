using System;
using System.Collections.Generic;
using System.IO;
using ArdEngine.ArdAttributes;
using ArdEngine.GameDataTools;
using UnityEditor;
using UnityEngine;
using static ArdEditor.AssetUtilities.EditorConstants;

namespace ArdEditor.AssetUtilities
{
    public sealed partial class GymlFileCache : ScriptableObject
    {
        private const string CACHE_PATH = EDITOR_ASSETS_PATH + "GymlFileCache.asset";
        [SerializeField] [OneLineProperty] private CacheEntry[] _entries;
        public static IReadOnlyList<CacheEntry> Entries => GetOrCreateCache()._entries;

        private void ScanProject()
        {
            string[] files = Directory.GetFiles(Application.dataPath,
                    "*" + GameDataHelper.GAME_DATA_EXTENSION,
                    SearchOption.AllDirectories);
            
            _entries = new CacheEntry[files.Length];
            for (var i = 0; i < files.Length; i++)
            {
                string relativeFilePath = files[i].ConvertToRelativeProjectPath();
                _entries[i] = new CacheEntry(relativeFilePath);
            }
        }

        public static GymlFileCache GetOrCreateCache()
        {
            var cache = AssetDatabase.LoadAssetAtPath<GymlFileCache>(CACHE_PATH);
            if (cache == null)
            {
                cache = CreateInstance<GymlFileCache>();
                EditorAssetUtilities.CreateAsset(cache, CACHE_PATH);
                cache.ScanProject();
                AssetDatabase.SaveAssets();
            }

            return cache;
        }

        [Serializable]
        public struct CacheEntry
        {
            public string Path;

            public CacheEntry(string path)
            {
                Path = path;
            }
        }
    }
}