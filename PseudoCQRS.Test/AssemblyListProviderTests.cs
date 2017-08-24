using System.Linq;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class AssemblyListProviderTests
	{
		[Fact]
		public void GetAssemblies_ReturnsAssembliesInAppDomain()
		{
			var provider = new AssemblyListProvider();
			Assert.True(provider.GetAssemblies().Count() > 1);
		}
	}
}