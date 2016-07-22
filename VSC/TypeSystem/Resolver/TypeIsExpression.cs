using System;
using VSC.AST;
using VSC.Base;


namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// ResolveScope result for a V# 'is' expression.
	/// "Input is TargetType".
	/// </summary>
	public class TypeIsExpression : Expression
	{
		public readonly Expression Input;
		/// <summary>
		/// Type that is being compared with.
		/// </summary>
		public readonly IType TargetType;
		
		public TypeIsExpression(Expression input, IType targetType, IType booleanType)
			: base(booleanType)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			this.Input = input;
			this.TargetType = targetType;
		}
	}
}
