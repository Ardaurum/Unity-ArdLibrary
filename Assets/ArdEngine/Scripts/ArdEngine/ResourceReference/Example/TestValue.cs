using System;

namespace ArdEngine.ResourceReference.Example
{
    [Serializable]
    public struct TestValue : IResourceValue
    {
        public int Value;
    }
}