using System;

namespace PseudoCQRS
{
	public class SynchronousAsyncCommandInvocationException<TCommand, TCommandResult> : Exception
		where TCommand : ICommand<TCommandResult>
		where TCommandResult : CommandResult
	{
		internal SynchronousAsyncCommandInvocationException()
			: base( $"Unable to invoke asynchronous CommandHandler for instance of {nameof(TCommand)} in a synchronous fashion" )
		{
		}
	}
}