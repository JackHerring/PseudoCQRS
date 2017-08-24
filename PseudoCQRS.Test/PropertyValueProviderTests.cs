using System;
using System.Collections.Generic;
using PseudoCQRS.PropertyValueProviders;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class PropertyValueProviderTests
	{
		private readonly TestPropertyValueConverter _converter;

		public PropertyValueProviderTests()
		{
			_converter = new TestPropertyValueConverter();
		}

		[Fact]
		public void CanConvertStringToInt()
		{
			var result = _converter.GetValue( "1", typeof( Int32 ) );
			Assert.Equal( 1, result );
		}

		[Fact]
		public void CanConvertCommaSeparatedListToList()
		{
			var result = _converter.GetValue( "1,2,3", typeof( List<Int32> ) );
			Assert.Equal( 3, ( (List<Int32>)result ).Count );
		}

		[Theory]
		[InlineData("10/11/2012")]
		[InlineData("10 Nov 2012")]
		public void CanConvertDateFormats( string format )
		{
			var result = (DateTime)_converter.GetValue( format, typeof( DateTime ) );
			Assert.True( new DateTime( 2012, 11, 10 ) == result );
		}

		[Theory]
		[InlineData("15:14")]
		[InlineData("03:14 pm")]
		[InlineData("3:14 pm")]
		public void CanConverTimeFormats( string format )
		{
			var result = (DateTime)_converter.GetValue( format, typeof( DateTime ) );
			Assert.True( result.Hour == 15 && result.Minute == 14 && result.Second == 0 );
		}

		[Theory]
		[InlineData("10/11/2012 03:14 pm")]
		[InlineData("10/11/2012 3:14 pm")]
		[InlineData("10/11/2012 15:14:00")]
		public void CanConvertDateWithTimeFormats( string format )
		{
			var result = (DateTime)_converter.GetValue( format, typeof( DateTime ) );
			Assert.True( new DateTime( 2012, 11, 10, 15, 14, 0 ) == result );
		}

		[Fact]
		public void CanConvertStringToNullableDateTime()
		{
			var result = (DateTime?)_converter.GetValue( "30/11/2013", typeof( DateTime? ) );
			Assert.True( result.HasValue );
			Assert.Equal( result, new DateTime( 2013, 11, 30 ) );
		}


		[Fact]
		public void ShouldReturnNullWhenTypeIsDateTimeButFormateIsNotRecognized()
		{
			Assert.Null( _converter.GetValue( "10-11-2012", typeof( DateTime ) ) );
		}
	}

	internal class TestPropertyValueConverter : BasePropertyValueProvider
	{
		public object GetValue( string value, Type propertyType )
		{
			return ConvertValue( value, propertyType );
		}
	}
}