using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PseudoCQRS.Checkers
{
	public class CheckersFinder : ICheckersFinder
	{
		private readonly IServiceProvider _serviceProvider;

		public CheckersFinder( IServiceProvider serviceProvider )
		{
			_serviceProvider = serviceProvider;
		}

		private class CheckersFinderResult<TAttribute, TChecker>
		{
			public TAttribute Attribute { get; set; }
			public TChecker Checker { get; set; }
		}

		private IEnumerable<CheckersFinderResult<TAttribute, TChecker>> GetCheckersImplmenting<TAttribute, TChecker>( object instance )
			where TAttribute : Attribute
		{
			var result = new List<CheckersFinderResult<TAttribute, TChecker>>();

			foreach ( var attrib in instance.GetType().GetTypeInfo().GetCustomAttributes( typeof( TAttribute ), true ) )
			{
				var checker = _serviceProvider.GetService( ( (BaseCheckAttribute)attrib ).CheckerType );
				result.Add( new CheckersFinderResult<TAttribute, TChecker>
				{
					Checker = (TChecker)checker,
					Attribute = (TAttribute)attrib
				} );
			}

			return result;
		}

		public List<IValidationChecker<T>> FindValidationCheckers<T>( T instance )
		{
			return GetCheckersImplmenting<ValidationCheckAttribute, IValidationChecker<T>>( instance ).Select( x => x.Checker ).ToList();
		}

		public List<IAuthorizationChecker> FindAuthorizationCheckers( object instance )
		{
			return GetCheckersImplmenting<AuthorizationCheckAttribute, IAuthorizationChecker>( instance ).Select( x => x.Checker ).ToList();
		}

		public List<AccessCheckerAttributeDetails> FindAccessCheckers( object instance )
		{
			return GetCheckersImplmenting<AccessCheckAttribute, IAccessChecker>( instance )
				.Select( x => new AccessCheckerAttributeDetails
				{
					AccessChecker = x.Checker,
					PropertyName = x.Attribute.PropertyName
				} )
				.ToList();
		}
	}
}