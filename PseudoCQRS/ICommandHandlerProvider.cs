using System;

namespace PseudoCQRS
{
	public interface ICommandHandlerProvider
	{
		IAsyncCommandHandler<TCommand, TCommandResult> GetAsyncCommandHandler<TCommand, TCommandResult>()
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult;

		[Obsolete("Please add an async CommandHandler and use " + nameof(GetAsyncCommandHandler) + "instead")]
		ICommandHandler<TCommand, TCommandResult> GetCommandHandler<TCommand, TCommandResult>()
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult;
	}
}