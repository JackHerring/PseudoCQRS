using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PseudoCQRS.Controllers;

namespace PseudoCQRS.Tests.Controllers.Helpers
{
	public class DummyReadExecuteController : BaseReadExecuteController<DummyReadExecuteViewModel, DummyReadExecuteViewModelArgs, DummyReadExecuteCommand>
	{
		public override ValueTask<IActionResult> OnFailureExecution( DummyReadExecuteViewModel viewModel, CommandResult result )
		{
			var actionResult = base.OnFailureExecution( viewModel, result );
			TempData[ "Error" ] = "Failed";
			return actionResult;
		}

		public override ValueTask<IActionResult> OnSuccessfulExecution( DummyReadExecuteViewModel viewModel, CommandResult command )
		{
			return new ValueTask<IActionResult>( Content( "Success" ) );
		}

		public override string ViewPath
		{
			get { return "Testing/Page"; }
		}
	}
}