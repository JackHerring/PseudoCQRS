using System;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PseudoCQRS.Controllers;
using PseudoCQRS.Tests.Controllers.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class BaseCommandControllerTests
	{
		private readonly Mock<IViewModelToCommandMappingEngine> _mapperMock;
		private readonly Mock<ICommandExecutor> _commandExecutorMock;
		private readonly Mock<IMessageManager> _messageManagerMock;
		private readonly Mock<IReferrerProvider> _referrerProviderMock;
		private readonly DummyExecuteController _controller;


		public BaseCommandControllerTests()
		{
			_mapperMock = new Mock<IViewModelToCommandMappingEngine>();

			_commandExecutorMock = new Mock<ICommandExecutor>();
			_messageManagerMock = new Mock<IMessageManager>();
			_referrerProviderMock = new Mock<IReferrerProvider>();
			_controller = ControllerInstantiater.Instantiate<DummyExecuteController>( _commandExecutorMock.Object, _mapperMock.Object, _messageManagerMock.Object, _referrerProviderMock.Object );

			var routeData = new RouteData();
			routeData.Values.Add( "controller", "DummyDeleteFile" );
			_controller.ControllerContext = new ControllerContext
			{
				RouteData = routeData
			};
		}

		[Fact]
		public void HasSetAllDependencies()
		{
			var commandExecutor = new Mock<ICommandExecutor>();

			var locator = new Mock<IServiceProvider>();
			locator
				.Setup( x => x.GetService( typeof(ICommandExecutor) ))
				.Returns( commandExecutor );

			var controller = ControllerInstantiater.Instantiate<DummyCommandController>( _commandExecutorMock.Object, _mapperMock.Object );

			HasSetAllDependenciesControllerHelper.AssertFieldsAreNotNull( controller );
		}
	}

	public class ControllerInstantiater
	{
		public static TController Instantiate<TController>(params object[] dependencies) where TController : Controller, new()
		{
			var controller = new TController();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddMvc();
			serviceCollection.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>( new NullLoggerFactory() );
			foreach ( var dependency in dependencies )
				serviceCollection.AddTransient( dependency.GetType().GetInterfaces()[2], sp => dependency );

			var httpContext = new Mock<HttpContext>();
			httpContext.Setup( x => x.RequestServices ).Returns( serviceCollection.BuildServiceProvider() );

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext.Object
			};

			return controller;
		}
	}
}