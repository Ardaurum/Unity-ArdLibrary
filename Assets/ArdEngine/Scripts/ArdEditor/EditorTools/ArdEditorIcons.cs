using ArdEditor.AssetUtilities;
using UnityEngine;

namespace ArdEditor.EditorTools
{
    public static class ArdEditorIcons
    {
        public static Texture2D GymlIcon => AssetCache.GetSingletonAssetWithName<Texture2D>("GymlIcon");
    }
}