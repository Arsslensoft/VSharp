using System;
using VSC.Base;


namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the 'typeof'.
	/// </summary>
	public class TypeOfResolveResult : ResolveResult
	{
		readonly IType referencedType;
		
		public TypeOfResolveResult(IType systemType, IType referencedType)
			: base(systemType)
		{
			if (referencedType == null)
				throw new ArgumentNullException("referencedType");
			this.referencedType = referencedType;
		}
		
		/// <summary>
		/// The type referenced by the 'typeof'.
		/// </summary>
		public IType ReferencedType {
			get { return referencedType; }
		}
	}
}
