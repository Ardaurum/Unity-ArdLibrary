using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace ArdEditor.GymlEditor
{
    public static class SimpleGymlDrawer
    {
        public static void DrawEditor(SimpleGymlDrawerContext ctx)
        {
            IList<YamlDocument> documents = ctx.Stream.Documents;
            for (var i = 0; i < documents.Count; i++)
            {
                var root = (YamlMappingNode) documents[i].RootNode;
                IDictionary<YamlNode, YamlNode> rootChildren = root.Children;
                foreach (KeyValuePair<YamlNode, YamlNode> node in rootChildren)
                {
                    DrawNode(node.Key, node.Value, ctx);
                }
            }
        }

        private static void DrawNode(YamlNode key, YamlNode value, SimpleGymlDrawerContext ctx)
        {
            switch (value.NodeType)
            {
                case YamlNodeType.Scalar:
                    DrawValueNode((YamlScalarNode) key, (YamlScalarNode) value);
                    break;
                case YamlNodeType.Mapping:
                    if (DrawFoldoutNode(key, ctx))
                    {
                        EditorGUI.indentLevel++;
                        DrawMappingNode((YamlMappingNode) value, ctx);
                        EditorGUI.indentLevel--;
                    }

                    break;
                case YamlNodeType.Sequence:
                    if (DrawFoldoutNode(key, ctx))
                    {
                        var sequenceNode = (YamlSequenceNode) value;
                        EditorGUI.indentLevel++;
                        DrawSequenceNode(sequenceNode, ctx);
                        EditorGUI.indentLevel--;
                    }

                    break;
                case YamlNodeType.Alias:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool DrawFoldoutNode(YamlNode node, SimpleGymlDrawerContext ctx)
        {
            ctx.Foldouts.TryGetValue(node, out bool expanded);
            ctx.Foldouts[node] = EditorGUILayout.Foldout(expanded, node.ToString(), true);
            return ctx.Foldouts[node];
        }

        private static void DrawValueNode(YamlScalarNode key, YamlScalarNode value)
        {
            EditorGUILayout.BeginHorizontal();
            key.Value = EditorGUILayout.TextField(key.Value);
            if (value != null)
            {
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                value.Value = EditorGUILayout.TextField(value.Value);
                EditorGUI.indentLevel = indentLevel;
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawMappingNode(YamlMappingNode mappingNode, SimpleGymlDrawerContext ctx)
        {
            foreach (KeyValuePair<YamlNode, YamlNode> node in mappingNode.Children)
            {
                DrawNode(node.Key, node.Value, ctx);
            }
        }

        private static void DrawSequenceNode(YamlSequenceNode sequenceNode, SimpleGymlDrawerContext ctx)
        {
            IList<YamlNode> children = sequenceNode.Children;
            for (var i = 0; i < children.Count; i++)
            {
                switch (children[i].NodeType)
                {
                    case YamlNodeType.Mapping:
                        DrawMappingNode((YamlMappingNode) children[i], ctx);
                        break;
                    case YamlNodeType.Scalar:
                        DrawValueNode((YamlScalarNode) children[i], null);
                        break;
                    default:
                        Debug.LogWarning("Unsupported Node!");
                        return;
                }
            }
        }
    }

    public readonly struct SimpleGymlDrawerContext
    {
        public readonly YamlStream Stream;
        public readonly Dictionary<YamlNode, bool> Foldouts;

        public SimpleGymlDrawerContext(YamlStream stream)
        {
            Stream = stream;
            Foldouts = new Dictionary<YamlNode, bool>();
        }
    }
}