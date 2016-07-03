using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
	public interface ICodeContext : ITypeResolveContext
	{
		/// <summary>
		/// Gets all currently visible local variables and lambda parameters.
		/// Does not include method parameters.
		/// </summary>
		IEnumerable<IVariable> LocalVariables { get; }
		
		/// <summary>
		/// Gets whether the context is within a lambda expression or anonymous method.
		/// </summary>
		bool IsWithinLambdaExpression { get; }
	}
}

