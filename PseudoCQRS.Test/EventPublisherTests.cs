using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Moq;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class EventPublisherTests
	{
		private readonly Mock<ISubscriptionService> _subscriptionServiceMock;
		private readonly Mock<IDbSessionManager> _dbSessionManagerMock;
		private readonly EventPublisher _publisher;
		private Mock<ILogger> _loggerMock;

		private static StringBuilder _sharedStringBuilder = new StringBuilder();


		// ReSharper disable once MemberCanBePrivate.Global
		public class TestingEvent {}

		public EventPublisherTests()
		{
			_subscriptionServiceMock = new Mock<ISubscriptionService>();
			_dbSessionManagerMock = new Mock<IDbSessionManager>();
			_loggerMock = new Mock<ILogger>();

			_publisher = new EventPublisher( _subscriptionServiceMock.Object, _dbSessionManagerMock.Object, _loggerMock.Object );
		}

		// EXPECT: Call _subscriptionService.GetSubscriptions
		[Fact]
		public void ShouldCallSubscriptionServiceGetSubscriptions()
		{
			_subscriptionServiceMock
				.Setup( x => x.GetSubscriptions<TestingEvent>() )
				.Returns( new List<IEventSubscriber<TestingEvent>>() );

			_publisher.Publish<TestingEvent>( new TestingEvent() );

			_subscriptionServiceMock.Verify( x => x.GetSubscriptions<TestingEvent>() );
		}

		// WHEN: _subscriptionService.GetSubscriptions returns > 0
		// EXPECT: Call Apply for each subscriber found.
		[Fact]
		public void ShouldCallApplyMethodOfEachSubscriberFoundWhenSubscriptionServiceGetSubscriptionsReturnsGreaterThan0()
		{
			var mockedEventSubscriber1 = new Mock<IEventSubscriber<TestingEvent>>();
			var mockedEventSubscriber2 = new Mock<IEventSubscriber<TestingEvent>>();

			_subscriptionServiceMock
				.Setup( x => x.GetSubscriptions<TestingEvent>() )
				.Returns( new List<IEventSubscriber<TestingEvent>>
				{
					mockedEventSubscriber1.Object,
					mockedEventSubscriber2.Object
				} );

			_publisher.Publish<TestingEvent>( new TestingEvent() );

			mockedEventSubscriber1.Verify( x => x.Notify( It.IsAny<TestingEvent>()) );
			mockedEventSubscriber2.Verify( x => x.Notify( It.IsAny<TestingEvent>()) );
		}


		public class AsyncSubscriber : IEventSubscriber<TestingEvent>
		{
			public void Notify( TestingEvent @event )
			{
				Thread.Sleep( 1000 );
				_sharedStringBuilder.Append( "Executing Subscriber, " );
			}

			public bool IsAsynchronous
			{
				get { return true; }
			}
		}

		public class AsyncSubscriber2 : IEventSubscriber<TestingEvent>
		{
			public void Notify( TestingEvent @event ) {}

			public bool IsAsynchronous
			{
				get { return true; }
			}
		}


		[Fact]
		public void CanNotifyAsynchronousSubscriber()
		{
			_sharedStringBuilder = new StringBuilder();
			_subscriptionServiceMock
				.Setup( x => x.GetSubscriptions<TestingEvent>() )
				.Returns( new List<IEventSubscriber<TestingEvent>>
				{
					new AsyncSubscriber()
				} );

			_sharedStringBuilder.Append( "Started, " );
			_publisher.Publish<TestingEvent>( new TestingEvent() );
			_sharedStringBuilder.Append( "Ended, " );
			Thread.Sleep( 2000 );
			//Console.WriteLine( _sharedStringBuilder.ToString() );
			Assert.Equal( "Started, Ended, Executing Subscriber, ", _sharedStringBuilder.ToString() );
		}

		[Fact]
		public void ShouldRunSyncWhenForced()
		{
			_sharedStringBuilder = new StringBuilder();
			_subscriptionServiceMock
				.Setup( x => x.GetSubscriptions<TestingEvent>() )
				.Returns( new List<IEventSubscriber<TestingEvent>>
				{
					new AsyncSubscriber()
				} );

			_sharedStringBuilder.Append( "Started, " );
			_publisher.PublishSynchronously<TestingEvent>( new TestingEvent() );
			_sharedStringBuilder.Append( "Ended, " );
			Thread.Sleep( 2000 );
			Assert.Equal( "Started, Executing Subscriber, Ended, ", _sharedStringBuilder.ToString() );
		}

		[Fact]
		public void ShouldCloseDbSessionAfterCallingEachAsyncSubscriber()
		{
			_subscriptionServiceMock
				.Setup( x => x.GetSubscriptions<TestingEvent>() )
				.Returns( new List<IEventSubscriber<TestingEvent>>
				{
					new AsyncSubscriber(),
					new AsyncSubscriber2()
				} );
			_publisher.Publish( new TestingEvent() );
			Thread.Sleep( 2000 );
			Console.WriteLine( _sharedStringBuilder.ToString() );
			_dbSessionManagerMock.Verify( x => x.CloseSession(), Times.Exactly( 2 ));
		}
	}
}