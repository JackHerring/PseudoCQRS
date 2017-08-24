using System.Net.Http;
using System.Web;
using PseudoCQRS.Controllers.ExtensionMethods;
#if !MVC5
using Microsoft.AspNetCore.Mvc;
#elif MVC5
using System.Web.Mvc;
using IActionResult = System.Web.Mvc.ActionResult;
#endif

namespace PseudoCQRS.Controllers
{
	[DbSessionManager]
	public abstract class BaseReadController<TViewModel, TArgs> : Controller, IViewPath
		where TViewModel : class
		where TArgs : new()
	{
		private IViewModelFactory<TViewModel, TArgs> _viewModelFactory => ControllerDependencyResolver.GetControllerDependency<IViewModelFactory<TViewModel, TArgs>>( this );

		public abstract string ViewPath { get; }
		
		public virtual IActionResult Execute()
		{
			return GetActionResult( GetViewModel() );
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

		protected virtual ViewResult GetViewResult( TViewModel viewModel )
		{
			return View( this.GetView(), viewModel );
		}

		protected virtual IActionResult GetActionResult( TViewModel viewModel )
		{
			return GetViewResult( viewModel );
		}
	}
}