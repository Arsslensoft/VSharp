using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem.Implementation;
using VSC.Base;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Anonymous type.
	/// </summary>
	public class AnonymousTypeSpec : TypeSpec
	{
		ICompilation compilation;
		IUnresolvedProperty[] unresolvedProperties;
		IList<IProperty> resolvedProperties;
		
		public AnonymousTypeSpec(ICompilation compilation, IList<IUnresolvedProperty> properties)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.compilation = compilation;
			this.unresolvedProperties = properties.ToArray();
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			this.resolvedProperties = new ProjectedList<ITypeResolveContext, IUnresolvedProperty, IProperty>(context, unresolvedProperties, (c, p) => new AnonymousTypeProperty(p, c, this));
		}
		
		sealed class AnonymousTypeProperty : ResolvedPropertySpec
		{
			readonly AnonymousTypeSpec declaringType;
			
			public AnonymousTypeProperty(IUnresolvedProperty unresolved, ITypeResolveContext parentContext, AnonymousTypeSpec declaringType)
				: base(unresolved, parentContext)
			{
				this.declaringType = declaringType;
			}
			
			public override IType DeclaringType {
				get { return declaringType; }
			}
			
			public override bool Equals(object obj)
			{
				AnonymousTypeProperty p = obj as AnonymousTypeProperty;
				return p != null && this.Name == p.Name && declaringType.Equals(p.declaringType);
			}
			
			public override int GetHashCode()
			{
				return declaringType.GetHashCode() ^ unchecked(27 * this.Name.GetHashCode());
			}
			
			protected override IMethod CreateResolvedAccessor(IUnresolvedMethod unresolvedAccessor)
			{
				return new AnonymousTypeAccessor(unresolvedAccessor, context, this);
			}
		}
		
		sealed class AnonymousTypeAccessor : ResolvedMethodSpec
		{
			readonly AnonymousTypeProperty owner;
			
			public AnonymousTypeAccessor(IUnresolvedMethod unresolved, ITypeResolveContext parentContext, AnonymousTypeProperty owner)
				: base(unresolved, parentContext, isExtensionMethod: false)
			{
				this.owner = owner;
			}
			
			public override IMember AccessorOwner {
				get { return owner; }
			}
			
			public override IType DeclaringType {
				get { return owner.DeclaringType; }
			}
			
			public override bool Equals(object obj)
			{
				AnonymousTypeAccessor p = obj as AnonymousTypeAccessor;
				return p != null && this.Name == p.Name && owner.DeclaringType.Equals(p.owner.DeclaringType);
			}
			
			public override int GetHashCode()
			{
				return owner.DeclaringType.GetHashCode() ^ unchecked(27 * this.Name.GetHashCode());
			}
		}
		
		public override ITypeReference ToTypeReference()
		{
			return new AnonymousTypeReference(unresolvedProperties);
		}
		
		public override string Name {
			get { return "Anonymous Type"; }
		}
		
		public override TypeKind Kind {
			get { return TypeKind.Anonymous; }
		}

		public override IEnumerable<IType> DirectBaseTypes {
			get {
				yield return compilation.FindType(KnownTypeCode.Object);
			}
		}
		
		public override bool? IsReferenceType {
			get { return true; }
		}
		
		public IList<IProperty> Properties {
			get { return resolvedProperties; }
		}
		
		public override IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return compilation.FindType(KnownTypeCode.Object).GetMethods(filter, options);
		}
		
		public override IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return compilation.FindType(KnownTypeCode.Object).GetMethods(typeArguments, filter, options);
		}
		
		public override IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			for (int i = 0; i < unresolvedProperties.Length; i++) {
				if (filter == null || filter(unresolvedProperties[i]))
					yield return resolvedProperties[i];
			}
		}
		
		public override IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter, GetMemberOptions options)
		{
			for (int i = 0; i < unresolvedProperties.Length; i++) {
				if (unresolvedProperties[i].CanGet) {
					if (filter == null || filter(unresolvedProperties[i].Getter))
						yield return resolvedProperties[i].Getter;
				}
				if (unresolvedProperties[i].CanSet) {
					if (filter == null || filter(unresolvedProperties[i].Setter))
						yield return resolvedProperties[i].Setter;
				}
			}
		}
		
		public override int GetHashCode()
		{
			unchecked {
				int hashCode = resolvedProperties.Count;
				foreach (var p in resolvedProperties) {
					hashCode *= 31;
					hashCode += p.Name.GetHashCode() ^ p.ReturnType.GetHashCode();
				}
				return hashCode;
			}
		}
		
		public override bool Equals(IType other)
		{
			AnonymousTypeSpec o = other as AnonymousTypeSpec;
			if (o == null || resolvedProperties.Count != o.resolvedProperties.Count)
				return false;
			for (int i = 0; i < resolvedProperties.Count; i++) {
				IProperty p1 = resolvedProperties[i];
				IProperty p2 = o.resolvedProperties[i];
				if (p1.Name != p2.Name || !p1.ReturnType.Equals(p2.ReturnType))
					return false;
			}
			return true;
		}
	}
}
