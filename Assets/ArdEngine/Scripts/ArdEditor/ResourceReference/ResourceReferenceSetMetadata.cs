using System;
using System.Collections.Generic;
using ArdEditor.AssetUtilities;
using ArdEngine.ResourceReference;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ArdEditor.ResourceReference
{
    public readonly struct ResourceReferenceSetMetadata
    {
        public readonly string Name;
        public readonly ResourceReferenceSet Set;

        public ResourceReferenceSetMetadata(Type setType)
        {
            string name = setType.Name;
            name = name.Remove(name.Length - 3, 3);
            Name = ObjectNames.NicifyVariableName(name);
            IReadOnlyList<Object> sets = EditorAssetUtilities.FindAssetsOfType(setType);
            if (sets.Count > 0)
            {
                Set = (ResourceReferenceSet) sets[0];
            }

            Set = null;
        }
    }
}