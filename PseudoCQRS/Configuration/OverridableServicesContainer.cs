using System;
using System.Collections.Generic;
using PseudoCQRS.Checkers;
using PseudoCQRS.Controllers;
using PseudoCQRS.Helpers;
using PseudoCQRS.PropertyValueProviders;

namespace PseudoCQRS.Configuration
{
	public class OverridableServicesContainer
	{
		private readonly Dictionary<Type, Type> _overrides;

		public OverridableServicesContainer()
		{
			_overrides = new Dictionary<Type, Type>();
		}

		private void AddOverride<TInterface, TImplementation>()
		{
			_overrides.Add( typeof( TInterface ), typeof( TImplementation ) );
		}

		public void CheckersExecutor<TCheckersExecutor>() where TCheckersExecutor : ICheckersExecuter
		{
			AddOverride<ICheckersExecuter, TCheckersExecutor>();
		}

		public void CheckersFinder<TCheckersFinder>() where TCheckersFinder : ICheckersFinder
		{
			AddOverride<ICheckersFinder, TCheckersFinder>();
		}

		public void PrerequisitesChecker<TPrerequisitesChecker>() where TPrerequisitesChecker : IPrerequisitesChecker
		{
			AddOverride<IPrerequisitesChecker, TPrerequisitesChecker>();
		}

		public void CommandExecutor<TCommandExecutor>() where TCommandExecutor : ICommandExecutor
		{
			AddOverride<ICommandExecutor, TCommandExecutor>();
		}

		public void MessageManager<TMessageManager>() where TMessageManager : IMessageManager
		{
			AddOverride<IMessageManager, TMessageManager>();
		}

		public void ReferrerProvider<TReferrerProvider>() where TReferrerProvider : IReferrerProvider
		{
			AddOverride<IReferrerProvider, TReferrerProvider>();
		}

		public void ObjectLookupCache<TObjectLookupCache>() where TObjectLookupCache : IObjectLookupCache
		{
			AddOverride<IObjectLookupCache, TObjectLookupCache>();
		}

		public void PropertyValueProviderFactory<TPropertyValueProviderFactory>() where TPropertyValueProviderFactory : IPropertyValueProviderFactory
		{
			AddOverride<IPropertyValueProviderFactory, TPropertyValueProviderFactory>();
		}

		public void AssemblyListProvider<TAssemblyListProvider>() where TAssemblyListProvider : IAssemblyListProvider
		{
			AddOverride<IAssemblyListProvider, TAssemblyListProvider>();
		}

		public void CommandBus<TCommandBus>() where TCommandBus : ICommandBus
		{
			AddOverride<ICommandBus, TCommandBus>();
		}

		public void CommandHandlerFinder<TCommandHandlerFinder>() where TCommandHandlerFinder : ICommandHandlerFinder
		{
			AddOverride<ICommandHandlerFinder, TCommandHandlerFinder>();
		}

		public void CommandHandlerProvider<TCommandHandlerProvider>() where TCommandHandlerProvider : ICommandHandlerProvider
		{
			AddOverride<ICommandHandlerProvider, TCommandHandlerProvider>();
		}

		public void ViewModelProviderArgumentsProvider<TViewModelProviderArgumentsProvider>() where TViewModelProviderArgumentsProvider : IViewModelProviderArgumentsProvider
		{
			AddOverride<IViewModelProviderArgumentsProvider, TViewModelProviderArgumentsProvider>();
		}

		internal Dictionary<Type, Type> GetOverrides()
		{
			return _overrides;
		}
	}
}