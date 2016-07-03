using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the resolve result of an 'ref x' or 'out x' expression.
	/// </summary>
	public class ByReferenceResolveResult : ResolveResult
	{
		public bool IsOut { get; private set; }
		public bool IsRef { get { return !IsOut;} }
		
		public readonly ResolveResult ElementResult;
		
		public ByReferenceResolveResult(ResolveResult elementResult, bool isOut)
			: this(elementResult.Type, isOut)
		{
			this.ElementResult = elementResult;
		}
		
		public ByReferenceResolveResult(IType elementType, bool isOut)
			: base(new ByReferenceType(elementType))
		{
			this.IsOut = isOut;
		}
		
		public IType ElementType {
			get { return ((ByReferenceType)this.Type).ElementType; }
		}
		
		public override IEnumerable<ResolveResult> GetChildResults()
		{
			if (ElementResult != null)
				return new[] { ElementResult };
			else
				return Enumerable.Empty<ResolveResult>();
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1} {2}]", GetType().Name, IsOut ? "out" : "ref", ElementType);
		}
	}
}
