using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PseudoCQRS.Helpers;

namespace PseudoCQRS
{
	public class CommandHandlerProvider : ICommandHandlerProvider
	{
		private readonly ICommandHandlerFinder _commandHandlerFinder;
		private readonly IServiceProvider _serviceProvider;

		public CommandHandlerProvider( ICommandHandlerFinder commandHandlerFinder, IServiceProvider serviceProvider )
		{
			_commandHandlerFinder = commandHandlerFinder;
			_serviceProvider = serviceProvider;
		}

		public IAsyncCommandHandler<TCommand, TCommandResult> GetAsyncCommandHandler<TCommand, TCommandResult>()
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult
		{
			var handlerType = _commandHandlerFinder.FindAsyncHandlerForCommand<TCommand, TCommandResult>();
			if ( handlerType != null )
				return (IAsyncCommandHandler<TCommand, TCommandResult>)_serviceProvider.GetService( handlerType );

			var commandHandler = GetSynchronousCommandHandler<TCommand, TCommandResult>();
			if ( commandHandler == null )
				return null;
			return new AsyncCommandHandlerWrapper<TCommand, TCommandResult>( commandHandler );
		}

		private ICommandHandler<TCommand, TCommandResult> GetSynchronousCommandHandler<TCommand, TCommandResult>() where TCommand : ICommand<TCommandResult> where TCommandResult : CommandResult
		{
			var handlerType = _commandHandlerFinder.FindHandlerForCommand<TCommand, TCommandResult>();
			if ( handlerType == null )
				return null;
			return (ICommandHandler<TCommand, TCommandResult>)_serviceProvider.GetService( handlerType );
		}

		public ICommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>() where TCommand : ICommand<TCommandResult> where TCommandResult : CommandResult
		{
			var handlerType = _commandHandlerFinder.FindAsyncHandlerForCommand<TCommand, TCommandResult>();
			if (handlerType != null)
				throw new SynchronousAsyncCommandInvocationException<TCommand, TCommandResult>();

			return GetSynchronousCommandHandler<TCommand, TCommandResult>();
		}
	}

	public class AsyncCommandHandlerWrapper<TCommand,TCommandResult> : IAsyncCommandHandler<TCommand,TCommandResult> 
		where TCommandResult : CommandResult
		where TCommand : ICommand<TCommandResult>
	{
		private readonly ICommandHandler<TCommand, TCommandResult> _commandHandler;

		public ICommandHandler<TCommand, TCommandResult> CommandHandler => _commandHandler;

		public AsyncCommandHandlerWrapper( ICommandHandler<TCommand,TCommandResult> commandHandler )
		{
			_commandHandler = commandHandler;
		}

		public Task<TCommandResult> HandleAsync( TCommand command, CancellationToken cancellationToken ) => Task.FromResult( _commandHandler.Handle( command ) );
	}
}