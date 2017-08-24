namespace PseudoCQRS
{
	public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, CommandResult>
		where TCommand : ICommand
	{
	}

	public interface ICommandHandler<in TCommand, out TCommandResult>
		where TCommand : ICommand<TCommandResult>
		where TCommandResult : CommandResult
	{
		TCommandResult Handle( TCommand command );
	}
}