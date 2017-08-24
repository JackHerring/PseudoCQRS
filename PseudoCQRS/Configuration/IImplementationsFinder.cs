using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PseudoCQRS.Configuration
{
	public interface IImplementationsFinder
	{
		Dictionary<Type, Type> FindImplementations();
	}
}
