using System;
using System.Collections.Generic;
using System.Linq;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// ResolveScope result representing an array creation.
	/// </summary>
	public class ArrayCreateExpression : Expression
	{
		/// <summary>
		/// Gets the size arguments.
		/// </summary>
		public readonly IList<Expression> SizeArguments;
		
		/// <summary>
		/// Gets the initializer elements.
		/// This field may be null if no initializer was specified.
		/// </summary>
		public readonly IList<Expression> InitializerElements;
		
		public ArrayCreateExpression(IType arrayType, IList<Expression> sizeArguments, IList<Expression> initializerElements)
			: base(arrayType)
		{
			if (sizeArguments == null)
				throw new ArgumentNullException("sizeArguments");
			this.SizeArguments = sizeArguments;
			this.InitializerElements = initializerElements;
		}
		
	}
}
