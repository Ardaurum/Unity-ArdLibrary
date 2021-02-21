using System;
using System.IO;
using ArdEditor.AssetUtilities;
using ArdEditor.CodeGeneration;
using ArdEngine.SVRepository;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.SVRepository
{
    public static class SVRepositoryGenerator
    {
        [MenuItem("Ard/Generate")]
        public static void Generate()
        {
            TypeCache.TypeCollection svrValues = TypeCache.GetTypesDerivedFrom<ISVRValue>();
            var codeGen = new CodeBuilder();

            for (var i = 0; i < svrValues.Count; i++)
            {
                Type svrValue = svrValues[i];
                string svrPath = Path.GetDirectoryName(EditorAssetUtilities.GetPathFromAssetName(svrValue.Name));
                if (svrPath == string.Empty)
                {
                    Debug.LogError($"{svrValue.Name} should have the same name as the file!");
                    continue;
                }
                
                GenerateSVRPair(codeGen, svrValue, svrPath);
                GenerateSVRData(codeGen, svrValue, svrPath);
                GenerateSVRepository(codeGen, svrValue, svrPath);
                GenerateSVRAttribute(codeGen, svrValue, svrPath);
            }
        }

        private static void GenerateSVRPair(CodeBuilder codeGen, Type svrValue, string svrPath)
        {
            codeGen.AppendLine("using System;")
                .AppendLine()
                .AppendLine($"namespace {svrValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine("[Serializable]").AppendLine($"public struct {svrValue.Name}Pair");
                using (codeGen.CurlyScope())
                {
                    codeGen.AppendLine("public string Key;")
                        .AppendLine($"public {svrValue.Name} Value;");
                }
            }

            codeGen.SaveToFile(Path.Combine(svrPath, $"{svrValue.Name}Pair.cs"));
        }

        private static void GenerateSVRData(CodeBuilder codeGen, Type svrValue, string svrPath)
        {
            codeGen.AppendLine("using UnityEngine;")
                .AppendLine("using System.Collections.Generic;")
                .AppendLine()
                .AppendLine($"namespace {svrValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine($"public sealed class {svrValue.Name}Data : ScriptableObject");
                using (codeGen.CurlyScope())
                {
                    codeGen.AppendLine($"[{nameof(SerializeField)}] private {svrValue.Name}Pair[] _data;")
                        .AppendLine($"public IReadOnlyList<{svrValue.Name}Pair> Data => _data;");
                }
            }

            codeGen.SaveToFile(Path.Combine(svrPath, $"{svrValue.Name}Data.cs"));
        }

        private static void GenerateSVRepository(CodeBuilder codeGen, Type svrValue, string svrPath)
        {
            codeGen.AppendLine("using System.Collections.Generic;")
                .AppendLine("using System.Linq;")
                .AppendLine()
                .AppendLine($"namespace {svrValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine(
                    $"public sealed class {svrValue.Name}Repository : IStringValueRepository<{svrValue.Name}>");
                using (codeGen.CurlyScope())
                {
                    //Fields
                    codeGen.AppendLine($"private readonly IReadOnlyDictionary<int, {svrValue.Name}> _repository;");
                    codeGen.AppendLine();

                    //Constructor
                    codeGen.AppendLine($"public {svrValue.Name}Repository({svrValue.Name}Data data)");
                    using (codeGen.CurlyScope())
                    {
                        codeGen.AppendLine(
                            "_repository = data.Data.ToDictionary(d => d.Key.GetHashCode(), d => d.Value);");
                    }

                    codeGen.AppendLine();

                    //Methods
                    codeGen.AppendLine($"public {svrValue.Name} GetValue(int key)");
                    using (codeGen.CurlyScope())
                    {
                        codeGen.AppendLine("return _repository[key];");
                    }
                }
            }

            codeGen.SaveToFile(Path.Combine(svrPath, $"{svrValue.Name}Repository.cs"));
        }

        private static void GenerateSVRAttribute(CodeBuilder codeGen, Type svrValue, string svrPath)
        {
            codeGen.AppendLine($"namespace {svrValue.Namespace}");
            using (codeGen.CurlyScope())
            {
                codeGen.AppendLine($"public sealed class {svrValue.Name}Attribute : SVRAttribute {{}}");
            }

            codeGen.SaveToFile(Path.Combine(svrPath, $"{svrValue.Name}Attribute.cs"));
        }
    }
}