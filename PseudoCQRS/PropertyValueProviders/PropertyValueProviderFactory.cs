using System;
using System.Collections.Generic;

namespace PseudoCQRS.PropertyValueProviders
{
	public class PropertyValueProviderFactory : IPropertyValueProviderFactory
	{
		private readonly IServiceProvider _serviceProvider;

		private readonly List<IPropertyValueProvider> _propertyValueProviders;

		public PropertyValueProviderFactory( IServiceProvider serviceProvider )
		{
			_serviceProvider = serviceProvider;
			var httpContext = _serviceProvider.GetService( typeof(IHttpContextWrapper)) as IHttpContextWrapper;
			_propertyValueProviders = new List<IPropertyValueProvider>()
			{
				new CookiePropertyValueProvider( httpContext ),
				new SessionPropertyValueProvider( httpContext ),
				new RouteDataPropertyValueProvider( httpContext ),
				new QueryStringPropertyValueProvider( httpContext ),
				new FormDataPropertyValueProvider( httpContext )
			};
		}

		public IEnumerable<IPropertyValueProvider> GetPropertyValueProviders()
		{
			return _propertyValueProviders;
		}

		public IPersistablePropertyValueProvider GetPersistablePropertyValueProvider( PersistanceLocation location )
		{
			IPersistablePropertyValueProvider result = null;
			switch ( location )
			{
				case PersistanceLocation.Cookie:
					result = new CookiePropertyValueProvider(_serviceProvider.GetService(typeof(IHttpContextWrapper)) as IHttpContextWrapper );
					break;
				case PersistanceLocation.Session:
					result = new SessionPropertyValueProvider(_serviceProvider.GetService(typeof(IHttpContextWrapper)) as IHttpContextWrapper );
					break;
			}

			return result;
		}
	}
}