using System;
using System.Globalization;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a local variable or parameter.
	/// </summary>
	public class LocalResolveResult : ResolveResult
	{
		readonly IVariable variable;
		
		public LocalResolveResult(IVariable variable)
			: base(UnpackTypeIfByRefParameter(variable))
		{
			this.variable = variable;
		}
		
		static IType UnpackTypeIfByRefParameter(IVariable variable)
		{
			if (variable == null)
				throw new ArgumentNullException("variable");
			IType type = variable.Type;
			if (type.Kind == TypeKind.ByReference) {
				IParameter p = variable as IParameter;
				if (p != null && (p.IsRef || p.IsOut))
					return ((ByReferenceType)type).ElementType;
			}
			return type;
		}
		
		public IVariable Variable {
			get { return variable; }
		}
		
		public bool IsParameter {
			get { return variable is IParameter; }
		}
		
		public override bool IsCompileTimeConstant {
			get { return variable.IsConst; }
		}
		
		public override object ConstantValue {
			get { return IsParameter ? null : variable.ConstantValue; }
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[LocalResolveResult {0}]", variable);
		}
		
		public override DomRegion GetDefinitionRegion()
		{
			return variable.Region;
		}
	}
}
