using System;
using Microsoft.AspNetCore.Http;
using PseudoCQRS.Controllers;
using PseudoCQRS.Test.TestHelpers;
using PseudoCQRS.Tests.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class SessionBasedMessageManagerTests
	{
		private const string ErrorKey = "SessionBasedMessageManager_ErrorKey";
		private const string SuccessKey = "SessionBasedMessageManager_SuccessKey";

		private const string ErrorMessage = @"Error";
		private const string SuccessMessage = @"Success";

		private readonly SessionBasedMessageManager _sessionBasedMessageManager;
		private readonly IHttpContextWrapper _httpContextWrapper;

		public SessionBasedMessageManagerTests()
		{
			_httpContextWrapper = new MockHttpContextWrapper();
			_sessionBasedMessageManager = new SessionBasedMessageManager( _httpContextWrapper );
		}

		[Fact]
		public void SetErrorMessage_ShouldSaveMessageUsingErrorKey()
		{
			_sessionBasedMessageManager.SetErrorMessage( ErrorMessage );
			
			Assert.Equal( ErrorMessage, _httpContextWrapper.GetSessionItem(ErrorKey));
		}

		[Fact]
		public void SetSuccessMessage_ShouldSaveMessageUsingSuccessKey()
		{
			_sessionBasedMessageManager.SetSuccessMessage( SuccessMessage );
			Assert.Equal( SuccessMessage, _httpContextWrapper.GetSessionItem(SuccessKey));
		}

		[Fact]
		public void GetErrorMessage_ShouldGetMessageUsingErrorKey()
		{
			_sessionBasedMessageManager.SetSuccessMessage( SuccessMessage );
			_sessionBasedMessageManager.SetErrorMessage( ErrorMessage );

			Assert.Equal( ErrorMessage, _sessionBasedMessageManager.GetErrorMessage() );
		}

		[Fact]
		public void GetSuccessMessage_ShouldGetMessageUsingSuccessKey()
		{
			_sessionBasedMessageManager.SetSuccessMessage( SuccessMessage );
			_sessionBasedMessageManager.SetErrorMessage( ErrorMessage );

			Assert.Equal( SuccessMessage, _sessionBasedMessageManager.GetSuccessMessage() );
		}

		[Fact]
		public void GetErrorMessageShouldReturnEmptyStringWhenNoErrorMessageExists()
		{
			Assert.Equal( String.Empty, _sessionBasedMessageManager.GetErrorMessage() );
		}

		[Fact]
		public void GetSuccessMessage_ShouldReturnEmptyStringWhenNoErrorMessageExists()
		{
			Assert.Equal( String.Empty, _sessionBasedMessageManager.GetSuccessMessage() );
		}
	}
}