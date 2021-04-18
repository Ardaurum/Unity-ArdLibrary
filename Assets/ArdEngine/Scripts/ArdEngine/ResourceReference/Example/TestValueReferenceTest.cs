using UnityEngine;

namespace ArdEngine.ResourceReference.Example
{
    public sealed class TestValueReferenceTest : MonoBehaviour
    {
        [SerializeField, TestValue] private int _value;
    }
}