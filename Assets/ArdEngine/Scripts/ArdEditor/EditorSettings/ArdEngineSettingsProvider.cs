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
        private readonly SerializedObject _settings;

        private ArdEngineSettingsProvider(string path, SerializedObject settings, IEnumerable<string> keywords = null) :
            base(path, SettingsScope.Project, keywords)
        {
            _settings = settings;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            //todo: Load using cached GUID after using `FindAsset`
            SerializedProperty pathProperty = _settings.FindProperty("_dataPath");

            var visualTreeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EDITOR_SETTINGS_PATH + "UI/ard_settings.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(EDITOR_SETTINGS_PATH + "UI/ard_settings.uss");
            rootElement.styleSheets.Add(styleSheet);
            TemplateContainer visualTree = visualTreeAsset.CloneTree();
            visualTree.Q<TextField>("data-path").BindProperty(pathProperty);
            visualTree.Q<Button>("data-path-browse").clickable.clicked += () =>
            {
                string absolutePath = EditorUtility.OpenFolderPanel("Select Data Path", "Assets", "Data");
                pathProperty.stringValue = absolutePath.ConvertToRelativeProjectPath();
                _settings.ApplyModifiedProperties();
            };

            rootElement.Add(visualTree);
            base.OnActivate(searchContext, rootElement);
        }

        [SettingsProvider]
        public static SettingsProvider FetchArdEngineSettingsProvider()
        {
            SerializedObject settings = ArdEngineSettings.GetSerializedSettings();
            IEnumerable<string> keywords =
                GetSearchKeywordsFromSerializedObject(settings).Union(new[] {"ArdEngine", "Ard"});

            var provider = new ArdEngineSettingsProvider("Project/Ard Engine Settings", settings, keywords);
            return provider;
        }
    }
}