using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if !MVC5
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
#elif MVC5
using System.Web.Mvc;
#endif
using Microsoft.Extensions.Primitives;
using Moq;
using PseudoCQRS.Controllers;
using PseudoCQRS.Tests.Controllers.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class BaseReadExecuteControllerTests
	{
		private Mock<IViewModelToCommandMappingEngine> _mapperMock;
		private Mock<IServiceProvider> _serviceProviderMock;

		private readonly DummyReadExecuteController _controller;
		private readonly Mock<ICommandExecutor> _commandExecutor;
		private readonly Mock<IViewModelFactory<DummyReadExecuteViewModel, DummyReadExecuteViewModelArgs>> _viewModelFactoryMock;

		public BaseReadExecuteControllerTests()
		{
			_mapperMock = new Mock<IViewModelToCommandMappingEngine>();
			_serviceProviderMock = new Mock<IServiceProvider>();
			_serviceProviderMock
				.Setup( x => x.GetService(typeof(IViewModelToCommandMappingEngine)) )
				.Returns( _mapperMock );

			_commandExecutor = new Mock<ICommandExecutor>();
			_viewModelFactoryMock = new Mock<IViewModelFactory<DummyReadExecuteViewModel, DummyReadExecuteViewModelArgs>>();

			_controller = ControllerInstantiater.Instantiate<DummyReadExecuteController>(
				_commandExecutor.Object,
				_mapperMock.Object,
				_viewModelFactoryMock.Object );

			_viewModelFactoryMock
				.Setup( x => x.GetViewModel() )
				.Returns( new DummyReadExecuteViewModel() );

			var routeData = new RouteData();
			routeData.Values.Add( "controller", "DummyDeleteFile" );
			_controller.ControllerContext.RouteData = routeData;

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
									It.IsAny<ValidationStateDictionary>(),
									It.IsAny<string>(),
									It.IsAny<Object>()));
			_controller.ObjectValidator = objectValidator.Object;

			_controller.TempData = new TempDataDictionary( new Mock<HttpContext>().Object, new Mock<ITempDataProvider>().Object );
		}

		[Fact]
		public void GET_ShouldReturnView()
		{
			Assert.IsType<ViewResult>( _controller.Execute() );
		}

		[Fact]
		public void GET_ShouldCallViewModelProvider()
		{
			_controller.Execute();
			_viewModelFactoryMock
				.Verify( x => x.GetViewModel() );
		}

		private async Task<IActionResult> PostArrangeExecute(bool executeCommandContainsError = false)
		{
			_viewModelFactoryMock
				.Setup(x => x.GetViewModel())
				.Returns(new DummyReadExecuteViewModel());

			_commandExecutor
				.Setup( x => x.ExecuteCommandAsync<DummyReadExecuteCommand, CommandResult>( It.IsAny<DummyReadExecuteCommand>(), It.IsAny<CancellationToken>() ) )
				.Returns( Task.FromResult( new CommandResult
				{
					ContainsError = executeCommandContainsError
				} ) );

			return await _controller.Execute(new FormCollection(new Dictionary<string, StringValues>()));
		}

		[Fact]
		public async Task POST_ShouldExecuteATestWhenCanTryUpdateModelReturnsTrue()
		{
			await PostArrangeExecute();
			_commandExecutor.Verify( x => x.ExecuteCommandAsync<DummyReadExecuteCommand, CommandResult>( It.IsAny<DummyReadExecuteCommand>(), It.IsAny<CancellationToken>() ) );
		}

		[Fact]
		public async Task POST_ShouldReturnOnFailureActionWhenCommandContainsError()
		{
			await PostArrangeExecute( true );
			Assert.Equal( "Failed", _controller.TempData[ "Error" ] );
		}

		[Fact]
		public void HasSetAllDependencies()
		{
			var viewModelFactory = new Mock<IViewModelFactory<DummyReadExecuteViewModel, DummyReadExecuteViewModelArgs>>();
			var commandExecutor = new Mock<ICommandExecutor>();
			var locator = new Mock<IServiceProvider>();
			locator
				.Setup( x => x.GetService(typeof(IViewModelFactory<DummyReadExecuteViewModel, DummyReadExecuteViewModelArgs>)) )
				.Returns( viewModelFactory );

			locator
				.Setup( x => x.GetService(typeof(ICommandExecutor)) )
				.Returns( commandExecutor );

			var controller = ControllerInstantiater.Instantiate<DummyReadExecuteController>( _commandExecutor.Object, _mapperMock.Object, viewModelFactory.Object );

			HasSetAllDependenciesControllerHelper.AssertFieldsAreNotNull( controller );
		}
	}
}