﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArdEngine.DataTools;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.EditorTools
{
    public sealed class AutocompleteWindow : EditorWindow
    {
        private static readonly Vector2 MinSize = new Vector2(300.0f, 200.0f);
        private static readonly Vector2 Size = new Vector2(320.0f, 300.0f);
        private static readonly GUIContent SearchField = new GUIContent(EditorIconUtility.SearchIcon);
        
        private Vector2 _scroll;
        private string _query;
        private IReadOnlyList<string> _options;
        private List<int> _filteredOptions;
        private Action<int> _callback;

        [PublicAPI]
        public static void Show(Vector2 position, IReadOnlyList<string> options, Action<int> callback)
        {
            Show(new Rect(position, Size), options, callback);
        }

        public static void ShowBelow(Rect rect, IReadOnlyList<string> options, Action<int> callback)
        {
            Show(new Rect(rect.xMin, rect.yMax, rect.width, Size.y), options, callback);
        }

        [PublicAPI]
        public static void Show(Rect rect, IReadOnlyList<string> options, Action<int> callback)
        {
            var window = CreateInstance<AutocompleteWindow>();
            window.InitWindow(rect, options, callback);
            window.ShowPopup();
        }

        private void InitWindow(Rect rect, IReadOnlyList<string> options, Action<int> callback)
        {
            SearchField.text = string.Empty;
            rect.width = Mathf.Max(rect.width, MinSize.x);
            rect.height = Mathf.Max(rect.height, MinSize.y);
            rect.position = GUIUtility.GUIToScreenPoint(rect.position);
            position = rect;
            _options = options;
            _filteredOptions = new List<int>();
            _callback = callback;
            FilterOptions();
        }

        private void OnLostFocus()
        {
            Close();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            _query = EditorGUILayout.TextField(_query, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                FilterOptions();
            }
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            for (var i = 0; i < _filteredOptions.Count; i++)
            {
                if (GUILayout.Button(_options[_filteredOptions[i]]))
                {
                    _callback.Invoke(_filteredOptions[i]);   
                }
            }
            GUI.EndScrollView();
            
            //TODO: HANDLE KEYBOARD INPUT
            /*
            Event currentEvent = Event.current;
			if (currentEvent.type == EventType.KeyUp)
			{
				HandleButtons(currentEvent);
			}
             */
        }

        //TODO: Move to a static utility method
        private void FilterOptions()
        {
            _filteredOptions.Clear();
            if (string.IsNullOrEmpty(_query))
            {
                _filteredOptions = Enumerable.Range(0, _options.Count).ToList();
            }
            else
            {
                for (var i = 0; i < _options.Count; i++)
                {
                    if (_options[i].FuzzyContains(_query))
                    {
                        _filteredOptions.Add(i);
                    }
                }
            }
        }
    }
}