using System;

namespace ArdEditor.DataEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DataEditorAttribute : Attribute
    {
        public string Name { get; set; }
    }
}