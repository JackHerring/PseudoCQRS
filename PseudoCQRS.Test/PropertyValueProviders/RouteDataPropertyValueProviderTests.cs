using Microsoft.AspNetCore.Http;
using PseudoCQRS.PropertyValueProviders;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.PropertyValueProviders
{
	public class RouteDataPropertyValueProviderTests
	{
		private readonly RouteDataPropertyValueProvider _valueProvider;
		private readonly MockHttpContextWrapper _httpContextWrapper;

		public RouteDataPropertyValueProviderTests()
		{
			_httpContextWrapper = new MockHttpContextWrapper();
			_valueProvider = new RouteDataPropertyValueProvider( _httpContextWrapper );
		}


		[Fact]
		public void HasValueShouldReturnTrueWhenFormContainsKey()
		{
			const string key = "id";
			const string value = "1324";
			_httpContextWrapper.SetRouteDataItem( key, value );

			Assert.True( _valueProvider.HasValue<string>( key ) );
		}

		[Fact]
		public void HasValueShouldReturnFalseWhenFormDoesNotContainKey()
		{
			const string key = "id";
			Assert.False( _valueProvider.HasValue<string>( key ) );
		}


		[Fact]
		public void GetValueShouldReturnValue()
		{
			const string key = "id";
			const string value = "1324";
			_httpContextWrapper.SetRouteDataItem( key, value );

			Assert.Equal( value, _valueProvider.GetValue<object>( key, typeof(string) ) );
		}
	}
}