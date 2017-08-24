using System;
using Microsoft.AspNetCore.Http;
using PseudoCQRS.PropertyValueProviders;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.PropertyValueProviders
{
	public class QueryStringPropertyValueProviderTests
	{
		private readonly QueryStringPropertyValueProvider _valueProvider;
		private readonly MockHttpContextWrapper _httpContextWrapper;

		public QueryStringPropertyValueProviderTests()
		{
			_httpContextWrapper = new MockHttpContextWrapper();
			_valueProvider = new QueryStringPropertyValueProvider( _httpContextWrapper );
		}


		[Fact]
		public void HasValueShouldReturnTrueWhenQueryStringContainsKey()
		{
			const string key = "id";
			const string value = "1324";
			_httpContextWrapper.SetQueryStringItem( key, value );

			Assert.True( _valueProvider.HasValue<string>( key ) );
		}

		[Fact]
		public void HasValueShouldReturnFalseWhenQueryStringDoesNotContainKey()
		{
			const string key = "id";
			Assert.False( _valueProvider.HasValue<string>( key ) );
		}

		[Fact]
		public void GetValueShouldReturnValue()
		{
			const string key = "id";
			const string value = "1324";
			_httpContextWrapper.SetQueryStringItem(key, value);

			Assert.Equal( value, _valueProvider.GetValue<Object>( key, typeof( string ) ) );
		}
	}
}