using System;
using Microsoft.AspNetCore.Http;
using PseudoCQRS.PropertyValueProviders;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.PropertyValueProviders
{
	public class CookiePropertyValueProviderTests
	{
		private readonly CookiePropertyValueProvider _valueProvider;
		private readonly MockHttpContextWrapper _httpContextWrapper;

		public CookiePropertyValueProviderTests()
		{
			_httpContextWrapper = new MockHttpContextWrapper();
			_valueProvider = new CookiePropertyValueProvider( _httpContextWrapper );
		}

		[Fact]
		public void HasValueShouldReturnTrueWhenValueExists()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.String:" + testKey;
			_httpContextWrapper.ResponseSetCookie( fullKey, "" );

			Assert.True( _valueProvider.HasValue<string>( testKey ) );
		}

		[Fact]
		public void HasValueShouldReturnFalseWhenValueDoNotExists()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.String:" + testKey;
			Assert.False( _valueProvider.HasValue<string>( fullKey ) );
		}

		[Fact]
		public void GetValueShouldReturnValue()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.Object:" + testKey;
			const string value = "12345";
			_httpContextWrapper.ResponseSetCookie(fullKey, value);
			var retVal = _valueProvider.GetValue<Object>( testKey, typeof( string ) );

			Assert.Equal( value, retVal );
		}

		[Fact]
		public void SetValueShouldSetValueInResponse()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.String:" + testKey;
			const string value = "12345";
			_valueProvider.SetValue<string>( testKey, value );
			
			Assert.True( _httpContextWrapper.RequestContainsCookie( fullKey) );
			Assert.Equal( value, _httpContextWrapper.RequestGetCookieValue( fullKey ) );
		}
	}
}