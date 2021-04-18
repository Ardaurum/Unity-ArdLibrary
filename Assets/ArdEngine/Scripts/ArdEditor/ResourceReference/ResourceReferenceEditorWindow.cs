using System;
using System.Linq;
using ArdEditor.AssetUtilities;
using ArdEditor.DataEditor;
using ArdEditor.EditorSettings;
using ArdEngine.DataTools;
using ArdEngine.ResourceReference;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.ResourceReference
{
    [DataEditor(Name = "Resource References")]
    public sealed class ResourceReferenceEditorWindow : EditorWindow
    {
        [SerializeField] private Vector2 _scroll;
        [SerializeField] private string _selectedSet;
        [SerializeField] private string _searchKey;

        private TypeCache.TypeCollection _resourceSets;
        private Type[] _filteredSets;
        private ArdEngineSettings _settings;
        private Editor _setEditor;

        private void OnEnable()
        {
            _resourceSets = TypeCache.GetTypesDerivedFrom<ResourceReferenceSet>();
            _filteredSets = _resourceSets.ToArray();
            _settings = ArdEngineSettings.GetOrCreateSettings();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Generate Classes"))
            {
                ResourceReferenceGenerator.GenerateAll();
            }

            if (GUILayout.Button("Generate Resources"))
            {
                //TODO: Generate Resource Classes
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(150.0f));
            EditorGUI.BeginChangeCheck();
            _searchKey = EditorGUILayout.TextField(_searchKey, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                _filteredSets = _resourceSets.Where(set => string.IsNullOrWhiteSpace(_searchKey) ||
                                                                set.Name.FuzzyContains(_searchKey)).ToArray();
            }
            
            _scroll = GUILayout.BeginScrollView(_scroll, false, true);
            for (var i = 0; i < _filteredSets.Length; i++)
            {
                bool selected = _selectedSet == _filteredSets[i].FullName;
                string setName = ObjectNames.NicifyVariableName(_filteredSets[i].Name);
                bool changeSelection = GUILayout.Toggle(selected, setName, EditorStyles.toolbarButton);
                if ((changeSelection && selected == false) || _setEditor == null)
                {
                    _selectedSet = _filteredSets[i].FullName;
                    var assetPath = $"{_settings.DataPath}/{_filteredSets[i].Name}.asset";
                    var loadedSet = AssetDatabase.LoadAssetAtPath<ResourceReferenceSet>(assetPath);
                    if (loadedSet == null)
                    {
                        loadedSet = (ResourceReferenceSet) CreateInstance(_filteredSets[i]);
                        EditorAssetUtilities.CreateAsset(loadedSet, assetPath);
                        AssetDatabase.SaveAssets();
                    }
                    
                    _setEditor = Editor.CreateEditor(loadedSet);
                }
                else if (changeSelection == false && selected)
                {
                    _setEditor = null;
                    _selectedSet = string.Empty;
                }
            }

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (_setEditor != null)
            {
                bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = true;
                _setEditor.OnInspectorGUI();
                EditorGUIUtility.hierarchyMode = hierarchyMode;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}