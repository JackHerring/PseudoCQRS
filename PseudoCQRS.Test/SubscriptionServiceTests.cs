using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using PseudoCQRS.Helpers;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class SubscriptionServiceTests
	{
		private Mock<IServiceProvider> _serviceProviderMock;
		private Mock<IEventSubscriberAssembliesProvider> _eventSubscriberAssembliesProviderMock;

		private Mock<IObjectLookupCache> _cache;
		private SubscriptionService _service;

		public SubscriptionServiceTests()
		{
			_serviceProviderMock = new Mock<IServiceProvider>();
			_eventSubscriberAssembliesProviderMock = new Mock<IEventSubscriberAssembliesProvider>();

			_eventSubscriberAssembliesProviderMock.Setup( x => x.GetEventSubscriberAssemblies() ).Returns( new List<Assembly>()
			{
				this.GetType().Assembly
			} );

			_cache = new Mock<IObjectLookupCache>();

			_service = new SubscriptionService( _cache.Object, _eventSubscriberAssembliesProviderMock.Object, _serviceProviderMock.Object );
		}

		// EXPECT: Call _cache.GetValue
		[Fact]
		public void ShouldCallCacheGetValue()
		{
			_service.GetSubscriptions<TestingEvent>();

			_cache.Verify( x => x.GetValue<IEnumerable<Type>>( typeof( TestingEvent ).FullName, null ) );
		}


		// WHEN: _cache.GetValue returns null
		// WHEN: GetImplementationsOf returns 0
		// EXPECT: Return a list with 0 items
		[Fact]
		public void ShouldReturn0ItemsWhenNoSubscribersFound()
		{
			var result = _service.GetSubscriptions<EventWithNoSubscribers>();
			Assert.Equal( 0, result.Count() );
		}

		// WHEN: _cache.GetValue returns null
		// WHEN: GetImplementationsOf returns 0
		// EXPECT: Not Call _cache.SetValue
		[Fact]
		public void ShouldNotCallCacheSetValueWhenGetImplementationsOfReturns0()
		{
			_service.GetSubscriptions<EventWithNoSubscribers>();

			_cache.Verify( x => x.SetValue<IEnumerable<Type>>( It.IsAny<string>(), It.IsAny<IEnumerable<Type>>() ), Times.Never );
		}


		// WHEN: _cache.GetValue returns null
		// WHEN: GetImplementationsOf return > 0
		// EXPECT: Call _cache.SetValue
		[Fact]
		public void ShouldCallCacheSetValueWhenGetImplementationsOfReturnsGreaterThan0()
		{
			_cache.Setup( x => x.GetValue<IEnumerable<Type>>( It.IsAny<string>(), null ) ).Returns( (IEnumerable<Type>)null );
			_serviceProviderMock.Setup( x => x.GetService( It.IsAny<Type>() ) ).Returns( new TestingEventSubscriber() );
			_service.GetSubscriptions<TestingEvent>();
			_cache.Verify( x => x.SetValue<IEnumerable<Type>>( It.Is<string>( s => s.Contains( typeof( TestingEvent ).FullName )), It.IsAny<IEnumerable<Type>>() ) );
		}

		// WHEN: _cache.GetValue is not null
		// EXPECT: Returned Value = 1
		[Fact]
		public void ShouldReturnSubsribersWhenCacheGetValueIsNotNull()
		{
			_cache
				.Setup( x => x.GetValue<IEnumerable<Type>>( It.IsAny<string>(), It.IsAny<IEnumerable<Type>>() ) )
				.Returns( new List<Type>
				{
					typeof( EventWithNoSubscribers )
				} );

			var result = _service.GetSubscriptions<EventWithNoSubscribers>();

			Assert.Equal( 1, result.Count() );
		}

		// WHEN: _cache.GetValue is null
		// WHEN: GetImplementationsOf return > 0
		// EXPECT: Returned Value = 1
		[Fact]
		public void ShouldReturnSubscribersWhenCacheGetValueIsNullButGetImplementationsOfGreaterThan0()
		{
			_cache.Setup(x => x.GetValue<IEnumerable<Type>>(It.IsAny<string>(), null)).Returns((IEnumerable<Type>)null);
			var result = _service.GetSubscriptions<TestingEvent>();
			Assert.Equal( 1, result.Count() );
		}
	}


	public class EventWithNoSubscribers {}

	public class TestingEvent {}

	public class TestingEventSubscriber : IEventSubscriber<TestingEvent>
	{
		public void Notify( TestingEvent @event )
		{
			throw new NotImplementedException();
		}

		public bool IsAsynchronous
		{
			get { throw new NotImplementedException(); }
		}
	}
}