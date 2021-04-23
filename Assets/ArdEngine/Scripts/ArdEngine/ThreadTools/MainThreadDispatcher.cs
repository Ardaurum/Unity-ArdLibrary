using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArdEngine.ThreadTools
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    public sealed class MainThreadDispatcher
    {
        private static readonly int MainThreadId = Thread.CurrentThread.ManagedThreadId;
        private static MainThreadDispatcher _instance;

        #if UNITY_EDITOR
        static MainThreadDispatcher()
        {
            Initialize();
            UnityEditor.EditorApplication.playModeStateChanged += _ => Initialize();
        }
        #endif

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _instance = new MainThreadDispatcher();
        }
            
        public static void QueueTask(Action task)
        {
            if (MainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                task();
            }
            else
            {
                _instance._taskQueue.Enqueue(task);
            }
        }

        private readonly ConcurrentQueue<Action> _taskQueue;

        private MainThreadDispatcher()
        {
            _taskQueue = new ConcurrentQueue<Action>();
            
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                UnityEditor.EditorApplication.update += Update;
                return;
            }
            #endif
            var updateProvider = new GameObject("ArdEngine: MainThreadDispatcher");
            updateProvider.AddComponent<RuntimeUpdateProvider>().MainThreadDispatcher = this;
            Object.DontDestroyOnLoad(updateProvider);
        }

        private void Update()
        {
            while (_taskQueue.TryDequeue(out Action task))
            {
                task();
            }
        }

        private sealed class RuntimeUpdateProvider : MonoBehaviour
        {
            public MainThreadDispatcher MainThreadDispatcher;

            private void Update()
            {
                MainThreadDispatcher.Update();
            }
        }
    }
}