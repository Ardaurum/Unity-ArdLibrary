using System.IO;
using ArdEditor.EditorTools;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ArdEditor.GymlEditor
{
    [ScriptedImporter(1, "gyml")]
    public class GymlImporter : ScriptedImporter
    {
        public TextAsset GymlAsset;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string fileText = File.ReadAllText(ctx.assetPath);
            GymlAsset = new TextAsset(fileText);
            
            ctx.AddObjectToAsset("Text Asset", GymlAsset, ArdEditorIcons.GymlIcon);
            ctx.SetMainObject(GymlAsset);
        }
    }
}