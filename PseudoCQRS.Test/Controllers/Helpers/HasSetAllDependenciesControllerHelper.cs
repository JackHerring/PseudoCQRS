using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace PseudoCQRS.Tests.Controllers.Helpers
{
	public static class HasSetAllDependenciesControllerHelper
	{
		public static void AssertFieldsAreNotNull( Controller controller )
		{
			var allReadOnlyFields = new List<FieldInfo>();

			Type controllerType = controller.GetType();
			while ( controllerType != null )
			{
				var fields =
					controllerType
						.GetFields( BindingFlags.NonPublic | BindingFlags.Instance )
						.Where( x => x.IsInitOnly );
				allReadOnlyFields.AddRange( fields );
				controllerType = controllerType.BaseType;
			}

			var fieldsWithNullValue = allReadOnlyFields
				.Where( x => x.GetValue( controller ) == null )
				.ToList();

			if ( fieldsWithNullValue.Count > 0 )
			{
				Console.WriteLine( "The Following " + fieldsWithNullValue.Count + " fields are not set" );
				fieldsWithNullValue.ForEach( x => Console.WriteLine( x.Name ) );
				Assert.True( false );
			}
		}
	}
}