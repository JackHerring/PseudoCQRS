using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class CommandHandlerFinderTests
	{
		private readonly CommandHandlerFinder _finder;
		private readonly Mock<IAssemblyListProvider> _assembliesListProvider;

		public CommandHandlerFinderTests()
		{
			_assembliesListProvider = new Mock<IAssemblyListProvider>();
			_assembliesListProvider
				.Setup( x => x.GetAssemblies() )
				.Returns( new Assembly[]
				{
					this.GetType().Assembly
				} );

			_finder = new CommandHandlerFinder( _assembliesListProvider.Object );
		}

		[Fact]
		public void ShouldReturnTypeForSyncronousHandlerWhenFound()
		{
			Type handlerType = _finder.FindHandlerForCommand<SyncTestCommand, CommandResult>();
			Assert.NotNull( handlerType );
			Assert.Equal(nameof(SyncTestCommandHandler), handlerType.Name);
		}

		[Fact]
		public void ShouldReturnTypeForAsyncronousHandlerWhenFound()
		{
			Type handlerType = _finder.FindAsyncHandlerForCommand<ASyncTestCommand, CommandResult>();
			Assert.NotNull(handlerType);
			Assert.Equal(nameof(ASyncTestCommandHandler), handlerType.Name);
		}

		[Fact]
		public void ShouldFindCommandHandlerFromAssembliesListProvider()
		{
			CommandHandlerFinder.ClearCache();
			_finder.FindHandlerForCommand<SyncTestCommand, CommandResult>();
			_assembliesListProvider
				.Verify( x => x.GetAssemblies() );
		}
	}

	internal class SyncTestCommand : ICommand { }

	internal class SyncTestCommandHandler : ICommandHandler<SyncTestCommand>
	{
		public CommandResult Handle(SyncTestCommand command)
		{
			return new CommandResult();
		}
	}

	internal class ASyncTestCommand : ICommand { }

	internal class ASyncTestCommandHandler : IAsyncCommandHandler<ASyncTestCommand, CommandResult>
	{
		public Task<CommandResult> HandleAsync(ASyncTestCommand command, CancellationToken cancellationToken )
		{
			return Task.FromResult( new CommandResult() );
		}
	}
}