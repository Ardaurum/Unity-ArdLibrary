//========================================================
// NO TOUCHING! This file was generated.
// Any changes in this code will be removed when auto-generation runs.
// Generation date: 09/02/2021 23:35:37
//========================================================

using System.Collections.Generic;
using System.Linq;

namespace ArdEngine.SVRepository.Example
{
	public sealed class TestSVRValueRepository : IStringValueRepository<TestSVRValue>
	{
		private readonly IReadOnlyDictionary<int, TestSVRValue> _repository;
		
		public TestSVRValueRepository(TestSVRValueData data)
		{
			_repository = data.Data.ToDictionary(d => d.Key.GetHashCode(), d => d.Value);
		}
		
		public TestSVRValue GetValue(int key)
		{
			return _repository[key];
		}
	}
}
