using System;
using System.Collections.Generic;

namespace PseudoCQRS.Configuration
{
	public class DependencyInjectionServiceLocator : IServiceProvider
	{
		private readonly IDependencyInjectionContainer _container;

		public DependencyInjectionServiceLocator( IDependencyInjectionContainer container )
		{
			_container = container;
		}

		public object GetService( Type serviceType )
		{
			return _container.Resolve(serviceType);
		}
	}
}