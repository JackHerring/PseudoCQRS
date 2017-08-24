using System;
using System.Collections.Generic;
using Moq;
using PseudoCQRS.Checkers;
using Xunit;

namespace PseudoCQRS.Tests.Checkers
{
	public class CheckersExecuterTests
	{
		private readonly Mock<ICheckersFinder> _checkersFinderMock;
		private readonly CheckersExecuter _checkersExecuter;

		public CheckersExecuterTests()
		{
			_checkersFinderMock = new Mock<ICheckersFinder>();
			_checkersExecuter = new CheckersExecuter( _checkersFinderMock.Object );
		}

		private CheckResult ExecuteAuthorizationCheckers_ArrangeAndAct( bool containsErrror )
		{
			var mockedAuthorizationChecker = new Mock<IAuthorizationChecker>();
			mockedAuthorizationChecker
				.Setup( x => x.Check() )
				.Returns( new CheckResult
				{
					Message = containsErrror ? "Error" : String.Empty,
					ContainsError = containsErrror
				} );

			_checkersFinderMock
				.Setup( x => x.FindAuthorizationCheckers( It.IsAny<BlankSimpleTestCommand>() ) )
				.Returns( new List<IAuthorizationChecker>
				{
					mockedAuthorizationChecker.Object
				} );

			return _checkersExecuter.ExecuteAuthorizationCheckers( It.IsAny<BlankSimpleTestCommand>() );
		}

		[Fact]
		public void ExecuteAuthorizationCheckers_ShouldFailWhenCheckerFails()
		{
			var result = ExecuteAuthorizationCheckers_ArrangeAndAct( true );
			Assert.True( result.ContainsError );
		}

		[Fact]
		public void ExecuteAuthorizationChecker_ShouldPassWhenAllCheckersPass()
		{
			var result = ExecuteAuthorizationCheckers_ArrangeAndAct( false );
			Assert.False( result.ContainsError );
		}


		private CheckResult ExecuteAccessCheckers_ArrangeAndAct( bool containsError )
		{
			var mockedAccessChecker = new Mock<IAccessChecker>();
			mockedAccessChecker
				.Setup( x => x.Check( It.IsAny<string>(), It.IsAny<object>() ) )
				.Returns( new CheckResult
				{
					Message = containsError ? "Error" : String.Empty,
					ContainsError = containsError
				} );
			_checkersFinderMock
				.Setup( x => x.FindAccessCheckers( It.IsAny<BlankSimpleTestCommand>() ) )
				.Returns( new List<AccessCheckerAttributeDetails>
				{
					new AccessCheckerAttributeDetails
					{
						AccessChecker = mockedAccessChecker.Object,
						PropertyName = "TaskId"
					}
				} );

			return _checkersExecuter.ExecuteAccessCheckers( It.IsAny<BlankSimpleTestCommand>() );
		}

		[Fact]
		public void ExecuteAccessCheckers_ShouldFailWhenCheckerFails()
		{
			var result = ExecuteAccessCheckers_ArrangeAndAct( true );
			Assert.True( result.ContainsError );
		}

		[Fact]
		public void ExecuteAccessCheckers_ShouldPassWhenCheckerPass()
		{
			var result = ExecuteAccessCheckers_ArrangeAndAct( false );
			Assert.False( result.ContainsError );
		}

		private CheckResult ExecuteValidationCheckers_ArrangeAndAct( bool containsError )
		{
			var mockedValidationChecker = new Mock<IValidationChecker<BlankSimpleTestCommand>>();
			mockedValidationChecker
				.Setup( x => x.Check( It.IsAny<BlankSimpleTestCommand>() ) )
				.Returns( new CheckResult
				{
					Message = containsError ? "Error" : String.Empty,
					ContainsError = containsError
				} );
			_checkersFinderMock
				.Setup( x => x.FindValidationCheckers( It.IsAny<BlankSimpleTestCommand>()) )
				.Returns( new List<IValidationChecker<BlankSimpleTestCommand>>
				{
					mockedValidationChecker.Object
				} );

			return _checkersExecuter.ExecuteValidationCheckers( new BlankSimpleTestCommand() );
		}

		[Fact]
		public void ExecuteValidationCheckers_ShouldFailWhenCheckerFails()
		{
			var result = ExecuteValidationCheckers_ArrangeAndAct( true );
			Assert.True( result.ContainsError );
		}

		[Fact]
		public void ExecuteValidationCheckers_ShouldPassWhenCheckerPass()
		{
			var result = ExecuteValidationCheckers_ArrangeAndAct( false );
			Assert.False( result.ContainsError );
		}
	}
}