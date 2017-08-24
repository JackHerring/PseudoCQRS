using Moq;
using PseudoCQRS.Checkers;
using Xunit;

namespace PseudoCQRS.Tests.Checkers
{
	public class PreRequisitesCheckerTests
	{
		private readonly Mock<ICheckersExecuter> _checkersExecuterMock;
		private readonly PrerequisitesChecker _prerequisitesChecker;

		public PreRequisitesCheckerTests()
		{
			_checkersExecuterMock = new Mock<ICheckersExecuter>();
			_prerequisitesChecker = new PrerequisitesChecker( _checkersExecuterMock.Object );
		}


		private CheckResult ArrangeAndAct(
			bool authorizationCheckContainsError,
			bool accessCheckContainsError,
			bool validationCheckContainsError
			)
		{
			var errorResult = new CheckResult()
			{
				ContainsError = true,
				Message = "Error"
			};

			var successResult = new CheckResult();

			_checkersExecuterMock
				.Setup( x => x.ExecuteAuthorizationCheckers( It.IsAny<BlankSimpleTestCommand>() ) )
				.Returns( authorizationCheckContainsError ? errorResult : successResult );

			_checkersExecuterMock
				.Setup( x => x.ExecuteAccessCheckers(It.IsAny<BlankSimpleTestCommand>()) )
				.Returns( accessCheckContainsError ? errorResult : successResult );

			_checkersExecuterMock
				.Setup( x => x.ExecuteValidationCheckers(It.IsAny<BlankSimpleTestCommand>()) )
				.Returns( validationCheckContainsError ? errorResult : successResult );

			return _prerequisitesChecker.Check( new BlankSimpleTestCommand() );
		}

		[Fact]
		public void ShouldFailWhenAuthorizationCheckFails()
		{
			var result = ArrangeAndAct( true, false, false );
			Assert.True( result.ContainsError );
		}

		[Fact]
		public void ShouldFailWhenAccessCheckFails()
		{
			var result = ArrangeAndAct( false, true, false );
			Assert.True( result.ContainsError );
		}

		[Fact]
		public void ShouldFailWhenValidationCheckFails()
		{
			var result = ArrangeAndAct( false, false, true );
			Assert.True( result.ContainsError );
		}

		[Fact]
		public void ShouldBeSuccessfulWhenAllChecksPass()
		{
			var result = ArrangeAndAct( false, false, false );
			Assert.False( result.ContainsError );
		}
	}
}