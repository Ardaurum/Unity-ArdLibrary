using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ArdEditor.AssetUtilities.EditorConstants;
using Object = UnityEngine.Object;

namespace ArdEditor.AssetUtilities
{
    public sealed class AssetCache : ScriptableObject, ISerializationCallbackReceiver
    {
        private const string CACHE_PATH = EDITOR_ASSETS_PATH + "AssetCache.asset";
        [SerializeField] private List<CacheEntry> _serializedEntries;
        private Dictionary<string, string> _cache;

        public static T GetSingletonAssetWithName<T>(string assetName)
            where T : Object
        {
            return GetOrCreateCache().GetSingletonAssetWithNameInternal<T>(assetName);
        }

        private T GetSingletonAssetWithNameInternal<T>(string assetName)
            where T : Object
        {
            var cacheID = $"{assetName}_{typeof(T).Name}";
            if (_cache.TryGetValue(cacheID, out string guid))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null)
                {
                    return asset;
                }
                _cache.Remove(cacheID);
            }
            
            guid = EditorAssetUtilities.GetSingletonAssetGuidOfTypeByName<T>(assetName);
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }
                
            _cache.Add(cacheID, guid);
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        public void OnBeforeSerialize()
        {
            if (_cache == null)
            {
                return;
            }
            
            _serializedEntries.Clear();
            foreach (KeyValuePair<string, string> keyValuePair in _cache)
            {
                _serializedEntries.Add(new CacheEntry(keyValuePair.Key, keyValuePair.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            _cache = new Dictionary<string, string>(_serializedEntries.Count);
            for (var i = 0; i < _serializedEntries.Count; i++)
            {
                CacheEntry entry = _serializedEntries[i];
                _cache.Add(entry.Name, entry.Guid);
            }
        }
        
        public static AssetCache GetOrCreateCache()
        {
            var cache = AssetDatabase.LoadAssetAtPath<AssetCache>(CACHE_PATH);
            if (cache == null)
            {
                cache = CreateInstance<AssetCache>();
                EditorAssetUtilities.CreateAsset(cache, CACHE_PATH);
                AssetDatabase.SaveAssets();
            }

            return cache;       
        }

        [Serializable]
        private struct CacheEntry
        {
            public string Name;
            public string Guid;

            public CacheEntry(string name, string guid)
            {
                Name = name;
                Guid = guid;
            }
        }
    }
}