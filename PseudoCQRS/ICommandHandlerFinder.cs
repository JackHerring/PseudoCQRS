using System;

namespace PseudoCQRS
{
	public interface ICommandHandlerFinder
	{
		Type FindHandlerForCommand<TCommand, TCommandResult>()
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult;

		Type FindAsyncHandlerForCommand<TCommand, TCommandResult>()
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult;
	}
}