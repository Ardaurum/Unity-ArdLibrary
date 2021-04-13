//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: 13/04/2021 22:21:44
//========================================================

using System.Collections.Generic;
using System.Linq;
using ArdEngine.DataTools;

namespace ArdEngine.ResourceReference.Example
{
	public sealed class TestValueRepository : IResourceReferenceRepository<TestValue>
	{
		private readonly IReadOnlyDictionary<int, TestValue> _repository;

		public TestValueRepository(TestValueSet set)
		{
			_repository = set.Data.ToDictionary(d => d.Key.GetStableHash(), d => d.Value);
		}

		public TestValue GetValue(int key)
		{
			return _repository[key];
		}
	}
}
