using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using PseudoCQRS.Tests.Controllers.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class BaseReadControllerTests
	{
		private readonly Mock<IViewModelFactory<DummyReadViewModel, DummyReadViewModelProviderArgument>> _viewModelFactoryMock;
		private readonly DummyReadController _controller;

		public BaseReadControllerTests()
		{
			_viewModelFactoryMock = new Mock<IViewModelFactory<DummyReadViewModel, DummyReadViewModelProviderArgument>>();
			_controller = ControllerInstantiater.Instantiate<DummyReadController>( _viewModelFactoryMock.Object );

			var routeData = new RouteData();
			routeData.Values.Add( "controller", "DummyDeleteFile" );
			_controller.ControllerContext.RouteData = routeData;
			_controller.TempData = new TempDataDictionary( _controller.HttpContext, new Mock<ITempDataProvider>().Object );
		}

		[Fact]
		public void ShouldReturnView()
		{
			Assert.IsType<ViewResult>( _controller.Execute() );
		}

		[Fact]
		public void ShouldCallGetViewModel()
		{
			_controller.Execute();
			_viewModelFactoryMock
				.Verify( x => x.GetViewModel() );
		}

		[Fact]
		public void HasSetAllDependencies()
		{
			var viewModelFactory = new Mock<IViewModelFactory<DummyReadViewModel, DummyReadViewModelProviderArgument>>();
			var locator = new Mock<IServiceProvider>();
			locator
				.Setup( x => x.GetService(typeof(IViewModelFactory<DummyReadViewModel, DummyReadViewModelProviderArgument>)) )
				.Returns( viewModelFactory );

			var controller = ControllerInstantiater.Instantiate<DummyReadController>( _viewModelFactoryMock.Object );
			HasSetAllDependenciesControllerHelper.AssertFieldsAreNotNull( controller );
		}
	}
}