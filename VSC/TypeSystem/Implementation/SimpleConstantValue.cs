using System;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// A simple constant value that is independent of the resolve context.
	/// </summary>
	[Serializable]
	public sealed class SimpleConstantValue : IConstantValue, ISupportsInterning
	{
		readonly ITypeReference type;
		readonly object value;
		
		public SimpleConstantValue(ITypeReference type, object value)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.type = type;
			this.value = value;
		}
		
		public Expression Resolve(ITypeResolveContext context)
		{
			//return new ConstantResolveResult(type.ResolveScope(context), value); //TODO:Fix this
            return null;
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
		                                                 Justification = "The V# keyword is lower case")]
		public override string ToString()
		{
			if (value == null)
				return "null";
			else if (value is bool)
				return value.ToString().ToLowerInvariant();
			else
				return value.ToString();
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return type.GetHashCode() ^ (value != null ? value.GetHashCode() : 0);
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			SimpleConstantValue scv = other as SimpleConstantValue;
			return scv != null && type == scv.type && value == scv.value;
		}
	}
}
