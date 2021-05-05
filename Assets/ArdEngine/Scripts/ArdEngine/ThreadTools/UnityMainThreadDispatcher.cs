using UnityEngine;
using Object = UnityEngine.Object;

namespace ArdEngine.ThreadTools
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    public static class UnityMainThreadDispatcher
    {
        #if UNITY_EDITOR
        static UnityMainThreadDispatcher()
        {
            Initialize();
            UnityEditor.EditorApplication.playModeStateChanged += _ => Initialize();
        }
        #endif

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                UnityEditor.EditorApplication.update += MainThreadDispatcher.Instance.Update;
                return;
            }
            #endif
            var updateProvider = new GameObject("ArdEngine: MainThreadDispatcher");
            Object.DontDestroyOnLoad(updateProvider);
        }

        private sealed class RuntimeUpdateProvider : MonoBehaviour
        {
            private void Update()
            {
                MainThreadDispatcher.Instance.Update();
            }
        }
    }
}