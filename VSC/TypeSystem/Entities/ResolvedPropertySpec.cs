using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	public class ResolvedPropertySpec : ResolvedMemberSpec, IProperty
	{
		protected new readonly IUnresolvedProperty unresolved;
		readonly IList<IParameter> parameters;
		IMethod getter;
		IMethod setter;
		const Accessibility InvalidAccessibility = (Accessibility)0xff;
		volatile Accessibility cachedAccessiblity = InvalidAccessibility;
		
		public ResolvedPropertySpec(IUnresolvedProperty unresolved, ITypeResolveContext parentContext)
			: base(unresolved, parentContext)
		{
			this.unresolved = unresolved;
			this.parameters = unresolved.Parameters.CreateResolvedParameters(context);
		}
		
		public IList<IParameter> Parameters {
			get { return parameters; }
		}
		
		public override Accessibility Accessibility {
			get {
				var acc = cachedAccessiblity;
				if (acc == InvalidAccessibility)
					return cachedAccessiblity = ComputeAccessibility();
				else
					return acc;
			}
		}
		
		Accessibility ComputeAccessibility()
		{
			var baseAcc = base.Accessibility;
			if (IsOverride && !(CanGet && CanSet)) {
				foreach (var baseMember in InheritanceHelper.GetBaseMembers(this, false)) {
					if (!baseMember.IsOverride)
						return baseMember.Accessibility;
				}
			}
			return baseAcc;
		}
		
		public bool CanGet {
			get { return unresolved.CanGet; }
		}
		
		public bool CanSet {
			get { return unresolved.CanSet; }
		}
		
		public IMethod Getter {
			get { return GetAccessor(ref getter, unresolved.Getter); }
		}
		
		public IMethod Setter {
			get { return GetAccessor(ref setter, unresolved.Setter); }
		}
		
		public bool IsIndexer {
			get { return unresolved.IsIndexer; }
		}
		
		public override ISymbolReference ToReference()
		{
			var declType = this.DeclaringType;
			var declTypeRef = declType != null ? declType.ToTypeReference() : SpecialTypeSpec.UnknownType;
			if (IsExplicitInterfaceImplementation && ImplementedInterfaceMembers.Count == 1) {
				return new ExplicitInterfaceImplementationMemberReference(declTypeRef, ImplementedInterfaceMembers[0].ToReference());
			} else {
				return new MemberReferenceSpec(
					this.SymbolKind, declTypeRef, this.Name, 0,
					this.Parameters.Select(p => p.Type.ToTypeReference()).ToList());
			}
		}
		
		public override IMember Specialize(TypeParameterSubstitution substitution)
		{
			if (TypeParameterSubstitution.Identity.Equals(substitution)
			    || DeclaringTypeDefinition == null
			    || DeclaringTypeDefinition.TypeParameterCount == 0)
			{
				return this;
			}
			if (substitution.MethodTypeArguments != null && substitution.MethodTypeArguments.Count > 0)
				substitution = new TypeParameterSubstitution(substitution.ClassTypeArguments, EmptyList<IType>.Instance);
			return new SpecializedProperty(this, substitution);
		}
	}
}
