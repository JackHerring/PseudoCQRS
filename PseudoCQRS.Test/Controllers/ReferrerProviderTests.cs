using Microsoft.AspNetCore.Http;
using PseudoCQRS.Controllers;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class ReferrerProviderTests
	{
		[Fact]
		public void GetAbsoluteUri_Always_ReturnsUrlReferrerAbsoluteUri()
		{
			var httpContextWrapper = new MockHttpContextWrapper();

			const string expected = "http://localhost/";
			httpContextWrapper.SetUrlReferrer( expected );

			var uri = new ReferrerProvider( httpContextWrapper ).GetAbsoluteUri();
			Assert.Equal( expected, uri );
		}
	}
}