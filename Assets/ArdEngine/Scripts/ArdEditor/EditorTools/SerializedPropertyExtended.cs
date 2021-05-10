using UnityEditor;

namespace ArdEditor.EditorTools
{
    public static class SerializedPropertyExtended
    {
        public static float GetMaxPropertyHeight(this SerializedProperty property)
        {
            SerializedProperty propertyCopy = property.Copy();
            propertyCopy.NextVisible(true);
            float max = EditorGUI.GetPropertyHeight(propertyCopy);
            while (propertyCopy.NextVisible(false) && propertyCopy.propertyPath.Contains(property.propertyPath))
            {
                float current = EditorGUI.GetPropertyHeight(propertyCopy);
                if (current > max)
                {
                    max = current;
                }
            }

            return max;
        }
    }
}