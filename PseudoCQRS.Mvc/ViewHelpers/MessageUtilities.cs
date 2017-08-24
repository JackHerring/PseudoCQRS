using System;
#if !MVC5
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#elif MVC5
using Microsoft.Practices.ServiceLocation;
using System.Web.Mvc;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
using HtmlString = System.Web.Mvc.MvcHtmlString;
#endif
using PseudoCQRS.Controllers;

namespace PseudoCQRS.ViewHelpers
{
	public static class MessageUtilities
	{
		public static HtmlString GetErrorMessage( this IHtmlHelper helper )
		{
#if !MVC5
			var messageManager = (IMessageManager)helper.ViewContext.HttpContext.RequestServices.GetService( typeof( IMessageManager ) );
#elif MVC5
			var messageManager = (IMessageManager)ServiceLocator.Current.GetService( typeof( IMessageManager ) );
#endif

			if ( messageManager == null )
				return new HtmlString( "Instance of IMessageManager could not be found.  Don't forget to register it in your DI configuration" );

			var message = messageManager.GetErrorMessage();
			return message == String.Empty ? HtmlString.Empty : new HtmlString( message );
		}
	}
}