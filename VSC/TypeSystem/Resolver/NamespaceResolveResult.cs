using System;
using System.Globalization;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents that an expression resolved to a namespace.
	/// </summary>
	public class NamespaceResolveResult : ResolveResult
	{
		readonly INamespace ns;
		
		public NamespaceResolveResult(INamespace ns) : base(SpecialTypeSpec.UnknownType)
		{
			this.ns = ns;
		}
		
		public INamespace Namespace {
			get { return ns; }
		}
		
		public string NamespaceName {
			get { return ns.FullName; }
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, ns);
		}
	}
}
