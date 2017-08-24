using System;
#if !MVC5
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
#elif MVC5
using System.Web.Mvc;
#endif

namespace PseudoCQRS.Controllers.ExtensionMethods
{
	public static class ControllerExtensionMethods
	{
		public static string GetView( this Controller controller )
		{
			string path = ( (IViewPath)controller ).ViewPath;
			return String.Format( "~/Views/{0}.cshtml", path );
		}
	}
}