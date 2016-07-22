using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the resolve result of an 'ref x' or 'out x' expression.
	/// </summary>
	public class ByReferenceExpression : Expression
	{
		public bool IsOut { get; private set; }
		public bool IsRef { get { return !IsOut;} }
		
		public readonly Expression ElementResult;
		
		public ByReferenceExpression(Expression elementResult, bool isOut)
			: this(elementResult.Type, isOut)
		{
			this.ElementResult = elementResult;
		}
		
		public ByReferenceExpression(IType elementType, bool isOut)
			: base(new ByReferenceType(elementType))
		{
			this.IsOut = isOut;
		}
		
		public IType ElementType {
			get { return ((ByReferenceType)this.Type).ElementType; }
		}
		
	
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1} {2}]", GetType().Name, IsOut ? "out" : "ref", ElementType);
		}
	}
}
