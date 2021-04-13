using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArdEditor.EditorTools;
using ArdEngine.DataTools;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.DataEditor
{
    public sealed class DataEditorWindow : EditorWindow
    {
        private DataWindow[] _filteredWindows;
        private DataWindow[] _dataWindows;
        
        [SerializeField] private CachedDataWindow _currentWindow;
        [SerializeField] private Vector2 _scroll;
        [SerializeField] private string _searchKey;

        [MenuItem("Window/Data Editor")]
        public static void ShowWindow()
        {
            GetWindow<DataEditorWindow>("Data Editor");
        }

        private void OnEnable()
        {
            _dataWindows = GetDataWindows();
            _filteredWindows = _dataWindows;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            WindowChoosePanel();
            DataViewPanel();
            EditorGUILayout.EndHorizontal();
        }

        private void WindowChoosePanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(150.0f));
            EditorGUI.BeginChangeCheck();
            _searchKey = EditorGUILayout.TextField(_searchKey, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                _filteredWindows = _dataWindows.Where(window => string.IsNullOrWhiteSpace(_searchKey) ||
                                                                window.Name.FuzzyContains(_searchKey)).ToArray();
            }

            _scroll = GUILayout.BeginScrollView(_scroll, false, true);
            
            for (var i = 0; i < _filteredWindows.Length; i++)
            {
                DataWindow dataWindow = _filteredWindows[i];
                bool isDataWindowOpen = dataWindow.IsOpen();
                bool open = GUILayout.Toggle(isDataWindowOpen, dataWindow.Name, EditorStyles.toolbarButton);
                if (open && isDataWindowOpen == false)
                {
                    _currentWindow = new CachedDataWindow(dataWindow);
                }
                else if (open == false && isDataWindowOpen)
                {
                    if (_currentWindow.IsValid && _currentWindow.WindowType == dataWindow.WindowType)
                    {
                        _currentWindow.Close();
                        _currentWindow = CachedDataWindow.Empty;
                    }
                    else
                    {
                        GetWindow(dataWindow.WindowType).Close();
                    }
                }
            }

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DataViewPanel()
        {
            EditorGUILayout.BeginVertical();
            if (_currentWindow.IsValid)
            {
                DrawCurrentWindow();
            }
            else
            {
                EditorGUILayout.HelpBox("Choose a window first", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCurrentWindow()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(_currentWindow.Name, EditorStyles.boldLabel);
                if (GUILayout.Button(EditorIconProperties.RestoreWindowIcon, GUILayout.Width(32.0f)))
                {
                    _currentWindow.OpenInANewWindow();
                    _currentWindow = CachedDataWindow.Empty;
                    return;
                }
            }

            _currentWindow.Draw();
        }

        private static DataWindow[] GetDataWindows()
        {
            TypeCache.TypeCollection dataWindowTypes =
                TypeCache.GetTypesWithAttribute<DataEditorAttribute>();
            var dataWindows = new List<DataWindow>(dataWindowTypes.Count);
            
            for (var i = 0; i < dataWindowTypes.Count; i++)
            {
                Type dataWindowType = dataWindowTypes[i];
                dataWindows.Add(new DataWindow(
                    dataWindowType.GetCustomAttribute<DataEditorAttribute>().Name,
                    dataWindowType
                ));
            }
            return dataWindows.ToArray();
        }
    }
}