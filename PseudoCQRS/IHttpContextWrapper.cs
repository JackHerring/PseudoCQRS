namespace PseudoCQRS
{
	public interface IHttpContextWrapper
	{
		string GetUrlReferrerAbsoluteUri();

		void SetSessionItem(string key, string value);
		string GetSessionItem(string key);
		void SessionRemoveItem(string key);
		bool ContainsSessionItem(string key);

		bool RequestContainsCookie(string cookieName);
		string RequestGetCookieValue(string cookieName);
		void ResponseSetCookie(string name, string value);

		bool ContainsFormItem(string key);
		string GetFormItem(string key);

		bool ContainsRouteDataItem(string key);
		object GetRouteDataItem(string key);

		string GetQueryStringItem(string key);
		bool ContainsQueryStringItem(string key);
	}
}
