using System;
using System.Collections.Generic;

namespace VSC.TypeSystem.Implementation
{
	[Serializable]
	public sealed class SpecializingMemberReference : IMemberReference
	{
		IMemberReference memberDefinitionReference;
		IList<ITypeReference> classTypeArgumentReferences;
		IList<ITypeReference> methodTypeArgumentReferences;
		
		public SpecializingMemberReference(IMemberReference memberDefinitionReference, IList<ITypeReference> classTypeArgumentReferences = null, IList<ITypeReference> methodTypeArgumentReferences = null)
		{
			if (memberDefinitionReference == null)
				throw new ArgumentNullException("memberDefinitionReference");
			this.memberDefinitionReference = memberDefinitionReference;
			this.classTypeArgumentReferences = classTypeArgumentReferences;
			this.methodTypeArgumentReferences = methodTypeArgumentReferences;
		}
		
		public IMember Resolve(ITypeResolveContext context)
		{
			var memberDefinition = memberDefinitionReference.Resolve(context);
			if (memberDefinition == null)
				return null;
			return memberDefinition.Specialize(
				new TypeParameterSubstitution(
					classTypeArgumentReferences != null ? classTypeArgumentReferences.Resolve(context) : null,
					methodTypeArgumentReferences != null ? methodTypeArgumentReferences.Resolve(context) : null
				)
			);
		}
		
		ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
		{
			return Resolve(context);
		}
		
		public ITypeReference DeclaringTypeReference {
			get {
				if (classTypeArgumentReferences != null)
					return new ParameterizedTypeReference(memberDefinitionReference.DeclaringTypeReference, classTypeArgumentReferences);
				else
					return memberDefinitionReference.DeclaringTypeReference;
			}
		}
	}
}
