using System.Reflection;

namespace ArdEditor.ReflectionTools
{
    public static class ReflectionHelper
    {
        public const BindingFlags BINDING_GET_ALL =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    }
}