using System;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PseudoCQRS.Controllers
{
	public class SessionBasedMessageManager : IMessageManager
	{
		private readonly IHttpContextWrapper _httpContextWrapper;
		private const string ErrorKey = "SessionBasedMessageManager_ErrorKey";
		private const string SuccessKey = "SessionBasedMessageManager_SuccessKey";

		public SessionBasedMessageManager( IHttpContextWrapper httpContextWrapper )
		{
			_httpContextWrapper = httpContextWrapper;
		}

		public void SetErrorMessage( string message )
		{
			SetMessage( ErrorKey, message );
		}

		public string GetErrorMessage()
		{
			return GetMessage( ErrorKey );
		}

		public void SetSuccessMessage( string message )
		{
			SetMessage( SuccessKey, message );
		}

		public string GetSuccessMessage()
		{
			return GetMessage( SuccessKey );
		}

		private void SetMessage( string key, string message ) => _httpContextWrapper.SetSessionItem( key, message );

		private string GetMessage( string key )
		{
			if ( !_httpContextWrapper.ContainsSessionItem( key ) )
				return string.Empty;

			var sessionItem = _httpContextWrapper.GetSessionItem( key );
			_httpContextWrapper.SessionRemoveItem( key );
			return sessionItem;
		}
	}
}