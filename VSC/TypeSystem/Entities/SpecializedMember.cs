using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a SpecializedMember (a member on which type substitution has been performed).
	/// </summary>
	public abstract class SpecializedMember : IMember
	{
		protected readonly IMember baseMember;
		TypeParameterSubstitution substitution;
		
		IType declaringType;
		IType returnType;
		
		protected SpecializedMember(IMember memberDefinition)
		{
			if (memberDefinition == null)
				throw new ArgumentNullException("memberDefinition");
			if (memberDefinition is SpecializedMember)
				throw new ArgumentException("Member definition cannot be specialized. Please use IMember.Specialize() instead of directly constructing SpecializedMember instances.");
			
			this.baseMember = memberDefinition;
			this.substitution = TypeParameterSubstitution.Identity;
		}
		
		/// <summary>
		/// Performs a substitution. This method may only be called by constructors in derived classes.
		/// </summary>
		protected void AddSubstitution(TypeParameterSubstitution newSubstitution)
		{
			Debug.Assert(declaringType == null);
			Debug.Assert(returnType == null);
			this.substitution = TypeParameterSubstitution.Compose(newSubstitution, this.substitution);
		}
		
		[Obsolete("Use IMember.Specialize() instead")]
		public static IMember Create(IMember memberDefinition, TypeParameterSubstitution substitution)
		{
			if (memberDefinition == null) {
				return null;
			} else {
				return memberDefinition.Specialize(substitution);
			}
		}
		
		public virtual IMemberReference ToMemberReference()
		{
			return ToReference();
		}
		
		public virtual IMemberReference ToReference()
		{
			return new SpecializingMemberReference(
				baseMember.ToReference(),
				ToTypeReference(substitution.ClassTypeArguments),
				null);
		}
		
		ISymbolReference ISymbol.ToReference()
		{
			return ToReference();
		}
		
		internal static IList<ITypeReference> ToTypeReference(IList<IType> typeArguments)
		{
			if (typeArguments == null)
				return null;
			else
				return typeArguments.Select(t => t.ToTypeReference()).ToArray();
		}
		
		internal IMethod WrapAccessor(ref IMethod cachingField, IMethod accessorDefinition)
		{
			if (accessorDefinition == null)
				return null;
			var result = LazyInit.VolatileRead(ref cachingField);
			if (result != null) {
				return result;
			} else {
				var sm = accessorDefinition.Specialize(substitution);
				//sm.AccessorOwner = this;
				return LazyInit.GetOrSet(ref cachingField, sm);
			}
		}
		
		/// <summary>
		/// Gets the substitution belonging to this specialized member.
		/// </summary>
		public TypeParameterSubstitution Substitution {
			get { return substitution; }
		}
		
		public IType DeclaringType {
			get {
				var result = LazyInit.VolatileRead(ref this.declaringType);
				if (result != null)
					return result;
				IType definitionDeclaringType = baseMember.DeclaringType;
				ITypeDefinition definitionDeclaringTypeDef = definitionDeclaringType as ITypeDefinition;
				if (definitionDeclaringTypeDef != null && definitionDeclaringType.TypeParameterCount > 0) {
					if (substitution.ClassTypeArguments != null && substitution.ClassTypeArguments.Count == definitionDeclaringType.TypeParameterCount) {
						result = new ParameterizedTypeSpec(definitionDeclaringTypeDef, substitution.ClassTypeArguments);
					} else {
						result = new ParameterizedTypeSpec(definitionDeclaringTypeDef, definitionDeclaringTypeDef.TypeParameters).AcceptVisitor(substitution);
					}
				} else if (definitionDeclaringType != null) {
					result = definitionDeclaringType.AcceptVisitor(substitution);
				}
				return LazyInit.GetOrSet(ref this.declaringType, result);
			}
			internal set {
				// This setter is used as an optimization when the code constructing
				// the SpecializedMember already knows the declaring type.
				Debug.Assert(this.declaringType == null);
				// As this setter is used only during construction before the member is published
				// to other threads, we don't need a volatile write.
				this.declaringType = value;
			}
		}
		
		public IMember MemberDefinition {
			get { return baseMember.MemberDefinition; }
		}
		
		public IUnresolvedMember UnresolvedMember {
			get { return baseMember.UnresolvedMember; }
		}
		
		public IType ReturnType {
			get {
				var result = LazyInit.VolatileRead(ref this.returnType);
				if (result != null)
					return result;
				else
					return LazyInit.GetOrSet(ref this.returnType, baseMember.ReturnType.AcceptVisitor(substitution));
			}
			protected set {
				// This setter is used for LiftedUserDefinedOperator, a special case of specialized member
				// (not a normal type parameter substitution).
				
				// As this setter is used only during construction before the member is published
				// to other threads, we don't need a volatile write.
				this.returnType = value;
			}
		}
		
		public bool IsVirtual {
			get { return baseMember.IsVirtual; }
		}
		
		public bool IsOverride {
			get { return baseMember.IsOverride; }
		}
		
		public bool IsOverridable {
			get { return baseMember.IsOverridable; }
		}
		
		public SymbolKind SymbolKind {
			get { return baseMember.SymbolKind; }
		}
		
		
		
		public DomRegion Region {
			get { return baseMember.Region; }
		}
		
		public DomRegion BodyRegion {
			get { return baseMember.BodyRegion; }
		}
		
		public ITypeDefinition DeclaringTypeDefinition {
			get { return baseMember.DeclaringTypeDefinition; }
		}
		
		public IList<IAttribute> Attributes {
			get { return baseMember.Attributes; }
		}
		
		IList<IMember> implementedInterfaceMembers;
		
		public IList<IMember> ImplementedInterfaceMembers {
			get {
				return LazyInitializer.EnsureInitialized(ref implementedInterfaceMembers, FindImplementedInterfaceMembers);
			}
		}
		
		IList<IMember> FindImplementedInterfaceMembers()
		{
			var definitionImplementations = baseMember.ImplementedInterfaceMembers;
			IMember[] result = new IMember[definitionImplementations.Count];
			for (int i = 0; i < result.Length; i++) {
				result[i] = definitionImplementations[i].Specialize(substitution);
			}
			return result;
		}
		
		public bool IsExplicitInterfaceImplementation {
			get { return baseMember.IsExplicitInterfaceImplementation; }
		}
	
		public Accessibility Accessibility {
			get { return baseMember.Accessibility; }
		}
		
		public bool IsStatic {
			get { return baseMember.IsStatic; }
		}
		
		public bool IsAbstract {
			get { return baseMember.IsAbstract; }
		}
		
		public bool IsSealed {
			get { return baseMember.IsSealed; }
		}
		
		public bool IsShadowing {
			get { return baseMember.IsShadowing; }
		}
		
		public bool IsSynthetic {
			get { return baseMember.IsSynthetic; }
		}
		
		public bool IsPrivate {
			get { return baseMember.IsPrivate; }
		}
		
		public bool IsPublic {
			get { return baseMember.IsPublic; }
		}
		
		public bool IsProtected {
			get { return baseMember.IsProtected; }
		}
		
		public bool IsInternal {
			get { return baseMember.IsInternal; }
		}
		
		public bool IsProtectedOrInternal {
			get { return baseMember.IsProtectedOrInternal; }
		}
		
		public bool IsProtectedAndInternal {
			get { return baseMember.IsProtectedAndInternal; }
		}
		
		public string FullName {
			get { return baseMember.FullName; }
		}
		
		public string Name {
			get { return baseMember.Name; }
		}
		
		public string Namespace {
			get { return baseMember.Namespace; }
		}
		
		public string ReflectionName {
			get { return baseMember.ReflectionName; }
		}
		
		public ICompilation Compilation {
			get { return baseMember.Compilation; }
		}
		
		public IAssembly ParentAssembly {
			get { return baseMember.ParentAssembly; }
		}

		public virtual IMember Specialize(TypeParameterSubstitution newSubstitution)
		{
			return baseMember.Specialize(TypeParameterSubstitution.Compose(newSubstitution, this.substitution));
		}

		public override bool Equals(object obj)
		{
			SpecializedMember other = obj as SpecializedMember;
			if (other == null)
				return false;
			return this.baseMember.Equals(other.baseMember) && this.substitution.Equals(other.substitution);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return 1000000007 * baseMember.GetHashCode() + 1000000009 * substitution.GetHashCode();
			}
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(GetType().Name);
			b.Append(' ');
			b.Append(this.DeclaringType.ToString());
			b.Append('.');
			b.Append(this.Name);
			b.Append(':');
			b.Append(this.ReturnType.ToString());
			b.Append(']');
			return b.ToString();
		}
	}
}
