using System;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.DataEditor
{
    public readonly struct DataWindow
    {
        public readonly string Name;
        public readonly Type WindowType;

        public DataWindow(string name, Type windowType)
        {
            Name = string.IsNullOrEmpty(name) ? ObjectNames.NicifyVariableName(windowType.Name) : name;
            WindowType = windowType;
        }

        public bool IsOpen()
        {
            return Resources.FindObjectsOfTypeAll(WindowType).Length > 0;
        }
    }
}