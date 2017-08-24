using System.Collections.Generic;

namespace PseudoCQRS.Test.TestHelpers
{
	public class MockHttpContextWrapper : IHttpContextWrapper
	{
		private string _referrer;
		public string GetUrlReferrerAbsoluteUri() => _referrer;
		public void SetUrlReferrer( string referrer ) => _referrer = referrer;

		private readonly Dictionary<string, string> _session = new Dictionary<string, string>();
		public void SetSessionItem( string key, string value ) => _session[ key ] = value;
		public string GetSessionItem( string key ) => _session[ key ];
		public void SessionRemoveItem( string key ) => _session.Remove( key );
		public bool ContainsSessionItem( string key ) => _session.ContainsKey( key );

		private readonly Dictionary<string, string> _cookies = new Dictionary<string, string>();
		public bool RequestContainsCookie( string cookieName ) => _cookies.ContainsKey( cookieName );
		public string RequestGetCookieValue( string cookieName ) => _cookies[ cookieName ];
		public void ResponseSetCookie( string name, string value ) => _cookies[ name ] = value;

		private readonly Dictionary<string, string> _form = new Dictionary<string, string>();
		public bool ContainsFormItem( string key ) => _form.ContainsKey( key );
		public string GetFormItem( string key ) => _form[ key ];
		public void SetFormItem( string key, string value ) => _form[ key ] = value;

		private readonly Dictionary<string, object> _route = new Dictionary<string, object>();
		public bool ContainsRouteDataItem( string key ) => _route.ContainsKey( key );
		public object GetRouteDataItem( string key ) => _route[ key ];
		public void SetRouteDataItem( string key, object value ) => _route[ key ] = value;

		private readonly Dictionary<string, string> _queryString = new Dictionary<string, string>();
		public string GetQueryStringItem( string key ) => _queryString[ key ];
		public bool ContainsQueryStringItem( string key ) => _queryString.ContainsKey( key );
		public void SetQueryStringItem( string key, string value ) => _queryString[ key ] = value;
	}
}