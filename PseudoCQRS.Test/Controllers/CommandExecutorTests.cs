using System.Threading;
using System.Threading.Tasks;
using Moq;
using PseudoCQRS.Controllers;
using PseudoCQRS.Tests.Controllers.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class CommandExecutorTests
	{
		private readonly Mock<ICommandBus> _commandBusMock;
		private readonly Mock<IMessageManager> _messageManagerMock;
		private readonly CommandExecutor _commandExecutor;

		public CommandExecutorTests()
		{
			_commandBusMock = new Mock<ICommandBus>();
			_messageManagerMock = new Mock<IMessageManager>();

			_commandExecutor = new CommandExecutor( _commandBusMock.Object, _messageManagerMock.Object );
		}

		[Fact]
		public async Task ErrorMessageSetWhenCommandResultHasError()
		{
			_commandBusMock.Setup(
					x => x.ExecuteAsync( It.IsAny<DummyExecuteCommand>(), It.IsAny<CancellationToken>() ) )
					.Returns(Task.FromResult(new CommandResult()
												{
													ContainsError = true,
													Message = "Error"
												}) );

			await _commandExecutor.ExecuteCommandAsync<DummyExecuteCommand, CommandResult>( new DummyExecuteCommand(), CancellationToken.None );

			_messageManagerMock.Verify( x => x.SetErrorMessage( "Error" ) );
		}


		[Fact]
		public async Task ShouldSetSuccessMessageInMessageManager_WhenNoErrorReturnedFromCommandBus()
		{
			_commandBusMock.Setup(
					x => x.ExecuteAsync(It.IsAny<DummyExecuteCommand>(), It.IsAny<CancellationToken>()))
					.Returns( Task.FromResult(new CommandResult()
													{
														ContainsError = false,
														Message = "Success"
											}) );

			await _commandExecutor.ExecuteCommandAsync<DummyExecuteCommand, CommandResult>( new DummyExecuteCommand(), CancellationToken.None );

			_messageManagerMock.Verify( x => x.SetSuccessMessage( "Success" ) );
		}
	}
}