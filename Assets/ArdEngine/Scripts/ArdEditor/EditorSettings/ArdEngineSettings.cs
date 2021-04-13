using System.IO;
using ArdEditor.AssetUtilities;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using static ArdEditor.AssetUtilities.EditorConstants;

namespace ArdEditor.EditorSettings
{
    public sealed class ArdEngineSettings : ScriptableObject
    {
        private const string SETTINGS_PATH = EDITOR_SETTINGS_PATH + "ArdEngineSettings.asset";

        [SerializeField] private string _dataPath;
        public string DataPath => _dataPath;

        [PublicAPI]
        internal static ArdEngineSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ArdEngineSettings>(SETTINGS_PATH);
            if (settings == null)
            {
                settings = CreateInstance<ArdEngineSettings>();
                settings._dataPath = Path.Combine($"{ASSET_PATH}/RawData/");
                EditorAssetUtilities.CreateAsset(settings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}