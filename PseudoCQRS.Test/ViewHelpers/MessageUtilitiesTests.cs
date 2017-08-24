using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PseudoCQRS.Controllers;
using PseudoCQRS.ViewHelpers;
using Xunit;

namespace PseudoCQRS.Tests.ViewHelpers
{
	public class MessageUtilitiesTests
	{
		private readonly Mock<IMessageManager> _messageManagerMock;
		private readonly Mock<IHtmlHelper> _htmlHelper;
		private readonly Mock<HttpContext> _httpContext;

		public MessageUtilitiesTests()
		{
			_messageManagerMock = new Mock<IMessageManager>();

			_httpContext = new Mock<HttpContext>();

			var viewContext = new ViewContext();
			viewContext.HttpContext = _httpContext.Object;

			_htmlHelper = new Mock<IHtmlHelper>();
			_htmlHelper.Setup( x => x.ViewContext ).Returns( viewContext );
		}

		[Fact]
		public void ShouldReturnErrorWhenMessageManagerIsNotRegisteredInDiContainer()
		{
			const string errorMessage = "Error Message";
			_messageManagerMock
				.Setup( x => x.GetErrorMessage() )
				.Returns( errorMessage );
			_httpContext.Setup(x => x.RequestServices).Returns(new ServiceCollection().BuildServiceProvider());

			var retVal = _htmlHelper.Object.GetErrorMessage();
			Assert.NotEqual(errorMessage, retVal.ToString());
		}


		[Fact]
		public void GetErrorMessageShouldReturnErrorMessage()
		{
			const string errorMessage = "Error MEssage";
			_messageManagerMock
				.Setup( x => x.GetErrorMessage() )
				.Returns( errorMessage );
			_httpContext.Setup(x => x.RequestServices).Returns(new ServiceCollection()
				.AddTransient(sp => _messageManagerMock.Object)
				.BuildServiceProvider());

			var retVal = _htmlHelper.Object.GetErrorMessage();
			Assert.Equal(errorMessage, retVal.ToString());
		}
	}
}