using System;
using UnityEngine;

namespace ArdEngine.ResourceReference
{
    public abstract class ResourceReferenceAttribute : PropertyAttribute
    {
        public abstract Type DataType { get; }
    }
}