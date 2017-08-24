using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PseudoCQRS.Helpers;

namespace PseudoCQRS
{
	public class CommandHandlerFinder : ICommandHandlerFinder
	{
		private readonly IAssemblyListProvider _assembliesListProvider;
		private readonly static Dictionary<Type, Type> _cachedHandlers = new Dictionary<Type, Type>();

		public CommandHandlerFinder( IAssemblyListProvider assembliesListProvider )
		{
			_assembliesListProvider = assembliesListProvider;
		}

		public Type FindHandlerForCommand<TCommand,TCommandResult>() 
			where TCommand : ICommand<TCommandResult> 
			where TCommandResult : CommandResult => GetHandler<ICommandHandler<TCommand,TCommandResult>>(  );

		private Type GetHandler<THandler>()
		{
			if ( _cachedHandlers.ContainsKey( typeof( THandler ) ) )
				return _cachedHandlers[ typeof( THandler ) ];

			var handlerInheritingFromType = typeof(THandler);

			var commandHandlerType = GetCommandHandlerType( handlerInheritingFromType );

			if ( commandHandlerType != null )
				_cachedHandlers[ typeof( THandler ) ] = commandHandlerType;

			return commandHandlerType;
		}

		public static void ClearCache()
		{
			_cachedHandlers.Clear();
		}

		public Type FindAsyncHandlerForCommand<TCommand, TCommandResult>()
			where TCommand : ICommand<TCommandResult>
			where TCommandResult : CommandResult => GetHandler<IAsyncCommandHandler<TCommand,TCommandResult>>( );

		private Type GetCommandHandlerType( Type handler )
		{ 
			return _assembliesListProvider
				.GetAssemblies().SelectMany( x => x.DefinedTypes )
				.SingleOrDefault( x => x.GetTypeInfo().ImplementedInterfaces.Any( y => y == handler ) );
		}
	}
}