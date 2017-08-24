using System;
using System.Collections.Generic;
using Moq;
using PseudoCQRS.Attributes;
using PseudoCQRS.PropertyValueProviders;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class ViewModelProviderArgumentsProviderTests
	{
		private Mock<IPropertyValueProviderFactory> _propertyValueProviderFactory;


		private Mock<IPropertyValueProviderFactory> StubGetPropertyValueProviders()
		{
			var propertyValueProviderFactory = new Mock<IPropertyValueProviderFactory>();
			var propertyValueProviders = new List<IPropertyValueProvider>
			{
				new TestPropertyValueProvider()
			};
			propertyValueProviderFactory.Setup( x => x.GetPropertyValueProviders() ).Returns( propertyValueProviders );

			return propertyValueProviderFactory;
		}

		[Fact]
		public void CanFillArgument()
		{
			var propertyValueProviderFactory = StubGetPropertyValueProviders();
			var viewModelProviderArgumentsProvider = new ViewModelProviderArgumentsProvider( propertyValueProviderFactory.Object );
			var arguments = viewModelProviderArgumentsProvider.GetArguments<TestArguments>();
			Assert.Equal( 1, arguments.LeagueId );
		}

		[Fact]
		public void Persist_WhenCalled_CallCookieSetValueCorrectlyForAllPropertysWithCookieAttribute()
		{
			const string cookie1Value = "Cookie 1 Value";
			const string cookie2Value = "Cookie 2 Value";

			var testClass = new TestClass
			{
				Cookie1 = cookie1Value,
				Cookie2 = cookie2Value
			};

			var cookieValueProvider = new Mock<IPersistablePropertyValueProvider>();
			var sessionValueProvider = new Mock<IPersistablePropertyValueProvider>();

			ArrangeAct_Persist( cookieValueProvider.Object, sessionValueProvider.Object, testClass );

			cookieValueProvider.Verify( x => x.SetValue<TestClass>( "Cookie1", cookie1Value ) );
			cookieValueProvider.Verify( x => x.SetValue<TestClass>( "Cookie2", cookie2Value ) );
		}

		[Fact]
		public void Persist_WhenCalled_CallCookieSetValueCorrectlyForAllPropertysWithSessionAttribute()
		{
			const string session1Value = "Session 1 Value";
			const string session2Value = "Session 2 Value";

			var testClass = new TestClass
			{
				Session1 = session1Value,
				Session2 = session2Value
			};

			var cookieValueProvider = new Mock<IPersistablePropertyValueProvider>();
			var sessionValueProvider = new Mock<IPersistablePropertyValueProvider>();

			ArrangeAct_Persist( cookieValueProvider.Object, sessionValueProvider.Object, testClass );

			sessionValueProvider.Verify( x => x.SetValue<TestClass>( "Session1", session1Value ) );
			sessionValueProvider.Verify( x => x.SetValue<TestClass>( "Session2", session2Value ) );
		}

		[Fact]
		public void Persist_WhenCalled_IfPropertyValueIsNullDoesNotCallSetValue()
		{
			const string session1Value = "Session 1 Value";

			var testClass = new TestClass
			{
				Session1 = session1Value,
				Session2 = null
			};

			var cookieValueProvider = new Mock<IPersistablePropertyValueProvider>();
			var sessionValueProvider = new Mock<IPersistablePropertyValueProvider>();

			ArrangeAct_Persist( cookieValueProvider.Object, sessionValueProvider.Object, testClass );

			sessionValueProvider.Verify( x => x.SetValue<TestClass>( "Session1", session1Value ) );
			sessionValueProvider.Verify( x => x.SetValue<TestClass>( "Session2", null ), Times.Never );
		}

		[Fact]
		public void Persist_WhenCalled_SmokeTest()
		{
			const string cookie1Value = "Cookie 1 Value";
			const string cookie2Value = "Cookie 2 Value";
			const string session1Value = "Session 1 Value";
			const string session2Value = "Session 2 Value";

			var propertyValueProvider = new Mock<IPropertyValueProvider>();
			propertyValueProvider.Setup( x => x.HasValue<TestClass>( It.IsAny<string>() ) ).Returns( true );
			MockPropertyValueProvider( propertyValueProvider, "Cookie1", cookie1Value );
			MockPropertyValueProvider( propertyValueProvider, "Cookie2", cookie2Value );
			MockPropertyValueProvider( propertyValueProvider, "Session1", session1Value );
			MockPropertyValueProvider( propertyValueProvider, "Session2", session2Value );


			var cookieValueProvider = new Mock<IPersistablePropertyValueProvider>();
			var sessionValueProvider = new Mock<IPersistablePropertyValueProvider>();

			_propertyValueProviderFactory = new Mock<IPropertyValueProviderFactory>();
			_propertyValueProviderFactory.Setup( x => x.GetPropertyValueProviders() ).Returns( new List<IPropertyValueProvider>
			{
				propertyValueProvider.Object
			} );

			_propertyValueProviderFactory.Setup( x => x.GetPersistablePropertyValueProvider( PersistanceLocation.Cookie ) ).Returns( cookieValueProvider.Object );
			_propertyValueProviderFactory.Setup( x => x.GetPersistablePropertyValueProvider( PersistanceLocation.Session ) ).Returns( sessionValueProvider.Object );

			var viewModelProviderArgumentsProvider = new ViewModelProviderArgumentsProvider( _propertyValueProviderFactory.Object );
			viewModelProviderArgumentsProvider.Persist<TestClass>();

			cookieValueProvider.Verify( x => x.SetValue<TestClass>( "Cookie1", cookie1Value ) );
			cookieValueProvider.Verify( x => x.SetValue<TestClass>( "Cookie2", cookie2Value ) );
			sessionValueProvider.Verify( x => x.SetValue<TestClass>( "Session1", session1Value ) );
			sessionValueProvider.Verify( x => x.SetValue<TestClass>( "Session2", session2Value ) );
		}

		private void ArrangeAct_Persist( IPersistablePropertyValueProvider cookieValueProvider, IPersistablePropertyValueProvider sessionValueProvider, TestClass testClass )
		{
			_propertyValueProviderFactory = new Mock<IPropertyValueProviderFactory>();
			_propertyValueProviderFactory.Setup( x => x.GetPersistablePropertyValueProvider( PersistanceLocation.Cookie ) ).Returns( cookieValueProvider );
			_propertyValueProviderFactory.Setup( x => x.GetPersistablePropertyValueProvider( PersistanceLocation.Session ) ).Returns( sessionValueProvider );

			var viewModelProviderArgumentsProvider = new ViewModelProviderArgumentsProvider( _propertyValueProviderFactory.Object );
			viewModelProviderArgumentsProvider.Persist( testClass );
		}

		private static void MockPropertyValueProvider( Mock<IPropertyValueProvider> propertyValueProviderMock, string key, string returnValue )
		{
			propertyValueProviderMock.Setup( x => x.GetValue<TestClass>( key, typeof( string ) ) ).Returns( returnValue );
		}

		private class TestClass
		{
			[Persist( PersistanceLocation.Cookie )]
			public String Cookie1 { get; set; }

			[Persist( PersistanceLocation.Cookie )]
			public String Cookie2 { get; set; }

			[Persist( PersistanceLocation.Session )]
			public String Session1 { get; set; }

			[Persist( PersistanceLocation.Session )]
			public String Session2 { get; set; }
		}
	}

	public class TestPropertyValueProvider : IPropertyValueProvider
	{
		private readonly Dictionary<string, string> values = new Dictionary<string, string>
		{
			{
				"LeagueId", "1"
			},
			{
				"Filter", "TestVal"
			}
		};


		public bool HasValue<T>( string key )
		{
			return values.ContainsKey( key );
		}

		public object GetValue<T>( string key, Type propertyType )
		{
			return Convert.ChangeType( values[ key ], propertyType );
		}
	}

	public class TestArguments
	{
		public int LeagueId { get; set; }
		public string Filter { get; set; }
	}
}