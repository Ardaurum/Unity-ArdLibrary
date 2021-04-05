using System;
using System.IO;
using ArdEditor.AssetUtilities;
using ArdEditor.CodeGeneration;
using ArdEngine.ResourceReference;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.ResourceReference
{
    public static class ResourceReferenceGenerator
    {
        public const string KEY_FIELD = "Key";
        public const string VALUE_FIELD = "Value";
        
        public static void GenerateAll()
        {
            TypeCache.TypeCollection resourceValues = TypeCache.GetTypesDerivedFrom<IResourceValue>();
            var codeGen = new CodeBuilder();

            for (var i = 0; i < resourceValues.Count; i++)
            {
                Type resourceValue = resourceValues[i];
                string resourcePath = Path.GetDirectoryName(EditorAssetUtilities.GetPathFromAssetName(resourceValue.Name));
                if (resourcePath == string.Empty)
                {
                    Debug.LogError($"{resourceValue.Name} should have the same name as the file!");
                    continue;
                }
                
                GenerateResourceReferencePair(codeGen, resourceValue, resourcePath);
                GenerateResourceReferenceSet(codeGen, resourceValue, resourcePath);
                GenerateResourceReferenceRepository(codeGen, resourceValue, resourcePath);
                GenerateResourceReferenceAttribute(codeGen, resourceValue, resourcePath);
            }
        }

        private static void GenerateResourceReferencePair(CodeBuilder codeGen, Type resourceValue, string resourcePath)
        {
            codeGen.AppendLine("using System;")
                .AppendLine()
                .AppendLine($"namespace {resourceValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine("[Serializable]").AppendLine($"public struct {resourceValue.Name}Pair");
                using (codeGen.CurlyScope())
                {
                    codeGen.AppendLine($"public string {KEY_FIELD};")
                        .AppendLine($"public {resourceValue.Name} {VALUE_FIELD};");
                }
            }

            codeGen.SaveToFile(Path.Combine(resourcePath, $"{resourceValue.Name}Pair.cs"));
        }

        private static void GenerateResourceReferenceSet(CodeBuilder codeGen, Type resourceValue, string resourcePath)
        {
            codeGen.AppendLine("using UnityEngine;")
                .AppendLine("using System.Collections.Generic;")
                .AppendLine()
                .AppendLine($"namespace {resourceValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine($"public sealed class {resourceValue.Name}Set : ResourceReferenceSet");
                using (codeGen.CurlyScope())
                {
                    codeGen.AppendLine($"[{nameof(SerializeField)}] private {resourceValue.Name}Pair[] _data;")
                        .AppendLine($"public IReadOnlyList<{resourceValue.Name}Pair> Data => _data;");
                }
            }

            codeGen.SaveToFile(Path.Combine(resourcePath, $"{resourceValue.Name}Set.cs"));
        }

        private static void GenerateResourceReferenceRepository(CodeBuilder codeGen, Type resourceValue, string resourcePath)
        {
            //TODO: Add ReplaceData() method
            //TODO: Add Observable data <- should be controlled from config, so field shouldn't be observable...
            //More like the object that takes data should have a binding which would kick in and update data if needed.
            
            codeGen.AppendLine("using System.Collections.Generic;")
                .AppendLine("using System.Linq;")
                .AppendLine("using ArdEngine.DataTools;")
                .AppendLine()
                .AppendLine($"namespace {resourceValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine(
                    $"public sealed class {resourceValue.Name}Repository : IResourceReferenceRepository<{resourceValue.Name}>");
                using (codeGen.CurlyScope())
                {
                    //Fields
                    codeGen.AppendLine($"private readonly IReadOnlyDictionary<int, {resourceValue.Name}> _repository;");
                    codeGen.AppendLine();

                    //Constructor
                    codeGen.AppendLine($"public {resourceValue.Name}Repository({resourceValue.Name}Set set)");
                    using (codeGen.CurlyScope())
                    {
                        codeGen.AppendLine(
                            $"_repository = set.Data.ToDictionary(d => d.{KEY_FIELD}.GetStableHash(), d => d.{VALUE_FIELD});");
                    }

                    codeGen.AppendLine();

                    //Methods
                    codeGen.AppendLine($"public {resourceValue.Name} GetValue(int key)");
                    using (codeGen.CurlyScope())
                    {
                        codeGen.AppendLine("return _repository[key];");
                    }
                }
            }

            codeGen.SaveToFile(Path.Combine(resourcePath, $"{resourceValue.Name}Repository.cs"));
        }

        private static void GenerateResourceReferenceAttribute(CodeBuilder codeGen, Type resourceValue, string resourcePath)
        {
            codeGen.AppendLine("using System;")
                .AppendLine()
                .AppendLine($"namespace {resourceValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine($"public sealed class {resourceValue.Name}Attribute : ResourceReferenceAttribute")
                    .AppendLine("{")
                    .AppendLine($"\tpublic override Type DataType => typeof({resourceValue.Name}Set);")
                    .AppendLine("}");
            }

            codeGen.SaveToFile(Path.Combine(resourcePath, $"{resourceValue.Name}Attribute.cs"));
        }
    }
}