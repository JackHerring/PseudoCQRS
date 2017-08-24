using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PseudoCQRS.Checkers;
using PseudoCQRS.ExtensionMethods;

namespace PseudoCQRS
{
	public class CommandBus : ICommandBus
	{
		private readonly ICommandHandlerProvider _commandHandlerProvider;
		private readonly IDbSessionManager _dbSessionManager;
		private readonly IPrerequisitesChecker _prerequisitesChecker;

		public CommandBus(
			ICommandHandlerProvider commandHandlerProvider,
			IDbSessionManager dbSessionManager,
			IPrerequisitesChecker prerequisitesChecker )
		{
			_commandHandlerProvider = commandHandlerProvider;
			_dbSessionManager = dbSessionManager;
			_prerequisitesChecker = prerequisitesChecker;
		}

		private TCommandResult InternalExecute<TCommand, TCommandResult>( TCommand command )
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult, new()
		{
			var result = CreateDefaultCommandResult<TCommand, TCommandResult>(command);

			var handler = _commandHandlerProvider.GetCommandHandler<TCommand, TCommandResult>();

			if (handler != null)
			{
				var hasTransactionAttribute = handler.HasTransactionAttribute();

				if (hasTransactionAttribute)
					_dbSessionManager.OpenTransaction();

				try
				{
					var checkResult = _prerequisitesChecker.Check(command);
					if ( !checkResult.ContainsError )
						result = handler.Handle( command );
					else
						result.Message = checkResult.Message;

					if (hasTransactionAttribute)
						_dbSessionManager.CommitTransaction();
				}
				catch (Exception)
				{
					if (hasTransactionAttribute)
						_dbSessionManager.RollbackTransaction();
					throw;
				}
			}

			return result;
		}

		private async Task<TCommandResult> InternalExecuteAsync<TCommand, TCommandResult>( TCommand command, CancellationToken cancellationToken )
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult, new()
		{
			var result = CreateDefaultCommandResult<TCommand, TCommandResult>( command );

			var handler = _commandHandlerProvider.GetAsyncCommandHandler<TCommand, TCommandResult>();

			if ( handler != null )
			{
				var hasTransactionAttribute = handler.HasTransactionAttribute();

				if ( hasTransactionAttribute )
					_dbSessionManager.OpenTransaction();

				try
				{
					var checkResult = _prerequisitesChecker.Check( command );
					if ( !checkResult.ContainsError )
						result = await handler.HandleAsync( command, cancellationToken );
					else
						result.Message = checkResult.Message;

					if ( hasTransactionAttribute )
						_dbSessionManager.CommitTransaction();
				}
				catch ( Exception )
				{
					if ( hasTransactionAttribute )
						_dbSessionManager.RollbackTransaction();
					throw;
				}
			}

			return result;
		}

		private TCommandResult CreateDefaultCommandResult<TCommand, TCommandResult>( TCommand command ) where TCommand : ICommand<TCommandResult> where TCommandResult : CommandResult, new()
		{
			var result = new TCommandResult
			{
				ContainsError = true,
				Message = $"Handler not found for command {command.GetType().Name}"
			};
			return result;
		}

		public Task<TCommandResult> ExecuteAsync<TCommandResult>( ICommand<TCommandResult> command, CancellationToken cancellationToken = default(CancellationToken) )
			where TCommandResult : CommandResult, new()
		{
			return (Task<TCommandResult>)GetType().GetMethod( nameof(InternalExecuteAsync), BindingFlags.NonPublic | BindingFlags.Instance ).MakeGenericMethod( command.GetType(), typeof( TCommandResult ) ).Invoke( this, new object[]
			{
				command,
				cancellationToken
			} );
		}

		public TCommandResult Execute<TCommandResult>( ICommand<TCommandResult> command ) where TCommandResult : CommandResult, new()
		{
			try
			{
				return (TCommandResult)GetType().GetMethod( nameof(InternalExecute), BindingFlags.NonPublic | BindingFlags.Instance ).MakeGenericMethod( command.GetType(), typeof( TCommandResult ) ).Invoke( this, new object[]
				{
					command
				} );
			}
			catch ( TargetInvocationException ex )
			{
				throw ex.InnerException;
			}
		}
	}
}