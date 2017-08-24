using System.Threading;
using System.Threading.Tasks;
#if !MVC5
using Microsoft.AspNetCore.Mvc;
#elif MVC5
using System.Web.Mvc;
using IActionResult = System.Web.Mvc.ActionResult;
#endif

namespace PseudoCQRS.Controllers
{
	public abstract class BaseCommandController<TViewModel, TCommand> : BaseCommandController<TViewModel, TCommand, CommandResult>
		where TCommand : ICommand
	{
	}

	[DbSessionManager]
	public abstract class BaseCommandController<TViewModel, TCommand, TCommandResult> : Controller
		where TCommand : ICommand<TCommandResult>
		where TCommandResult : CommandResult, new()
	{
		private ICommandExecutor _commandExecutor => ControllerDependencyResolver.GetControllerDependency<ICommandExecutor>( this );
		private IViewModelToCommandMappingEngine _mappingEngine => ControllerDependencyResolver.GetControllerDependency<IViewModelToCommandMappingEngine>( this );

#if MVCCORE
		protected CancellationToken RequestAbortedCancellationToken => HttpContext.RequestAborted;
#else
		protected CancellationToken RequestAbortedCancellationToken => CancellationToken.None;
#endif

		//todo: make System.Threading.Tasks.Extensions (wherein 'ValueTask' lives) a dependency of this package
		public abstract ValueTask<IActionResult> OnSuccessfulExecution( TViewModel viewModel, TCommandResult commandResult );
		public abstract ValueTask<IActionResult> OnFailureExecution( TViewModel viewModel, TCommandResult commandResult );

		protected virtual async Task<TCommandResult> ExecuteCommand( TViewModel viewModel, CancellationToken cancellationToken )
		{
			var command = await ConvertViewModelToCommand( viewModel );
			var result = await ExecuteCommand( command, cancellationToken );
			return result;
		}

		private async Task<TCommandResult> ExecuteCommand( TCommand command, CancellationToken cancellationToken )
		{
			return await _commandExecutor.ExecuteCommandAsync<TCommand, TCommandResult>( command, cancellationToken );
		}

		protected virtual ValueTask<TCommand> ConvertViewModelToCommand( TViewModel viewModel )
		{
			return new ValueTask<TCommand>( _mappingEngine.Map<TViewModel, TCommand>(viewModel) );
		}

		protected async Task<IActionResult> ExecuteCommandAndGetActionResult( TViewModel viewModel, CancellationToken cancellationToken )
		{
			var commandResult = await ExecuteCommand( viewModel, cancellationToken );
			return commandResult.ContainsError ? await OnFailureExecution( viewModel, commandResult ) : await OnSuccessfulExecution( viewModel, commandResult );
		}
	}

	public static class ControllerDependencyResolver
	{
		public static T GetControllerDependency<T>( Controller controller )
		{
#if !MVC5
			return (T)controller.HttpContext.RequestServices.GetService(typeof(T));
#else
			return (T)Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetService(typeof(T));
#endif
		}
	}
}