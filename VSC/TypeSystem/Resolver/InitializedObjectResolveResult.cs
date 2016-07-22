using System;
using System.Collections.Generic;
using System.Linq;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Refers to the object that is currently being initialized.
	/// Used within <see cref="InvocationResolveResult.InitializerStatements"/>.
	/// </summary>
	public class InitializedObjectExpression : Expression
	{
        public InitializedObjectExpression(IType type)
		{
            ResolvedType = type;
            _resolved = true;
		}

	}
}
