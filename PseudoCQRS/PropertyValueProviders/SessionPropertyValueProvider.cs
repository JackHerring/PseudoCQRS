using System;
using Microsoft.AspNetCore.Http;

namespace PseudoCQRS.PropertyValueProviders
{
	public class SessionPropertyValueProvider : BasePropertyValueProvider, IPropertyValueProvider, IPersistablePropertyValueProvider
	{
		private const string KeyFormat = "{0}:{1}";

		private readonly IHttpContextWrapper _httpContextAccessor;

		public SessionPropertyValueProvider(IHttpContextWrapper httpContextAccessor )
		{
			_httpContextAccessor = httpContextAccessor;
		}

		private string GetKey( Type objectType, string propertyName )
		{
			return string.Format( KeyFormat, objectType.FullName, propertyName );
		}

		public bool HasValue<T>( string key )
		{
			return _httpContextAccessor.ContainsSessionItem( GetKey( typeof( T ), key ) );
		}

		public object GetValue<T>( string key, Type propertyType )
		{
			return ConvertValue( _httpContextAccessor.GetSessionItem(  GetKey( typeof( T ), key ) ).ToString(), propertyType );
		}

		public void SetValue<T>( string key, object value )
		{
			_httpContextAccessor.SetSessionItem( GetKey( typeof( T ), key ) , value.ToString() );
		}
	}
}