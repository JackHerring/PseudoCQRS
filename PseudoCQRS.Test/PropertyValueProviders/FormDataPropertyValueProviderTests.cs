using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Http;
using PseudoCQRS.PropertyValueProviders;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Controllers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.PropertyValueProviders
{
	public class FormDataPropertyValueProviderTests
	{
		private readonly MockHttpContextWrapper _mockHttpContextWrapper;
		private readonly FormDataPropertyValueProvider _valueProvider;

		public FormDataPropertyValueProviderTests()
		{
			_mockHttpContextWrapper = new MockHttpContextWrapper();
			_valueProvider = new FormDataPropertyValueProvider( _mockHttpContextWrapper );
		}


		[Fact]
		public void HasValueShouldReturnTrueWhenFormContainsKey()
		{
			const string key = "id";
			const string value = "1324";
			_mockHttpContextWrapper.SetFormItem( key, value );

			Assert.True( _valueProvider.HasValue<string>( key ) );
		}

		[Fact]
		public void HasValueShouldReturnFalseWhenFormDoesNotContainKey()
		{
			const string key = "id";
			Assert.False( (bool)_valueProvider.HasValue<string>( key ) );
		}

		[Fact]
		public void HasValueShouldReturnTrueWhen_KeyNotFoundButKeyWithExistsFound()
		{
			// if key do not exist but there is another key with same {name}_Exists then it exist
			const string keyName = "MyTestKey";

			_mockHttpContextWrapper.SetFormItem(keyName + "_Exists", String.Empty);

			Assert.True( (bool)_valueProvider.HasValue<string>( keyName ) );
		}

		[Fact]
		public void GetValueShouldReturnValue()
		{
			const string key = "id";
			const string value = "1324";

			_mockHttpContextWrapper.SetFormItem(key, value);

			Assert.Equal( value, _valueProvider.GetValue<Object>( key, typeof( string ) ) );
		}

		[Fact]
		public void GetValueShouldReturnEmptyListWhenListIsNull()
		{
			const string key = "MyKey";
			_mockHttpContextWrapper.SetFormItem(key, null);

			var retVal = _valueProvider.GetValue<Object>( key, typeof( List<string> ) );
			Assert.NotNull( retVal );
		}
	}
}