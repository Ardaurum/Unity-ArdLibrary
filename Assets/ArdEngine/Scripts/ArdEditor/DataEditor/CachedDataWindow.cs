using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArdEditor.DataEditor
{
    [Serializable]
    public struct CachedDataWindow : ISerializationCallbackReceiver
    {
        private const string DRAW_METHOD_NAME = "OnGUI";
        public static CachedDataWindow Empty = new CachedDataWindow();
        
        [SerializeField] private string _name;
        [SerializeField] private EditorWindow _window;
        private Action _drawMethod;

        public string Name => _name;
        public Type WindowType => _window.GetType();
        public bool IsValid => _window != null;

        public CachedDataWindow(string name, Type windowType)
        {
            _name = name;
            _window = (EditorWindow) ScriptableObject.CreateInstance(windowType);
            _drawMethod = GetDrawMethodDelegate(_window);
        }

        public CachedDataWindow(DataWindow dataWindow) : this(dataWindow.Name, dataWindow.WindowType) 
        { }

        public void Close()
        {
            Object.DestroyImmediate(_window);
        }
        
        public void OpenInANewWindow()
        {
            _window.Show();
            _window.titleContent = new GUIContent(Name);
        }

        public void Draw()
        {
            _drawMethod();
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            _drawMethod = GetDrawMethodDelegate(_window);
        }
        
        private static Action GetDrawMethodDelegate(EditorWindow window)
        {
            MethodInfo drawMethodInfo = window.GetType().GetMethod(DRAW_METHOD_NAME, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (drawMethodInfo == null)
            {
                throw new MissingMethodException(window.GetType().Name, DRAW_METHOD_NAME);
            }
            
            return (Action) drawMethodInfo.CreateDelegate(typeof(Action), window);
        }
    }
}