//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: 25/03/2021 22:35:15
//========================================================

using System.Collections.Generic;
using UnityEngine;

namespace ArdEngine.ResourceReference.Example
{
	[CreateAssetMenu(menuName = "Ard/TestResourceValueSet")]
	public sealed class TestResourceValueSet : ResourceReferenceSetScriptable
	{
		[SerializeField] private TestResourceValuePair[] _data;
		public IReadOnlyList<TestResourceValuePair> Data => _data;
	}
}
