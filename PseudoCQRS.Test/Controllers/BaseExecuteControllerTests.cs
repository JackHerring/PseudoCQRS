using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PseudoCQRS.Controllers;
using PseudoCQRS.Tests.Controllers.Helpers;
using Xunit;

namespace PseudoCQRS.Tests.Controllers
{
	public class BaseExecuteControllerTests
	{
		private readonly Mock<ICommandExecutor> _commandExecutorMock;
		private readonly Mock<IMessageManager> _messageManagerMock;
		private readonly Mock<IReferrerProvider> _referrerProviderMock;
		private readonly Mock<IViewModelToCommandMappingEngine> _mappingEngineMock;
		private readonly DummyExecuteController _controller;

		public BaseExecuteControllerTests()
		{
			_commandExecutorMock = new Mock<ICommandExecutor>();
			_mappingEngineMock = new Mock<IViewModelToCommandMappingEngine>();
			_messageManagerMock = new Mock<IMessageManager>();
			_referrerProviderMock = new Mock<IReferrerProvider>();
			_controller = ControllerInstantiater.Instantiate<DummyExecuteController>( _commandExecutorMock.Object, _mappingEngineMock.Object, _messageManagerMock.Object, _referrerProviderMock.Object );
		}

		private async Task<IActionResult> ArrangeAndAct_WhenModelStateIsNotValid( string url )
		{
			_controller.ModelState.AddModelError( "TestError", "Error Message" );

			_referrerProviderMock
				.Setup( x => x.GetAbsoluteUri() )
				.Returns( url );

			return await _controller.Execute( new DummyExecuteViewModel() );
		}

		private async Task<IActionResult> ArrangeAndAct_WhenModelStateIsValid()
		{
			var mapper = new Mock<IViewModelToCommandMappingEngine>();
			var serviceProviderMock = new Mock<IServiceProvider>();
			serviceProviderMock
				.Setup( x => x.GetService(typeof(IViewModelToCommandMappingEngine)) )
				.Returns( mapper );
			_commandExecutorMock
				.Setup( x => x.ExecuteCommandAsync<DummyExecuteCommand, CommandResult>( null, It.IsAny<CancellationToken>() ) )
				.Returns( Task.FromResult( new CommandResult
				{
					ContainsError = false,
					Message = "Success"
				} ) );

			_referrerProviderMock
				.Setup( x => x.GetAbsoluteUri() )
				.Returns( "localhost" );

			return await _controller.Execute( new DummyExecuteViewModel() );
		}

		[Fact]
		public void ShouldSetValidationFailue_WhenModalStateIsNotValid()
		{
			ArrangeAndAct_WhenModelStateIsNotValid( "localhost" );
			_messageManagerMock
				.Verify( x => x.SetErrorMessage( It.IsAny<string>() ) );
		}

		[Fact]
		public async Task Execute_WhenViewModelInValid_ReturnsRedirectResult()
		{
			var actionResult = await ArrangeAndAct_WhenModelStateIsNotValid( "localhost" );
			Assert.IsType<RedirectResult>( actionResult );
		}

		[Fact]
		public void ShouldExecuteCommand_WhenModalStateIsValid()
		{
			var result = ArrangeAndAct_WhenModelStateIsValid();
			_commandExecutorMock
				.Verify( x => x.ExecuteCommandAsync<DummyExecuteCommand, CommandResult>( It.IsAny<DummyExecuteCommand>(), CancellationToken.None ) );
		}

		[Fact]
		public async Task Execute_WhenViewModelValid_ReturnsRedirectResult()
		{
			var actionResult = await ArrangeAndAct_WhenModelStateIsValid();
			Assert.IsType<RedirectResult>( actionResult );
		}

		[Fact]
		public void HasSetAllDependencies()
		{
			var messageManager = new Mock<IMessageManager>();
			var commandExecutor = new Mock<ICommandExecutor>();
			var referrerProvider = new Mock<IReferrerProvider>();

			var locator = new Mock<IServiceProvider>();
			locator
				.Setup( x => x.GetService(typeof(ICommandExecutor)  ) )
				.Returns( commandExecutor );
			locator
				.Setup( x => x.GetService(typeof(IMessageManager)) )
				.Returns( messageManager );
			locator
				.Setup( x => x.GetService(typeof(IReferrerProvider)) )
				.Returns( referrerProvider );

			var controller = ControllerInstantiater.Instantiate<DummyExecuteController>( commandExecutor.Object, _mappingEngineMock.Object, messageManager.Object, referrerProvider.Object );

			HasSetAllDependenciesControllerHelper.AssertFieldsAreNotNull( controller );
		}
	}
}