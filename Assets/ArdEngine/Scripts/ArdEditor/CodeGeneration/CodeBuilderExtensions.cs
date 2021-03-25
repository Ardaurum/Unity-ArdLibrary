using System;
using ArdEditor.AssetUtilities;

namespace ArdEditor.CodeGeneration
{
    public static class CodeBuilderExtensions
    {
        private const string CODE_HEADER =
@"//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: {0}
//========================================================

";

        public static void SaveToFile(this CodeBuilder builder, string path)
        {
            string header = string.Format(CODE_HEADER, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            EditorAssetUtilities.CreateFile(header + builder, path);
            builder.Clear();
        }
    }
}