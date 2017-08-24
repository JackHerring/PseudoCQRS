using System;
using Moq;
using PseudoCQRS.Checkers;
using Xunit;

namespace PseudoCQRS.Tests.Checkers
{
	public class CheckersFinderTests
	{
		[ValidationCheck( typeof( TestValidationChecker ) )]
		[ValidationCheck( typeof( TestValidationChecker2 ) )]
		[AuthorizationCheck( typeof( TestAuthorizationChecker ) )]
		[AccessCheck( typeof( TestAccessChecker1 ), "TeamId" )]
		[AccessCheck( typeof( TestAccessChecker2 ), "UserId" )]
		[AccessCheck( typeof( TestAccessChecker3 ), "JobId" )]
		public class TestCommandForCheckersFinder {}

		public class TestValidationChecker : IValidationChecker<TestCommandForCheckersFinder>
		{
			public CheckResult Check( TestCommandForCheckersFinder instance )
			{
				return new CheckResult();
			}
		}

		public class TestValidationChecker2 : IValidationChecker<TestCommandForCheckersFinder>
		{
			public CheckResult Check( TestCommandForCheckersFinder instance )
			{
				return new CheckResult();
			}
		}

		public class TestAuthorizationChecker : IAuthorizationChecker
		{
			public CheckResult Check()
			{
				return new CheckResult();
			}
		}

		public class TestAccessChecker1 : IAccessChecker
		{
			public CheckResult Check( string propertyName, object instance )
			{
				return new CheckResult();
			}
		}

		public class TestAccessChecker2 : IAccessChecker
		{
			public CheckResult Check( string propertyName, object instance )
			{
				return new CheckResult();
			}
		}

		public class TestAccessChecker3 : IAccessChecker
		{
			public CheckResult Check( string propertyName, object instance )
			{
				return new CheckResult();
			}
		}

		private CheckersFinder _finder;

		public CheckersFinderTests()
		{
			var mockedServiceLocator = new Mock<IServiceProvider>();
			mockedServiceLocator
				.Setup( x => x.GetService( typeof( TestValidationChecker ) ) )
				.Returns( new TestValidationChecker() );
			mockedServiceLocator
				.Setup( x => x.GetService( typeof( TestValidationChecker2 ) ) )
				.Returns( new TestValidationChecker2() );
			mockedServiceLocator
				.Setup( x => x.GetService( typeof( TestAuthorizationChecker ) ) )
				.Returns( new TestAuthorizationChecker() );
			mockedServiceLocator
				.Setup( x => x.GetService( typeof( TestAccessChecker1 ) ) )
				.Returns( new TestAccessChecker1() );
			mockedServiceLocator
				.Setup( x => x.GetService( typeof( TestAccessChecker2 ) ) )
				.Returns( new TestAccessChecker2() );
			mockedServiceLocator
				.Setup( x => x.GetService( typeof( TestAccessChecker3 ) ) )
				.Returns( new TestAccessChecker3() );
			_finder = new CheckersFinder( mockedServiceLocator.Object );
		}

		[Fact]
		public void FindValidationCheckersCanReturnCheckers()
		{
			Assert.Equal( 2, _finder.FindValidationCheckers( new TestCommandForCheckersFinder() ).Count );
		}

		[Fact]
		public void FindAuthorizationCheckersCanReturnCheckers()
		{
			Assert.Equal( 1, _finder.FindAuthorizationCheckers( new TestCommandForCheckersFinder() ).Count );
		}

		[Fact]
		public void FindAccessCheckersCanReturnCheckers()
		{
			Assert.Equal( 3, _finder.FindAccessCheckers( new TestCommandForCheckersFinder() ).Count );
		}
	}
}