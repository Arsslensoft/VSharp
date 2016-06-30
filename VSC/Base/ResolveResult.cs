using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;

namespace VSC.Base
{
	/// <summary>
	/// Represents the result of resolving an expression.
	/// </summary>
	public class ResolveResult
	{
		internal IType type;
		
		public ResolveResult(IType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.type = type;
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
		                                                 Justification = "Unrelated to object.GetType()")]
		public IType Type {
			get { return type; }
		}
		
		public virtual bool IsCompileTimeConstant {
			get { return false; }
		}
		
		public virtual object ConstantValue {
			get { return null; }
		}
		
		public virtual bool IsError {
			get { return false; }
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + type + "]";
		}
		
		public virtual IEnumerable<ResolveResult> GetChildResults()
		{
			return Enumerable.Empty<ResolveResult>();
		}
		
		public virtual DomRegion GetDefinitionRegion()
		{
			return DomRegion.Empty;
		}
		
		public virtual ResolveResult ShallowClone()
		{
			return (ResolveResult)MemberwiseClone();
		}
	}
}
