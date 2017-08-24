using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PseudoCQRS.Controllers;

namespace PseudoCQRS.Tests.Controllers.Helpers
{
	public class DummyCommandController : BaseCommandController<DummyCommandViewModel, DummyCommandCommand>
	{
		public override ValueTask<IActionResult> OnSuccessfulExecution( DummyCommandViewModel viewModel, CommandResult commandResult )
		{
			throw new NotImplementedException();
		}

		public override ValueTask<IActionResult> OnFailureExecution( DummyCommandViewModel viewModel, CommandResult commandResult )
		{
			throw new NotImplementedException();
		}
	}
}