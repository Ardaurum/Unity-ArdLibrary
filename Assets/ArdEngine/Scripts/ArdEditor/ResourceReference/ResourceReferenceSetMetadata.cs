using System;
using ArdEditor.AssetUtilities;
using ArdEngine.ResourceReference;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ArdEditor.ResourceReference
{
    //TODO: Use this in ResourceReferenceEditorWindow
    public readonly struct ResourceReferenceSetMetadata
    {
        public readonly string Name;
        public readonly ResourceReferenceSet Set;

        public ResourceReferenceSetMetadata(Type setType)
        {
            string name = setType.Name;
            name = name.Remove(name.Length - 3, 3);
            Name = ObjectNames.NicifyVariableName(name);
            Object[] sets = EditorAssetUtilities.FindAssetsOfType(setType);
            if (sets.Length > 0)
            {
                Set = (ResourceReferenceSet) sets[0];
            }

            Set = null;
        }
    }
}