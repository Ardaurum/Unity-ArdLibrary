using System;
using System.Reflection;
using UnityEditorInternal;

namespace ArdEditor.EditorTools
{
    public static class ReorderableListUtils
    {
        private static readonly MethodInfo ClearCacheMethod =
            typeof(ReorderableList).GetMethod("ClearCache", BindingFlags.Instance | BindingFlags.NonPublic);

        public static Action CreateClearCacheMethod(this ReorderableList list)
        {
            return (Action) Delegate.CreateDelegate(typeof(Action), list, ClearCacheMethod);
        }
    }
}