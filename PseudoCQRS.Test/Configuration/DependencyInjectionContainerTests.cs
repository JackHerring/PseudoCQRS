using System;
using System.Collections.Generic;
using PseudoCQRS.Configuration;
using Xunit;

namespace PseudoCQRS.Tests.Configuration
{
	public class DependencyInjectionContainerTests : IDisposable
	{
		private readonly DependencyInjectionContainer _container;

		public DependencyInjectionContainerTests()
		{
			_container = new DependencyInjectionContainer();
		}

		public interface IInterface {}

		public class Implementation : IInterface {}

		public interface IInterface2 {}

		public class Implementation2 : IInterface2 {}

		public class ClassWithDependencies
		{
			public ClassWithDependencies( IInterface iInterface, IInterface2 interface2 )
			{
				Interface = iInterface;
				Interface2 = interface2;
			}

			public IInterface Interface { get; private set; }

			public IInterface2 Interface2 { get; private set; }
		}

		public class ClassWithNoConstructor {}

		private void AssertValidImplementationInstance( Type implementationType, object instance )
		{
			Assert.IsAssignableFrom( implementationType, instance );
			Assert.NotNull( instance );
		}

		[Fact]
		public void CanRegisterAndResolveADependency()
		{
			_container.Register<IInterface, Implementation>();
			var implementation = _container.Resolve<IInterface>();

			AssertValidImplementationInstance( typeof( Implementation ), implementation );
		}

		[Fact]
		public void CanRegisterMultipleDependencies()
		{
			var implementationType = typeof( Implementation );
			var implementation2Type = typeof( Implementation2 );
			_container.Register( new Dictionary<Type, Type>
			{
				{ typeof( IInterface ), implementationType },
				{ typeof( IInterface2 ), implementation2Type }
			} );

			AssertValidImplementationInstance( implementationType, _container.Resolve<IInterface>() );
			AssertValidImplementationInstance( implementation2Type, _container.Resolve<IInterface2>() );
		}

		[Fact]
		public void CanResolveATypeWithDependencies()
		{
			_container.Register<IInterface, Implementation>();
			_container.Register<IInterface2, Implementation2>();

			var classWithDependencies = _container.Resolve<ClassWithDependencies>();
			Assert.NotNull( classWithDependencies );
		}

		[Fact]
		public void CanResolveATypeWithNoConstructors()
		{
			_container.Register<IInterface, Implementation>();
			var classWithNoConstructor = _container.Resolve<ClassWithNoConstructor>();
			Assert.NotNull( classWithNoConstructor );
		}

		[Fact]
		public void Resolve_AUnregisteredInterfaceType_ThrowAnException()
		{
			Assert.Throws<Exception>( () => _container.Resolve<IInterface>() );
		}

		[Fact]
		public void WhenResolveIsCalledForSecondTime_ReturnsSameInstance()
		{
			_container.Register<IInterface, Implementation>();
			_container.Register<IInterface2, Implementation2>();

			var firstInstance = _container.Resolve<ClassWithDependencies>();
			var secondInstance = _container.Resolve<ClassWithDependencies>();
			Assert.Equal( firstInstance, secondInstance );
		}

		public void Dispose() => _container.ClearAll();
	}
}