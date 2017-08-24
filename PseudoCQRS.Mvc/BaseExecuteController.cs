using System;
using System.Linq;
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
	public abstract class BaseExecuteController<TViewModel, TCommand> : BaseExecuteController<TViewModel, TCommand, CommandResult>
		where TViewModel : class
		where TCommand : ICommand<CommandResult>
	{
	}

	public abstract class BaseExecuteController<TViewModel, TCommand, TCommandResult> : BaseCommandController<TViewModel, TCommand, TCommandResult>
		where TViewModel : class
		where TCommand : ICommand<TCommandResult>
		where TCommandResult : CommandResult, new()
	{
		private IMessageManager _messageManager => ControllerDependencyResolver.GetControllerDependency<IMessageManager>( this );

		private IReferrerProvider _referrerProvider => ControllerDependencyResolver.GetControllerDependency<IReferrerProvider>( this );


		public override ValueTask<IActionResult> OnSuccessfulExecution( TViewModel viewModel, TCommandResult commandResult )
		{
			return new ValueTask<IActionResult>( Redirect( _referrerProvider.GetAbsoluteUri() ) );
		}

		public override ValueTask<IActionResult> OnFailureExecution( TViewModel viewModel, TCommandResult commandResult )
		{
			return new ValueTask<IActionResult>( Redirect( _referrerProvider.GetAbsoluteUri() ) );
		}

		[AcceptVerbs( "POST" )]
		public virtual async Task<IActionResult> Execute( TViewModel viewModel )
		{
			if ( !ModelState.IsValid )
			{
				var errorMessage = GetErrorMessage();
				_messageManager.SetErrorMessage( errorMessage );
				return await OnFailureExecution( viewModel, new TCommandResult
				{
					ContainsError = true,
					Message = errorMessage
				} );
			}

			return await ExecuteCommandAndGetActionResult( viewModel, RequestAbortedCancellationToken );
		}

		protected virtual string GetErrorMessage()
		{
			return String.Join( "\r\n", ModelState
				.Where( x => x.Value.Errors.Count > 0 )
				.Select( x => String.Join( "\r\n", x.Value.Errors.Select( y => String.IsNullOrEmpty( y.ErrorMessage ) ? y.Exception.ToString() : y.ErrorMessage ) ) ) );
		}
	}
}