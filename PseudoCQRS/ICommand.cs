namespace PseudoCQRS
{
	public interface ICommand : ICommand<CommandResult>
	{
	}

	public interface ICommand<T> where T : CommandResult
	{
	}
}