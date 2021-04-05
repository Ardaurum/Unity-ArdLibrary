using ArdEditor.DataEditor;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.ResourceReference
{
    [DataEditor(Name = "Resource References")]
    public sealed class ResourceReferenceEditorWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            GetWindow<ResourceReferenceEditorWindow>("Resource References");
        }

        private void OnEnable()
        {
            
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
        }
    }
}