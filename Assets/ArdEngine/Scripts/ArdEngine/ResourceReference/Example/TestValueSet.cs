//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: 13/04/2021 22:21:43
//========================================================

using UnityEngine;
using System.Collections.Generic;

namespace ArdEngine.ResourceReference.Example
{
	public sealed class TestValueSet : ResourceReferenceSet
	{
		[SerializeField] private TestValuePair[] _data;
		public IReadOnlyList<TestValuePair> Data => _data;
	}
}
