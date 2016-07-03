using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Given a reference to an accessor, returns the accessor's owner.
	/// </summary>
	[Serializable]
	sealed class AccessorOwnerMemberReference : IMemberReference
	{
		readonly IMemberReference accessorReference;
		
		public AccessorOwnerMemberReference(IMemberReference accessorReference)
		{
			if (accessorReference == null)
				throw new ArgumentNullException("accessorReference");
			this.accessorReference = accessorReference;
		}
		
		public ITypeReference DeclaringTypeReference {
			get { return accessorReference.DeclaringTypeReference; }
		}
		
		public IMember Resolve(ITypeResolveContext context)
		{
			IMethod method = accessorReference.Resolve(context) as IMethod;
			if (method != null)
				return method.AccessorOwner;
			else
				return null;
		}
		
		ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
		{
			return ((IMemberReference)this).Resolve(context);
		}
	}
}
