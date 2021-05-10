using System;
using UnityEngine;

namespace ArdEngine.ArdAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GymlSchemaAttribute : PropertyAttribute { }
}