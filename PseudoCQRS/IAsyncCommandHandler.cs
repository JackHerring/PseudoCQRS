using System.Threading;
using System.Threading.Tasks;

namespace PseudoCQRS
{
	public interface IAsyncCommandHandler<in TCommand> : IAsyncCommandHandler<TCommand, CommandResult>
		where TCommand : ICommand
	{
	}

	public interface IAsyncCommandHandler<in TCommand, TCommandResult>
		where TCommand : ICommand<TCommandResult>
		where TCommandResult : CommandResult

	{
		Task<TCommandResult> HandleAsync(TCommand command,CancellationToken cancellationToken);
	}
}