using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using PseudoCQRS.Controllers.ExtensionMethods;
#if !MVC5
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#elif MVC5
using System.Web.Mvc;
using IActionResult = System.Web.Mvc.ActionResult;
#endif

namespace PseudoCQRS.Controllers
{
	public abstract class BaseReadExecuteController<TViewModel, TArgs, TCommand> : BaseReadExecuteController<TViewModel, TArgs, TCommand, CommandResult>
		where TViewModel : class
		where TCommand : ICommand
		where TArgs : new()
	{
	}

	public abstract class BaseReadExecuteController<TViewModel, TArgs, TCommand, TCommandResult> : BaseCommandController<TViewModel, TCommand, TCommandResult>, IViewPath
		where TViewModel : class
		where TCommand : ICommand<TCommandResult>
		where TCommandResult : CommandResult, new()
		where TArgs : new()
	{
		private IViewModelFactory<TViewModel, TArgs> _viewModelFactory => ControllerDependencyResolver.GetControllerDependency<IViewModelFactory<TViewModel, TArgs>>( this );

		public abstract string ViewPath { get; }

		public override ValueTask<IActionResult> OnFailureExecution( TViewModel viewModel, TCommandResult commandResult )
		{
			return new ValueTask<IActionResult>( View( this.GetView(), viewModel ) );
		}

		protected virtual async Task<IActionResult> GetActionResult( TViewModel viewModel, CancellationToken cancellationToken )
		{
			return await ExecuteCommandAndGetActionResult( viewModel, cancellationToken );
		}

		[AcceptVerbs( "GET", "HEAD" )]
		public virtual IActionResult Execute()
		{
			return GetViewResult( GetViewModel() );
		}

		protected virtual TViewModel GetViewModel()
		{
			try
			{
				return _viewModelFactory.GetViewModel();
			}
			catch ( ArgumentBindingException exception )
			{
				throw new HttpRequestException( exception.Message, exception.InnerException );
			}
		}

		[AcceptVerbs( "POST" )]
		public virtual async Task<IActionResult> Execute( FormCollection form )
		{
			var viewModel = GetViewModel();
#if MVCCORE
			return await TryUpdateModelAsync( viewModel ) ? await GetActionResult( viewModel, RequestAbortedCancellationToken ) : GetViewResult( viewModel );
#else
			return TryUpdateModel(viewModel) ? await GetActionResult(viewModel, RequestAbortedCancellationToken) : GetViewResult(viewModel);
#endif
		}

		protected virtual IActionResult GetViewResult( TViewModel viewModel )
		{
			return View( this.GetView(), viewModel );
		}
	}
}