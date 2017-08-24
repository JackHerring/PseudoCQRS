using System;
using Moq;
using PseudoCQRS.Checkers;
using Xunit;

namespace PseudoCQRS.Tests
{
	public class ViewModelFactoryTests
	{
		public class TestViewModel {}

		public class TestViewModelProviderArgument {}

		private readonly Mock<IViewModelProvider<TestViewModel, TestViewModelProviderArgument>> _viewModelProviderMock;
		private Mock<IViewModelProviderArgumentsProvider> _viewModelProviderArgumentsProviderMock;
		private readonly Mock<IPrerequisitesChecker> _prerequisitesChecker;
		private readonly ViewModelFactory<TestViewModel, TestViewModelProviderArgument> _viewModelFactory;

		public ViewModelFactoryTests()
		{
			_viewModelProviderMock = new Mock<IViewModelProvider<TestViewModel, TestViewModelProviderArgument>>();
			_viewModelProviderArgumentsProviderMock = new Mock<IViewModelProviderArgumentsProvider>();
			_prerequisitesChecker = new Mock<IPrerequisitesChecker>();
			_viewModelFactory = new ViewModelFactory<TestViewModel, TestViewModelProviderArgument>(
				_viewModelProviderMock.Object,
				_viewModelProviderArgumentsProviderMock.Object,
				_prerequisitesChecker.Object );
		}

		[Fact]
		public void ShouldThrowExceptionWhenPreRequisiteCheckContainsError()
		{
			_prerequisitesChecker
				.Setup( x => x.Check( It.IsAny<TestViewModelProviderArgument>() ) )
				.Returns( new CheckResult
				{
					ContainsError = true,
					Message = "Error"
				} );

			Assert.Throws<ArgumentException>( () => _viewModelFactory.GetViewModel() );
		}

		[Fact]
		public void ShouldReturnViewModelWhenPreRequisiteCheckReturnsSuccess()
		{
			_prerequisitesChecker
				.Setup( x => x.Check( It.IsAny<TestViewModelProviderArgument>() ) )
				.Returns( new CheckResult() );

			_viewModelProviderMock
				.Setup( x => x.GetViewModel( It.IsAny<TestViewModelProviderArgument>() ) )
				.Returns( new TestViewModel() );

			var viewModel = _viewModelFactory.GetViewModel();

			Assert.NotNull( viewModel );
		}
	}
}