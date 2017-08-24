using System;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoCQRS
{
	public interface ICommandBus
	{
		Task<TCommandResult> ExecuteAsync<TCommandResult>( ICommand<TCommandResult> command, CancellationToken cancellationToken = default(CancellationToken) )
			where TCommandResult : CommandResult, new();

		[Obsolete("Please use " + nameof(ExecuteAsync) + "instead")]
		TCommandResult Execute<TCommandResult>(ICommand<TCommandResult> command )
			where TCommandResult : CommandResult, new();
	}
}