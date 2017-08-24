using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PseudoCQRS.Checkers;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class ExecuteCommandBusTests : BaseCommandBusTests
	{
		protected override async Task<CommandResult> InvokeExecute()
		{
			return _bus.Execute( new BlankSimpleTestCommand() );
		}
	}

	public class ExecuteAsyncCommandBusTests : BaseCommandBusTests
	{
		protected override async Task<CommandResult> InvokeExecute()
		{
			return await _bus.ExecuteAsync( new BlankSimpleTestCommand(), CancellationToken.None );
		}
	}


	public abstract class BaseCommandBusTests
	{
		private readonly Mock<ICommandHandlerProvider> _commandHandlerProviderMock;
		private readonly Mock<IDbSessionManager> _dbSessionManagerMock;
		private readonly Mock<IPrerequisitesChecker> _prerequisitesCheckerMock;
		protected readonly CommandBus _bus;

		public BaseCommandBusTests()
		{
			_commandHandlerProviderMock = new Mock<ICommandHandlerProvider>();
			_dbSessionManagerMock = new Mock<IDbSessionManager>();
			_prerequisitesCheckerMock = new Mock<IPrerequisitesChecker>();

			_bus = new CommandBus( _commandHandlerProviderMock.Object, _dbSessionManagerMock.Object, _prerequisitesCheckerMock.Object );
		}

		private async Task<CommandResult> ExecuteArrangeAndAct(
			ICommandHandler<BlankSimpleTestCommand> getCommandHandlerRetVal = null,
			string commandPreHandleResult = "" )
		{
			_commandHandlerProviderMock
				.Setup( x => x.GetAsyncCommandHandler<BlankSimpleTestCommand, CommandResult>() )
				.Returns( getCommandHandlerRetVal == null ? null : new AsyncCommandHandlerWrapper<BlankSimpleTestCommand, CommandResult>( getCommandHandlerRetVal ) );

			_commandHandlerProviderMock
				.Setup( x => x.GetCommandHandler<BlankSimpleTestCommand, CommandResult>() )
				.Returns( getCommandHandlerRetVal );

			_prerequisitesCheckerMock
				.Setup( x => x.Check( It.IsAny<BlankSimpleTestCommand>() ) )
				.Returns( String.IsNullOrEmpty( commandPreHandleResult )
					? new CheckResult()
					: new CheckResult()
					{
						ContainsError = true,
						Message = commandPreHandleResult
					} );

			return await InvokeExecute();
		}

		protected abstract Task<CommandResult> InvokeExecute();


		[Fact]
		public async Task ShouldReturnFalseWhenHandlerNotFound()
		{
			var result = await ExecuteArrangeAndAct();

			Assert.True( result.ContainsError );
			Assert.False( string.IsNullOrEmpty(result.Message) );
		}


		[Fact]
		public async Task ShouldCallHandlerWhenNotNull()
		{
			var handler = new Mock<ICommandHandler<BlankSimpleTestCommand>>();
			await ExecuteArrangeAndAct( handler.Object );

			handler.Verify( x => x.Handle( It.IsAny<BlankSimpleTestCommand>() ) );
		}

		[Fact]
		public async Task ShouldNotCallHandler_WhenPreHandlerReturnsFalse()
		{
			var handler = new Mock<ICommandHandler<BlankSimpleTestCommand>>();
			await ExecuteArrangeAndAct( handler.Object, "Error" );

			handler.Verify( x => x.Handle(It.IsAny<BlankSimpleTestCommand>()), Times.Never );
		}

		[Fact]
		public async Task ShouldReturnFalseWhenPreHandlerReturnsFalse()
		{
			var handler = new Mock<ICommandHandler<BlankSimpleTestCommand>>();
			var result = await ExecuteArrangeAndAct( handler.Object, "Error" );

			Assert.True( result.ContainsError );
		}

		[DbTransaction]
		internal class CommandHandlerWithTransactionAttribute : ICommandHandler<BlankSimpleTestCommand>
		{
			public CommandResult Handle( BlankSimpleTestCommand command )
			{
				return new CommandResult();
			}
		}

		[Fact]
		public async Task ShouldOpenTransactionWhenDbTransactionAttributeIsApplied()
		{
			await ExecuteArrangeAndAct( new CommandHandlerWithTransactionAttribute() );
			_dbSessionManagerMock.Verify( x => x.OpenTransaction() );
		}

		[Fact]
		public async Task ShouldCommitTransactionWhenDbTransactionAttributeIsApplied()
		{
			await ExecuteArrangeAndAct( new CommandHandlerWithTransactionAttribute() );
			_dbSessionManagerMock.Verify( x => x.CommitTransaction() );
		}

		[Fact]
		public async Task Execute_HasTransactionAttributeAndOnError_RollbackTransaction()
		{
			_commandHandlerProviderMock
			.Setup( x => x.GetAsyncCommandHandler<BlankSimpleTestCommand, CommandResult>() )
			.Returns(new AsyncCommandHandlerWrapper<BlankSimpleTestCommand, CommandResult>(new CommandHandlerWithTransactionAttribute()));

			_commandHandlerProviderMock
				.Setup(x => x.GetCommandHandler<BlankSimpleTestCommand, CommandResult>())
				.Returns(new CommandHandlerWithTransactionAttribute());

			_prerequisitesCheckerMock
				.Setup( x => x.Check(It.IsAny<BlankSimpleTestCommand>()) )
				.Throws( new Exception( "Test Exception" ) );

			try
			{
				await _bus.ExecuteAsync( new BlankSimpleTestCommand(), CancellationToken.None );
			}
			catch ( Exception )
			{				
			}
			_dbSessionManagerMock.Verify( x => x.RollbackTransaction() );


		}

		[DbTransaction]
		internal class CommandHandlerThatThrowsException : ICommandHandler<BlankSimpleTestCommand>
		{
			public CommandResult Handle( BlankSimpleTestCommand command )
			{
				throw new NotImplementedException();
			}
		}

		[Fact]
		public async Task Execute_AHandlerThatThrowsAnException_RollsbackTransaction()
		{
			await Assert.ThrowsAsync<NotImplementedException>( () => ExecuteArrangeAndAct( new CommandHandlerThatThrowsException() ) );

			_dbSessionManagerMock.Verify( x => x.RollbackTransaction() );
		}
	}
}