using PseudoCQRS.Controllers;

namespace PseudoCQRS.Tests.Controllers.Helpers
{
	public class DummyReadController : BaseReadController<DummyReadViewModel, DummyReadViewModelProviderArgument>
	{

		public override string ViewPath
		{
			get { return "Test/Read"; }
		}
	}
}