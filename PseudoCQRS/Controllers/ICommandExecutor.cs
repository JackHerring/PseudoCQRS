using System.Threading;
using System.Threading.Tasks;

namespace PseudoCQRS.Controllers
{
	public interface ICommandExecutor
	{
		Task<TCommandResult> ExecuteCommandAsync<TCommand, TCommandResult>( TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand<TCommandResult> where TCommandResult : CommandResult, new();
	}
}