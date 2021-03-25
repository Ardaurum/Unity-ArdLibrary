using System;
using System.Collections.Generic;
using ArdEngine.DataTools;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ArdEditor.EditorTools
{
    public sealed class SearchableListUI
    {
        private ReorderableList List { get; }

        private readonly Action _clearCacheDelegate;
        private readonly List<bool> _searchIndices;
        private readonly Func<int, string> _indexToKey;

        private readonly bool _draggable;

        [PublicAPI] public string SearchKey { get; private set; }

        [PublicAPI] public bool IsSearching => string.IsNullOrEmpty(SearchKey) == false;

        public SearchableListUI(ReorderableList list, Func<int, string> indexToKey)
        {
            List = list;
            _clearCacheDelegate = list.CreateClearCacheMethod();
            _draggable = list.draggable;
            _searchIndices = new List<bool>(list.count);
            _indexToKey = indexToKey;

            ReorderableList.ElementCallbackDelegate drawElementCallback = List.drawElementCallback;
            List.drawElementCallback = (rect, index, active, focused) =>
            {
                if (IsElementValid(index))
                {
                    drawElementCallback(rect, index, active, focused);
                }
            };

            ReorderableList.ElementHeightCallbackDelegate elementHeightCallback = List.elementHeightCallback;
            List.elementHeightCallback = index =>
            {
                //ReSharper disable once ConvertToLambdaExpression
                //Unity issue: height of `0` causes infinite loop
                return IsElementValid(index)
                    ? elementHeightCallback?.Invoke(index) ?? EditorGUIUtility.singleLineHeight
                    : .0001f;
            };

            ReorderableList.CanAddCallbackDelegate canAddCallback = list.onCanAddCallback;
            List.onCanAddCallback = reorderableList =>
                IsSearching == false && canAddCallback?.Invoke(reorderableList) != false;

            ReorderableList.CanRemoveCallbackDelegate canRemoveCallback = list.onCanRemoveCallback;
            List.onCanRemoveCallback = reorderableList =>
                IsSearching == false && canRemoveCallback?.Invoke(reorderableList) != false;
        }

        private bool IsElementValid(int index)
        {
            return IsSearching == false || _searchIndices[index];
        }

        [PublicAPI]
        public float GetHeight()
        {
            return List.GetHeight() + EditorGUIUtility.singleLineHeight;
        }

        [PublicAPI]
        public void DoLayoutList()
        {
            Rect rect = GUILayoutUtility.GetRect(0.0f, GetHeight(), GUILayout.ExpandWidth(true));
            DoList(rect);
        }

        [PublicAPI]
        public void DoList(Rect rect)
        {
            DoList(rect, EditorRectUtilities.InfinityRect);
        }

        [PublicAPI]
        public void DoList(Rect rect, Rect visibleRect)
        {
            Rect searchRect = rect;
            searchRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginChangeCheck();
            SearchKey = EditorGUI.TextField(searchRect, SearchKey, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                bool editable = IsSearching == false;
                List.draggable = editable && _draggable;
                FilterList();
            }

            rect = rect.MoveBelow(searchRect);
            visibleRect.height -= searchRect.height;
            List.DoList(rect, visibleRect);
        }

        private void FilterList()
        {
            _searchIndices.Clear();
            for (var i = 0; i < List.count; i++)
            {
                string key = _indexToKey(i);
                _searchIndices.Add(key.FuzzyContains(SearchKey));
            }

            _clearCacheDelegate();
        }
    }
}