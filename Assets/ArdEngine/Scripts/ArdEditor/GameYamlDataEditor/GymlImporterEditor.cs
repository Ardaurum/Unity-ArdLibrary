using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace ArdEditor.GameYamlDataEditor
{
    [CustomEditor(typeof(GymlImporter))]
    public class GymlImporterEditor : ScriptedImporterEditor
    {
        private YamlStream _yamlStream;
        private TextAsset _gymlAsset;

        protected override bool needsApplyRevert => false;

        public override void OnEnable()
        {
            base.OnEnable();
            ReadGymlData();
        }

        private void ReadGymlData()
        {
            _gymlAsset = ((GymlImporter) target).GymlAsset;
            using var rawInput = new StringReader(_gymlAsset.text);
            _yamlStream = new YamlStream();
            _yamlStream.Load(rawInput);
        }

        public override void ReloadPreviewInstances()
        {
            ReadGymlData();
        }

        public override void OnInspectorGUI()
        {
            IList<YamlDocument> documents = _yamlStream.Documents;
            for (var i = 0; i < documents.Count; i++)
            {
                IDictionary<YamlNode, YamlNode> root = ((YamlMappingNode) documents[i].RootNode).Children;
                foreach (KeyValuePair<YamlNode, YamlNode> node in root)
                {
                    PrintNode(node.Key, node.Value);
                }
            }
        }

        private void PrintNode(YamlNode key, YamlNode value)
        {
            if (value.NodeType == YamlNodeType.Scalar)
            {
                PrintValueNode((YamlScalarNode) key, (YamlScalarNode) value);
                return;
            }

            if (value.NodeType == YamlNodeType.Mapping)
            {
                EditorGUI.indentLevel++;
                foreach (KeyValuePair<YamlNode, YamlNode> node in ((YamlMappingNode)value).Children)
                {
                    PrintNode(node.Key, node.Value);
                }
                EditorGUI.indentLevel--;

                return;
            }
            
            if (value.NodeType == YamlNodeType.Sequence)
            {
                //TODO: Array support
            }
        }

        private void PrintValueNode(YamlScalarNode key, YamlScalarNode value)
        {
            EditorGUI.BeginChangeCheck();
            value.Value = EditorGUILayout.TextField(key.Value, value.Value);
            if (EditorGUI.EndChangeCheck())
            {
                using var outputStream = new StringWriter();
                _yamlStream.Save(outputStream);
                var saveData = outputStream.ToString();
                File.WriteAllText(AssetDatabase.GetAssetPath(target), saveData);
            }
        }
    }
}