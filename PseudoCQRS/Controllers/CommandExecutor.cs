using System.Threading;
using System.Threading.Tasks;

namespace PseudoCQRS.Controllers
{
	public class CommandExecutor : ICommandExecutor
	{
		private readonly ICommandBus _commandBus;
		private readonly IMessageManager _messageManager;

		public CommandExecutor(
			ICommandBus commandBus,
			IMessageManager messageManager )
		{
			_commandBus = commandBus;
			_messageManager = messageManager;
		}

		public async Task<TCommandResult> ExecuteCommandAsync<TCommand, TCommandResult>( TCommand command, CancellationToken cancellationToken = default(CancellationToken)) 
			where TCommand : ICommand<TCommandResult> where TCommandResult : CommandResult, new()
		{
			var result = await _commandBus.ExecuteAsync( command, cancellationToken);
			if ( result.ContainsError )
				_messageManager.SetErrorMessage( result.Message );
			else
				_messageManager.SetSuccessMessage( result.Message );
			return result;
		}
	}
}