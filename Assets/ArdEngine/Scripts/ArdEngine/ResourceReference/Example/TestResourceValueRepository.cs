//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: 25/03/2021 22:35:15
//========================================================

using System.Collections.Generic;
using System.Linq;
using ArdEngine.DataTools;

namespace ArdEngine.ResourceReference.Example
{
	public sealed class TestResourceValueRepository : IResourceReferenceRepository<TestResourceValue>
	{
		private readonly IReadOnlyDictionary<int, TestResourceValue> _repository;

		public TestResourceValueRepository(TestResourceValueSet set)
		{
			_repository = set.Data.ToDictionary(d => d.Key.GetStableHash(), d => d.Value);
		}

		public TestResourceValue GetValue(int key)
		{
			return _repository[key];
		}
	}
}
