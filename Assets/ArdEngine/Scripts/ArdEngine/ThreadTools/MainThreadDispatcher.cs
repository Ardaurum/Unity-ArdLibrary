using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine.Assertions;

namespace ArdEngine.ThreadTools
{
    public sealed class MainThreadDispatcher
    {
        private static readonly int MainThreadId = Thread.CurrentThread.ManagedThreadId;
        public static readonly MainThreadDispatcher Instance = new MainThreadDispatcher();

        public static void QueueTask(Action task)
        {
            if (MainThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                task();
            }
            else
            {
                Instance._taskQueue.Enqueue(task);
            }
        }

        private readonly ConcurrentQueue<Action> _taskQueue;

        private MainThreadDispatcher()
        {
            _taskQueue = new ConcurrentQueue<Action>();
        }

        public void Update()
        {
            Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == MainThreadId);
            while (_taskQueue.TryDequeue(out Action task))
            {
                task();
            }
        }
    }
}