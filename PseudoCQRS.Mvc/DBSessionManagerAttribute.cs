using System;
#if !MVC5
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
#elif MVC5
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
#endif


namespace PseudoCQRS
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
	public class DbSessionManagerAttribute : ActionFilterAttribute
	{
		public DbSessionManagerAttribute()
		{
			Order = 1;
		}

		public override void OnResultExecuted( ResultExecutedContext filterContext )
		{
#if !MVC5
			filterContext.HttpContext.RequestServices.GetService<IDbSessionManager>().CloseSession();
#elif MVC5
			if (!filterContext.IsChildAction)
				ServiceLocator.Current.GetInstance<IDbSessionManager>().CloseSession();
#endif
		}
	}
}