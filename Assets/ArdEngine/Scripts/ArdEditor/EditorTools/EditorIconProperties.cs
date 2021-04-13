﻿using UnityEditor;
using UnityEngine;

namespace ArdEditor.EditorTools
{
    public static class EditorIconProperties
    {
        public static readonly GUIContent InfoIcon = EditorGUIUtility.IconContent("console.infoicon");
        public static readonly GUIContent WarningIcon = EditorGUIUtility.IconContent("console.warnicon");
        public static readonly GUIContent ErrorIcon = EditorGUIUtility.IconContent("console.erroricon");
        public static readonly GUIContent SearchIcon = EditorGUIUtility.IconContent("Search Icon");
        public static readonly GUIContent RestoreWindowIcon = EditorGUIUtility.IconContent("winbtn_win_restore");
    }
}