using System;
using System.Collections.Generic;
using System.IO;
using ArdEngine.GameDataTools;
using UnityEditor;
using UnityEngine;
using static ArdEditor.AssetUtilities.EditorConstants;

namespace ArdEditor.AssetUtilities
{
    public sealed partial class GameYamlFileCache : ScriptableObject
    {
        private const string CACHE_PATH = EDITOR_ASSETS_PATH + "YamlFileCache.asset";
        [SerializeField] private CacheEntry[] _entries;
        public static IReadOnlyList<CacheEntry> Entries => GetOrCreateCache()._entries;

        private void ScanProject()
        {
            string[] files =
                Directory.GetFiles(Path.Combine(Application.dataPath, "*" + GameDataHelper.GAME_DATA_EXTENSION));
            _entries = new CacheEntry[files.Length];
            for (var i = 0; i < files.Length; i++)
            {
                _entries[i] = new CacheEntry(files[i]);
            }
        }

        public static GameYamlFileCache GetOrCreateCache()
        {
            var cache = AssetDatabase.LoadAssetAtPath<GameYamlFileCache>(CACHE_PATH);
            if (cache == null)
            {
                cache = CreateInstance<GameYamlFileCache>();
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