using UnityEditor;
using UnityEngine;

namespace ArdEditor.GymlEditor
{
    public sealed class ScriptableGymlEditorProxy : ScriptableObject
    {
        [SerializeReference] public object Reference;
    }

    [CustomEditor(typeof(ScriptableGymlEditorProxy))]
    public sealed class ScriptableGymlEditorProxyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty iterator = serializedObject.FindProperty(nameof(ScriptableGymlEditorProxy.Reference));
            var expanded = true;
            while (iterator.NextVisible(expanded))
            {
                expanded = false;
                EditorGUILayout.PropertyField(iterator);
            }
        }
    }
}