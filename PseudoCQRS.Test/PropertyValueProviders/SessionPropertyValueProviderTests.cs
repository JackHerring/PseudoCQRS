using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using PseudoCQRS.PropertyValueProviders;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.PropertyValueProviders
{
	public class SessionPropertyValueProviderTests
	{
		private readonly IHttpContextWrapper _httpContextWrapper;
		private readonly SessionPropertyValueProvider _valueProvider;

		public SessionPropertyValueProviderTests()
		{
			_httpContextWrapper = new MockHttpContextWrapper();
			_valueProvider = new SessionPropertyValueProvider( _httpContextWrapper );
		}

		[Fact]
		public void HasValueShouldReturnTrueWhenValueExists()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.String:" + testKey;
			_httpContextWrapper.SetSessionItem( fullKey, "" );

			Assert.True( _valueProvider.HasValue<string>( testKey ) );
		}

		[Fact]
		public void HasValueShouldReturnFalseWhenValueDoNotExists()
		{
			const string testKey = "MyTestKey";
			Assert.False( _valueProvider.HasValue<string>( testKey ) );
		}

		[Fact]
		public void GetValueShouldReturnValue()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.Object:" + testKey;
			const string value = "12345";
			_httpContextWrapper.SetSessionItem(fullKey, value);

			var retVal = _valueProvider.GetValue<Object>( testKey, typeof(string) );

			Assert.Equal( value, retVal );
		}

		[Fact]
		public void SetValueShouldSetValue()
		{
			const string testKey = "MyTestKey";
			const string fullKey = "System.String:" + testKey;
			const string value = "12345";
			_valueProvider.SetValue<string>( testKey, value );
			
			
			Assert.True(_httpContextWrapper.GetSessionItem(fullKey) == value );
		}
	}
}