using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace PseudoCQRS.Controllers
{
	public class ReferrerProvider : IReferrerProvider
	{
		private readonly IHttpContextWrapper _httpContextWrapper;
		
		public ReferrerProvider( IHttpContextWrapper httpContextWrapper )
		{
			_httpContextWrapper = httpContextWrapper;
		}

		public string GetAbsoluteUri() => _httpContextWrapper.GetUrlReferrerAbsoluteUri();
	}
}