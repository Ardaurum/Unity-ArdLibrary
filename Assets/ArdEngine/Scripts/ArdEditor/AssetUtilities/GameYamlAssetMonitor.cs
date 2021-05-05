using System;
using System.Collections.Generic;
using ArdEngine.GameDataTools;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental;

namespace ArdEditor.AssetUtilities
{
    public sealed partial class GameYamlFileCache
    {
        [UsedImplicitly]
        private sealed class GameYamlAssetMonitor : AssetsModifiedProcessor
        {
            protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets,
                AssetMoveInfo[] movedAssets)
            {
                var isDirty = false;
                GameYamlFileCache cache = GetOrCreateCache();
                CacheEntry[] entries = cache._entries ?? new CacheEntry[0];
                string[] assetsToUpdate = FindAssetsToUpdate(addedAssets);
                
                if (assetsToUpdate.Length > 0)
                {
                    int index = entries.Length;
                    Array.Resize(ref entries, assetsToUpdate.Length);
                    for (int i = index; i < index + assetsToUpdate.Length; i++)
                    {
                        entries[i] = new CacheEntry(assetsToUpdate[i]);
                    }

                    isDirty = true;
                }

                assetsToUpdate = FindAssetsToUpdate(deletedAssets);
                var removedEntries = 0;
                for (var i = 0; i < assetsToUpdate.Length; i++)
                {
                    for (var j = 0; j < entries.Length; j++)
                    {
                        if (entries[j].Path == assetsToUpdate[i])
                        {
                            entries[j] = entries[entries.Length - 1 - removedEntries];
                            removedEntries++;
                            isDirty = true;
                        }
                    }
                }
                
                Array.Resize(ref entries, entries.Length - removedEntries);
                
                for (var i = 0; i < movedAssets.Length; i++)
                {
                    for (var j = 0; j < entries.Length; j++)
                    {
                        if (entries[j].Path == movedAssets[i].sourceAssetPath)
                        {
                            entries[j].Path = movedAssets[i].destinationAssetPath;
                            isDirty = true;
                        }
                    }
                }

                if (isDirty)
                {
                    cache._entries = entries;
                    EditorUtility.SetDirty(cache);
                }
            }

            private static string[] FindAssetsToUpdate(IReadOnlyList<string> assets)
            {
                var count = 0;
                var assetsToUpdate = new string[assets.Count];
                for (var i = 0; i < assets.Count; i++)
                {
                    if (assets[i].EndsWith(GameDataHelper.GAME_DATA_EXTENSION))
                    {
                        assetsToUpdate[count++] = assets[i];
                    }
                }

                Array.Resize(ref assetsToUpdate, count);
                return assetsToUpdate;
            }
        }
    }
}