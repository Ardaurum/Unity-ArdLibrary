using System;
using YamlDotNet.Serialization;

namespace ArdEditor.GymlEditor
{
    [Serializable]
    public struct GymlHeader
    {
        [YamlMember(Alias = "CS_SCHEMA_FILE", ApplyNamingConventions = false)]
        public string Schema { get; set; }
    }
}