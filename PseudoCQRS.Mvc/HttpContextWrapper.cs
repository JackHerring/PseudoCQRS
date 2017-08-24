using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
#if !MVC5
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
#elif MVC5
using System.Web;
using System.Web.Mvc;
#endif

namespace PseudoCQRS.Mvc
{
#if !MVC5
public class HttpContextWrapper : IHttpContextWrapper
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private HttpContext Context => _httpContextAccessor.HttpContext;

		public HttpContextWrapper(IHttpContextAccessor httpContextAccessor )
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public string GetUrlReferrerAbsoluteUri() => Context.Request.Headers["Referer"].ToString();


		public bool ContainsSessionItem(string key) => Context.Session.Keys.Contains( key );

		public string GetQueryStringItem(string key) => Context.Request.Query[ key ];

		public bool ContainsQueryStringItem(string key) => GetQueryStringItem( key ) != null;

		public void SetSessionItem(string key, string value) => Context?.Session.SetString( key, value );

		public string GetSessionItem(string key) => Context.Session.GetString( key );

		public void SessionRemoveItem(string key) => Context.Session.Remove(key);

		public bool RequestContainsCookie(string cookieName) => RequestGetCookieValue( cookieName ) != null;

		public string RequestGetCookieValue(string cookieName) => Context.Request.Cookies[ cookieName ];

		public void ResponseSetCookie(string name, string value) => Context.Response.Cookies.Append( name, value );

		public bool ContainsFormItem(string key) => GetFormItem( key ) != null;

		public string GetFormItem(string key)
		{
			Context.Request.Form.TryGetValue( key, out StringValues formItem );
			return formItem;
		}

		public bool ContainsRouteDataItem(string key) => Context.Request.HttpContext.GetRouteData().Values.ContainsKey( key );

		public object GetRouteDataItem(string key)
		{
			Context.Request.HttpContext.GetRouteData().Values.TryGetValue( key, out object value );
			return value;
		}
	}
#elif MVC5
	public class HttpContextWrapper : IHttpContextWrapper
	{
		public string GetUrlReferrerAbsoluteUri()
		{
			return HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
		}


		public bool ContainsSessionItem(string key)
		{
			return HttpContext.Current.Session[key] != null;
		}

		public string GetQueryStringItem(string key)
		{
			return HttpContext.Current.Request.QueryString[key];
		}

		public bool ContainsQueryStringItem(string key)
		{
			return HttpContext.Current.Request.QueryString[key] != null;
		}

		public void SetSessionItem(string key, string value)
		{
			if (HttpContext.Current != null)
				HttpContext.Current.Session[key] = value;
		}

		public string GetSessionItem(string key)
		{
			var result = HttpContext.Current.Session[key];
			return result.ToString();
		}

		public void SessionRemoveItem(string key)
		{
			HttpContext.Current.Session.Remove(key);
		}

		public bool RequestContainsCookie(string cookieName)
		{
			return HttpContext.Current.Request.Cookies[cookieName] != null;
		}

		public string RequestGetCookieValue(string cookieName)
		{
			return HttpContext.Current.Request.Cookies[cookieName].Value;
		}

		public void ResponseSetCookie(string name, string value)
		{
			HttpContext.Current.Response.Cookies.Set(new HttpCookie(name, value));
		}

		public bool ContainsFormItem(string key)
		{
			return HttpContext.Current.Request.Form[key] != null;
		}

		public string GetFormItem(string key)
		{
			return HttpContext.Current.Request.Form[key];
		}

		public bool ContainsRouteDataItem(string key)
		{
			return HttpContext.Current.Request.RequestContext.RouteData.Values.ContainsKey(key);
		}

		public object GetRouteDataItem(string key)
		{
			return HttpContext.Current.Request.RequestContext.RouteData.Values[key];
		}
	}
#endif
}