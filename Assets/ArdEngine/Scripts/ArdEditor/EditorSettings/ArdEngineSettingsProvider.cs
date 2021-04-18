using System.Collections.Generic;
using System.Linq;
using ArdEditor.AssetUtilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static ArdEditor.AssetUtilities.EditorConstants;

namespace ArdEditor.EditorSettings
{
    internal sealed class ArdEngineSettingsProvider : SettingsProvider
    {
        private ArdEngineSettingsProvider() : base("Project/Ard Engine Settings", SettingsScope.Project) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            SerializedObject settings = ArdEngineSettings.GetSerializedSettings();
            keywords = GetSearchKeywordsFromSerializedObject(settings).Union(new[] {"ArdEngine", "Ard"});
            
            SerializedProperty pathProperty = settings.FindProperty("_dataPath");

            var visualTreeAsset = AssetCache.GetSingletonAssetWithName<VisualTreeAsset>("ard_settings");
            var styleSheet = AssetCache.GetSingletonAssetWithName<StyleSheet>("ard_settings");
            rootElement.styleSheets.Add(styleSheet);
            TemplateContainer visualTree = visualTreeAsset.CloneTree();
            visualTree.Q<TextField>("data-path").BindProperty(pathProperty);
            visualTree.Q<Button>("data-path-browse").clickable.clicked += () =>
            {
                string absolutePath = EditorUtility.OpenFolderPanel("Select Data Path", "Assets", "Data");
                pathProperty.stringValue = absolutePath.ConvertToRelativeProjectPath();
                settings.ApplyModifiedProperties();
            };

            rootElement.Add(visualTree);
            base.OnActivate(searchContext, rootElement);
        }

        [SettingsProvider]
        public static SettingsProvider FetchArdEngineSettingsProvider()
        {
            return new ArdEngineSettingsProvider();
        }
    }
}