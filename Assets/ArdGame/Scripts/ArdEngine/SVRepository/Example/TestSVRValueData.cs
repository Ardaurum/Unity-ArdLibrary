//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: 09/02/2021 23:35:37
//========================================================

using UnityEngine;
using System.Collections.Generic;

namespace ArdEngine.SVRepository.Example
{
	public sealed class TestSVRValueData : ScriptableObject
	{
		[SerializeField] private TestSVRValuePair[] _data;
		public IReadOnlyList<TestSVRValuePair> Data => _data;
	}
}
