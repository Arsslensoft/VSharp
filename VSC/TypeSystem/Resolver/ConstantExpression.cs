using System;
using System.Globalization;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// ResolveResult representing a compile-time constant.
	/// Note: this class is mainly used for literals; there may be other ResolveResult classes
	/// which are compile-time constants as well.
	/// For example, a reference to a <c>const</c> field results in a <see cref="MemberExpression"/>.
	/// 
	/// Check <see cref="Expression.IsCompileTimeConstant"/> to determine is a resolve result is a constant.
	/// </summary>
	public class ConstantExpression : Expression
	{
		object constantValue;
		
		public ConstantExpression(IType type, object constantValue) : base(type)
		{
			this.constantValue = constantValue;
		}
		
		public override bool IsCompileTimeConstant {
			get { return true; }
		}
		
		public override object ConstantValue {
			get { return constantValue; }
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1} = {2}]", GetType().Name, this.Type, constantValue);
		}
	}
}
