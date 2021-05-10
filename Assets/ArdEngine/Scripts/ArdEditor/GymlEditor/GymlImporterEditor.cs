using System;
using System.Collections.Generic;
using System.IO;
using ArdEngine.ArdAttributes;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ArdEditor.GymlEditor
{
    [CustomEditor(typeof(GymlImporter))]
    public class GymlImporterEditor : ScriptedImporterEditor
    {
        private YamlStream _yamlStream;
        private TextAsset _gymlAsset;
        private GymlHeader _header;
        private SimpleGymlDrawerContext _drawerContext;

        private readonly List<object> _serializedObjects = new List<object>();
        private readonly List<Editor> _gymlSchemaEditors = new List<Editor>();

        protected override bool needsApplyRevert => false;

        public override void OnEnable()
        {
            base.OnEnable();
            ReadGymlData();
        }

        public void OnDestroy()
        {
            string assetPath = AssetDatabase.GetAssetPath(target);
            AssetDatabase.ImportAsset(assetPath);
        }

        public override void ReloadPreviewInstances()
        {
            ReadGymlData();
        }

        private void ReadGymlData()
        {
            _serializedObjects.Clear();
            _gymlSchemaEditors.Clear();
            _yamlStream = null;
            
            _gymlAsset = ((GymlImporter) target).GymlAsset;
            string gymlContent = _gymlAsset.text;

            using (var rawInput = new StringReader(gymlContent))
            {
                if (ParseSchemaGyml(rawInput))
                {
                    return;
                }
            }

            using (var rawInput = new StringReader(gymlContent))
            {
                _yamlStream = new YamlStream();
                _yamlStream.Load(rawInput);
                _drawerContext = new SimpleGymlDrawerContext(_yamlStream);
            }
        }

        private bool ParseSchemaGyml(TextReader rawInput)
        {
            var gymlParser = new Parser(rawInput);
            gymlParser.Expect<StreamStart>();
            gymlParser.Expect<DocumentStart>();
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            try
            {
                _header = deserializer.Deserialize<GymlHeader>(gymlParser);
            }
            catch (YamlException)
            {
                //File doesn't have a header. Not an issue so ignore.
                return false;
            }

            TypeCache.TypeCollection schemas = TypeCache.GetTypesWithAttribute<GymlSchemaAttribute>();
            Type gymlSchemaType = null;
            for (var i = 0; i < schemas.Count; i++)
            {
                if (schemas[i].Name.Equals(_header.Schema))
                {
                    gymlSchemaType = schemas[i];
                    break;
                }
            }

            if (gymlSchemaType == null)
            {
                Debug.LogWarning($"No schema file {_header.Schema} found!");
                return false;
            }

            while (gymlParser.Accept<StreamEnd>() == false)
            {
                gymlParser.MoveNext();
                if (gymlParser.Accept<DocumentStart>() == false)
                {
                    continue;
                }

                var proxy = CreateInstance<ScriptableGymlEditorProxy>();
                try
                {
                    proxy.Reference = deserializer.Deserialize(gymlParser, gymlSchemaType);
                }
                catch (YamlException ex)
                {
                    DestroyImmediate(proxy);
                    Debug.LogWarning($"Gyml file structure is not valid with the schema: {ex}");
                    continue;
                }

                _serializedObjects.Add(proxy.Reference);
                _gymlSchemaEditors.Add(CreateEditor(proxy));
            }

            return _serializedObjects.Count > 0;
        }

        public override void OnInspectorGUI()
        {
            if (_gymlSchemaEditors.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                for (var i = 0; i < _gymlSchemaEditors.Count; i++)
                {
                    _gymlSchemaEditors[i].OnInspectorGUI();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    SaveSchemaGyml();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                SimpleGymlDrawer.DrawEditor(_drawerContext);
                if (EditorGUI.EndChangeCheck())
                {
                    using var outputStream = new StringWriter();
                    _yamlStream.Save(outputStream);
                    var saveData = outputStream.ToString();
                    string assetPath = AssetDatabase.GetAssetPath(target);
                    File.WriteAllText(assetPath, saveData);
                }
            }
        }

        private void SaveSchemaGyml()
        {
            using var outputStream = new StringWriter();
            var emitter = new Emitter(outputStream);
            IValueSerializer serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .EmitDefaults()
                .BuildValueSerializer();

            emitter.Emit(new StreamStart());
            emitter.Emit(new DocumentStart());
            serializer.SerializeValue(emitter, _header, typeof(GymlHeader));
            emitter.Emit(new DocumentEnd(true));
            for (var i = 0; i < _gymlSchemaEditors.Count; i++)
            {
                _gymlSchemaEditors[i].serializedObject.ApplyModifiedProperties();
                emitter.Emit(new DocumentStart());
                var proxy = (ScriptableGymlEditorProxy) _gymlSchemaEditors[i].target;
                serializer.SerializeValue(emitter, proxy.Reference, proxy.Reference.GetType());
                emitter.Emit(new DocumentEnd(true));
            }
            emitter.Emit(new StreamEnd());
            
            var saveData = outputStream.ToString();
            string assetPath = AssetDatabase.GetAssetPath(target);
            File.WriteAllText(assetPath, saveData);
        }
    }
}