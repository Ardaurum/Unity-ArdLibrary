using System.Collections;
using System.Threading.Tasks;
using ArdEngine.ThreadTools;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace PlayTests.ThreadTools
{
    public sealed class MainThreadDispatcherTests
    {
        [Test]
        public void QueueTask_OnMainThread_RunsTaskImmediately()
        {
            MainThreadDispatcher.QueueTask(Assert.Pass);
            Assert.Fail();
        }

        [UnityTest]
        public IEnumerator QueueTask_OnAnotherThread_RunsTaskInAMainThread()
        {
            var test = false;
            Task.Run(() => MainThreadDispatcher.QueueTask(() => test = true));
            yield return null;
            yield return null;
            Assert.IsTrue(test);
        }
    }
}