using System;
using System.Linq;
using Moq;
using PseudoCQRS.PropertyValueProviders;
using Xunit;

namespace PseudoCQRS.Tests.PropertyValueProviders
{
	public class PropertyValueProviderFactoryTests
	{
		private readonly Mock<IServiceProvider> _serviceProvider;

		public PropertyValueProviderFactoryTests() => _serviceProvider = new Mock<IServiceProvider>();

		[Fact]
		public void ShouldReturnAllPropertyValueProviders()
		{
			var factory = new PropertyValueProviderFactory(_serviceProvider.Object);
			Assert.Equal( 5, factory.GetPropertyValueProviders().Count() );
		}

		[Fact]
		public void GetPersistablePropertyValueProviders_PersistanceLocationIsCookie_ReturnsCookiePersistablePropertyValueProvider()
		{
			var factory = new PropertyValueProviderFactory(_serviceProvider.Object);
			Assert.IsType<CookiePropertyValueProvider>( factory.GetPersistablePropertyValueProvider( PersistanceLocation.Cookie ) );
		}

		[Fact]
		public void GetPersistablePropertyValueProvider_PersistanceLocationIsSession_ReturnsSessionPropertyValueProvider()
		{
			var factory = new PropertyValueProviderFactory(_serviceProvider.Object);
			Assert.IsType<SessionPropertyValueProvider>( factory.GetPersistablePropertyValueProvider( PersistanceLocation.Session ) );
		}
	}
}