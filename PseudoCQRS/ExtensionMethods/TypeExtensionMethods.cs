using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PseudoCQRS.ExtensionMethods
{
    public static class TypeExtensionMethods
    {
		public static bool HasTransactionAttribute( this object obj )
		{
			var type = obj.GetType();
			if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( AsyncCommandHandlerWrapper<,> ) )
				return type.GetProperty( "CommandHandler" ).GetValue( obj ).HasTransactionAttribute();
			var attributeType = typeof(DbTransactionAttribute);
			return type.GetCustomAttributes(attributeType, false).Any();
		}
    }
}
