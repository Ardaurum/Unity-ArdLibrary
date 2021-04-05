using System;
using System.Collections.Generic;
using System.Reflection;
using ArdEditor.EditorTools;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.DataEditor
{
    public sealed class DataEditorWindow : EditorWindow
    {
        private DataWindow[] _dataWindows;
        
        [SerializeField] private CachedDataWindow _currentWindow;
        [SerializeField] private Vector2 _scroll;

        [MenuItem("Window/Data Editor")]
        public static void ShowWindow()
        {
            GetWindow<DataEditorWindow>("Data Editor");
        }

        private void OnEnable()
        {
            _dataWindows = GetDataWindows();
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
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(144.0f));
            _scroll = GUILayout.BeginScrollView(_scroll, false, true);
            
            for (var i = 0; i < _dataWindows.Length; i++)
            {
                DataWindow dataWindow = _dataWindows[i];
                bool isDataWindowOpen = dataWindow.IsOpen();
                bool open = GUILayout.Toggle(isDataWindowOpen, dataWindow.Name, EditorStyles.toolbarButton);
                if (open && isDataWindowOpen == false)
                {
                    _currentWindow = new CachedDataWindow(_dataWindows[i]);
                }
                else if (open == false && isDataWindowOpen)
                {
                    if (_currentWindow.IsValid && _currentWindow.WindowType == _dataWindows[i].WindowType)
                    {
                        _currentWindow.Close();
                        _currentWindow = CachedDataWindow.Empty;
                    }
                    else
                    {
                        GetWindow(_dataWindows[i].WindowType).Close();
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
                if (GUILayout.Button(EditorIconUtility.RestoreWindowIcon, GUILayout.Width(32.0f)))
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