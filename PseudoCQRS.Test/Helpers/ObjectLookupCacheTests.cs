using System;
using PseudoCQRS.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Helpers
{
	public class ObjectLookupCacheTests
	{
		private readonly ObjectLookupCache _cache;

		public ObjectLookupCacheTests()
		{
			_cache = new ObjectLookupCache();
		}

		[Fact]
		public void CanReadAndWrite()
		{
			const string key = "Test124";
			const int defaultValue = 1223;
			const int savedValue = 101;
			Assert.Equal( defaultValue, _cache.GetValue( key, defaultValue ) );
			_cache.SetValue( key, savedValue );
			Assert.Equal( savedValue, _cache.GetValue( key, defaultValue ) );
		}
	}
}