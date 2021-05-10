using System;
using ArdEngine.ArdAttributes;
using UnityEngine;

namespace ArdEngine.GymlExamples
{
    [Serializable, GymlSchema]
    public struct GymlTestSchema
    {
        public bool TestBool;
        public int TestInt;
        public string TestString;
        public float TestFloat;
        [Range(.0f, 1.0f)] public float TestRange;
    }
}