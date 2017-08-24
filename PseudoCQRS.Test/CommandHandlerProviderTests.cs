using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PseudoCQRS.Helpers;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class CommandHandlerProviderTests
	{
		private readonly Mock<ICommandHandlerFinder> _commandHandlerFinderMock;
		private readonly Mock<IServiceProvider> _serviceProvider;
		private readonly CommandHandlerProvider _provider;

		public CommandHandlerProviderTests()
		{
			_commandHandlerFinderMock = new Mock<ICommandHandlerFinder>();
			_serviceProvider = new Mock<IServiceProvider>();
			_provider = new CommandHandlerProvider( _commandHandlerFinderMock.Object, _serviceProvider.Object );
		}

		private void SetupCommandHandlerFinderMock( Expression<Func<ICommandHandlerFinder, Type>> method, Type returnType = null )
		{
			_commandHandlerFinderMock
				.Setup( method )
				.Returns( returnType );
		}

		private void SetupServiceProviderMock<T>(T service)
		{
			_serviceProvider
				.Setup( x => x.GetService( typeof( T ) ) )
				.Returns( service );
		}

		[Fact]
		public void WhenGetAsyncCommandHandlerCalled_AndAsyncCommandHandlerIsFoundByFinder_DoesReturnIAsyncCommandHandlerForThatCommand()
		{
			SetupCommandHandlerFinderMock( x => x.FindAsyncHandlerForCommand<AsyncDummyCommand, AsyncDummyCommandResult>(), typeof( AsyncDummyCommandHandler ) );
			SetupServiceProviderMock( new AsyncDummyCommandHandler() );

			var handler = _provider.GetAsyncCommandHandler<AsyncDummyCommand, AsyncDummyCommandResult>();
			_commandHandlerFinderMock.Verify( x => x.FindAsyncHandlerForCommand<AsyncDummyCommand, AsyncDummyCommandResult>() );
			Assert.NotNull( handler );
			Assert.IsType<AsyncDummyCommandHandler>( handler );
		}

		[Fact]
		public void WhenGetSyncCommandHandlerCalled_AndSyncCommandHandlerIsFoundByFinder_DoesReturnICommandHandlerForThatCommand()
		{
			SetupCommandHandlerFinderMock(x => x.FindAsyncHandlerForCommand<DummyCommand, DummyCommandResult>() );
			SetupCommandHandlerFinderMock( x => x.FindHandlerForCommand<DummyCommand, DummyCommandResult>(), typeof( DummyCommandHandler ) );
			SetupServiceProviderMock(new DummyCommandHandler());
			
			var handler = _provider.GetCommandHandler<DummyCommand, DummyCommandResult>();
			_commandHandlerFinderMock.Verify( x => x.FindAsyncHandlerForCommand<DummyCommand, DummyCommandResult>() );
			_commandHandlerFinderMock.Verify( x => x.FindHandlerForCommand<DummyCommand, DummyCommandResult>() );
			Assert.NotNull( handler );
			Assert.IsType<DummyCommandHandler>( handler );
		}

		[Fact]
		public void WhenGetSyncCalled_ButAsynchronousImplementationFound_ThrowsException()
		{
			SetupCommandHandlerFinderMock( x => x.FindAsyncHandlerForCommand<DummyCommand, DummyCommandResult>(), typeof( AsyncDummyCommandHandler ) );

			Assert.Throws<SynchronousAsyncCommandInvocationException<DummyCommand, DummyCommandResult>>( () => _provider.GetCommandHandler<DummyCommand, DummyCommandResult>() );
		}

		[Fact]
		public void WhenGetAsyncCalled_ButSyncHandlerFound_DoesReturnAsyncWrapper()
		{
			SetupCommandHandlerFinderMock(x => x.FindAsyncHandlerForCommand<DummyCommand, DummyCommandResult>());
			SetupCommandHandlerFinderMock( x => x.FindHandlerForCommand<DummyCommand, DummyCommandResult>(), typeof( DummyCommandHandler ) );
			SetupServiceProviderMock( new DummyCommandHandler() );

			var handler = _provider.GetAsyncCommandHandler<DummyCommand, DummyCommandResult>();
			var wrapper = handler as AsyncCommandHandlerWrapper<DummyCommand, DummyCommandResult>;
			Assert.NotNull( wrapper );
		}

		[Fact]
		public void WhenNoCommandHandlersFound_ReturnsNull_AndDoesntThrowAnException()
		{
			SetupCommandHandlerFinderMock(x => x.FindAsyncHandlerForCommand<DummyCommand, DummyCommandResult>());
			SetupCommandHandlerFinderMock(x => x.FindAsyncHandlerForCommand<DummyCommand, DummyCommandResult>());
			
			Assert.Null(_provider.GetCommandHandler<DummyCommand, DummyCommandResult>());
			Assert.Null(_provider.GetAsyncCommandHandler<AsyncDummyCommand, AsyncDummyCommandResult>());
		}

		internal class AsyncDummyCommand : ICommand<AsyncDummyCommandResult>
		{
		}

		internal class AsyncDummyCommandResult : CommandResult
		{
		}

		internal class AsyncDummyCommandHandler : IAsyncCommandHandler<AsyncDummyCommand, AsyncDummyCommandResult>
		{
			public Task<AsyncDummyCommandResult> HandleAsync( AsyncDummyCommand command, CancellationToken cancellationToken )
			{
				return Task.FromResult( new AsyncDummyCommandResult() );
			}
		}

		internal class DummyCommand : ICommand<DummyCommandResult>
		{
		}

		internal class DummyCommandResult : CommandResult
		{
		}

		internal class DummyCommandHandler : ICommandHandler<DummyCommand, DummyCommandResult>
		{
			public DummyCommandResult Handle( DummyCommand command )
			{
				return new DummyCommandResult();
			}
		}
	}
}