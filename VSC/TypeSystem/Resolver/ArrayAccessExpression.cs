using System;
using System.Collections.Generic;
using System.Linq;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// ResolveScope result representing an array access.
	/// </summary>
	public class ArrayAccessExpression : Expression
	{
		public readonly Expression Array;
		public readonly IList<Expression> Indexes;
		
		public ArrayAccessExpression(IType elementType, Expression array, IList<Expression> indexes) : base(elementType)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (indexes == null)
				throw new ArgumentNullException("indexes");
			this.Array = array;
			this.Indexes = indexes;
		}
	
	}
}
