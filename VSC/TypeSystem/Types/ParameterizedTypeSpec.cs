using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem
{
	/// <summary>
	/// ParameterizedTypeSpec represents an instance of a generic type.
	/// Example: List&lt;string&gt;
	/// </summary>
	/// <remarks>
	/// When getting the members, this type modifies the lists so that
	/// type parameters in the signatures of the members are replaced with
	/// the type arguments.
	/// </remarks>
	[Serializable]
    public sealed class ParameterizedTypeSpec : IType, IEntity, ICompilationProvider
	{
		readonly ITypeDefinition genericType;
		readonly IType[] typeArguments;
		
		public ParameterizedTypeSpec(ITypeDefinition genericType, IEnumerable<IType> typeArguments)
		{
			if (genericType == null)
				throw new ArgumentNullException("genericType");
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.genericType = genericType;
			this.typeArguments = typeArguments.ToArray(); // copy input array to ensure it isn't modified
			if (this.typeArguments.Length == 0)
				throw new ArgumentException("Cannot use ParameterizedTypeSpec with 0 type arguments.");
			if (genericType.TypeParameterCount != this.typeArguments.Length)
				throw new ArgumentException("Number of type arguments must match the type definition's number of type parameters");
			for (int i = 0; i < this.typeArguments.Length; i++) {
				if (this.typeArguments[i] == null)
					throw new ArgumentNullException("typeArguments[" + i + "]");
				ICompilationProvider p = this.typeArguments[i] as ICompilationProvider;
				if (p != null && p.Compilation != genericType.Compilation)
					throw new InvalidOperationException("Cannot parameterize a type with type arguments from a different compilation.");
			}
		}
		
		/// <summary>
		/// Fast internal version of the constructor. (no safety checks)
		/// Keeps the array that was passed and assumes it won't be modified.
		/// </summary>
		internal ParameterizedTypeSpec(ITypeDefinition genericType, IType[] typeArguments)
		{
			Debug.Assert(genericType.TypeParameterCount == typeArguments.Length);
			this.genericType = genericType;
			this.typeArguments = typeArguments;
		}
		
		public TypeKind Kind {
			get { return genericType.Kind; }
		}
		
		public ICompilation Compilation {
			get { return genericType.Compilation; }
		}
		
		public bool? IsReferenceType {
			get { return genericType.IsReferenceType; }
		}
		
		public IType DeclaringType {
			get {
				ITypeDefinition declaringTypeDef = genericType.DeclaringTypeDefinition;
				if (declaringTypeDef != null && declaringTypeDef.TypeParameterCount > 0
				    && declaringTypeDef.TypeParameterCount <= genericType.TypeParameterCount)
				{
					IType[] newTypeArgs = new IType[declaringTypeDef.TypeParameterCount];
					Array.Copy(this.typeArguments, 0, newTypeArgs, 0, newTypeArgs.Length);
					return new ParameterizedTypeSpec(declaringTypeDef, newTypeArgs);
				}
				return declaringTypeDef;
			}
		}
		
		public int TypeParameterCount {
			get { return typeArguments.Length; }
		}
		
		public string FullName {
			get { return genericType.FullName; }
		}
		
		public string Name {
			get { return genericType.Name; }
		}
		
		public string Namespace {
			get { return genericType.Namespace; }
		}
		
		public string ReflectionName {
			get {
				StringBuilder b = new StringBuilder(genericType.ReflectionName);
				b.Append('[');
				for (int i = 0; i < typeArguments.Length; i++) {
					if (i > 0)
						b.Append(',');
					b.Append('[');
					b.Append(typeArguments[i].ReflectionName);
					b.Append(']');
				}
				b.Append(']');
				return b.ToString();
			}
		}
		
		public override string ToString()
		{
			return ReflectionName;
		}
		
		public IList<IType> TypeArguments {
			get {
				return typeArguments;
			}
		}

		public bool IsParameterized { 
			get {
				return true;
			}
		}

		/// <summary>
		/// Same as 'parameterizedType.TypeArguments[index]', but is a bit more efficient (doesn't require the read-only wrapper).
		/// </summary>
		public IType GetTypeArgument(int index)
		{
			return typeArguments[index];
		}
		
		/// <summary>
		/// Gets the definition of the generic type.
		/// For <c>ParameterizedTypeSpec</c>, this method never returns null.
		/// </summary>
		public ITypeDefinition GetDefinition()
		{
			return genericType;
		}
		
		public ITypeReference ToTypeReference()
		{
			return new ParameterizedTypeReference(genericType.ToTypeReference(), typeArguments.Select(t => t.ToTypeReference()));
		}
		
		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type.
		/// </summary>
		public TypeParameterSubstitution GetSubstitution()
		{
			return new TypeParameterSubstitution(typeArguments, null);
		}
		
		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type,
		/// and also substitutes method type parameters with the specified method type arguments.
		/// </summary>
		public TypeParameterSubstitution GetSubstitution(IList<IType> methodTypeArguments)
		{
			return new TypeParameterSubstitution(typeArguments, methodTypeArguments);
		}
		
		public IEnumerable<IType> DirectBaseTypes {
			get {
				var substitution = GetSubstitution();
				return genericType.DirectBaseTypes.Select(t => t.AcceptVisitor(substitution));
			}
		}


        public IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
                return genericType.GetNestedTypes(filter, options);
            else
                return GetMembersHelper.GetNestedTypes(this, filter, options);
        }

        public IEnumerable<IType> GetNestedTypes(IList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
                return genericType.GetNestedTypes(typeArguments, filter, options);
            else
                return GetMembersHelper.GetNestedTypes(this, typeArguments, filter, options);
        }
		
		public IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetConstructors(filter, options);
			else
				return GetMembersHelper.GetConstructors(this, filter, options);
		}
		
		public IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetMethods(filter, options);
			else
				return GetMembersHelper.GetMethods(this, filter, options);
		}
		
		public IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetMethods(typeArguments, filter, options);
			else
				return GetMembersHelper.GetMethods(this, typeArguments, filter, options);
		}
		
		public IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetProperties(filter, options);
			else
				return GetMembersHelper.GetProperties(this, filter, options);
		}
		
		public IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetFields(filter, options);
			else
				return GetMembersHelper.GetFields(this, filter, options);
		}
		
		public IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetEvents(filter, options);
			else
				return GetMembersHelper.GetEvents(this, filter, options);
		}
		
		public IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetMembers(filter, options);
			else
				return GetMembersHelper.GetMembers(this, filter, options);
		}
		
		public IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
				return genericType.GetAccessors(filter, options);
			else
				return GetMembersHelper.GetAccessors(this, filter, options);
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as IType);
		}
		
		public bool Equals(IType other)
		{
			ParameterizedTypeSpec c = other as ParameterizedTypeSpec;
			if (c == null || !genericType.Equals(c.genericType) || typeArguments.Length != c.typeArguments.Length)
				return false;
			for (int i = 0; i < typeArguments.Length; i++) {
				if (!typeArguments[i].Equals(c.typeArguments[i]))
					return false;
			}
			return true;
		}
		
		public override int GetHashCode()
		{
			int hashCode = genericType.GetHashCode();
			unchecked {
				foreach (var ta in typeArguments) {
					hashCode *= 1000000007;
					hashCode += 1000000009 * ta.GetHashCode();
				}
			}
			return hashCode;
		}
		
		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitParameterizedType(this);
		}
		
		public IType VisitChildren(TypeVisitor visitor)
		{
			IType g = genericType.AcceptVisitor(visitor);
			ITypeDefinition def = g as ITypeDefinition;
			if (def == null)
				return g;
			// Keep ta == null as long as no elements changed, allocate the array only if necessary.
			IType[] ta = (g != genericType) ? new IType[typeArguments.Length] : null;
			for (int i = 0; i < typeArguments.Length; i++) {
				IType r = typeArguments[i].AcceptVisitor(visitor);
				if (r == null)
					throw new NullReferenceException("TypeVisitor.Visit-method returned null");
				if (ta == null && r != typeArguments[i]) {
					// we found a difference, so we need to allocate the array
					ta = new IType[typeArguments.Length];
					for (int j = 0; j < i; j++) {
						ta[j] = typeArguments[j];
					}
				}
				if (ta != null)
					ta[i] = r;
			}
			if (def == genericType && ta == null)
				return this;
			else
				return new ParameterizedTypeSpec(def, ta ?? typeArguments);
		}

#region IEntity
        public DomRegion Region
        {
            get { return genericType.Region; }
        }

        public DomRegion BodyRegion
        {
            get { return genericType.BodyRegion; }
        }

        public ITypeDefinition DeclaringTypeDefinition
        {
            get { return genericType.DeclaringTypeDefinition; }
        }

        public IAssembly ParentAssembly
        {
            get { return genericType.ParentAssembly; }
        }

        public IList<IAttribute> Attributes
        {
            get { return genericType.Attributes; }
        }

        public bool IsStatic
        {
            get { return genericType.IsStatic; }
        }

        public bool IsAbstract
        {
            get { return genericType.IsAbstract; }
        }

        public bool IsSealed
        {
            get { return genericType.IsSealed; }
        }

        public bool IsShadowing
        {
            get { return genericType.IsShadowing; }
        }

        public bool IsSynthetic
        {
            get { return genericType.IsSynthetic; }
        }

        public bool IsBaseTypeDefinition(IType baseType)
        {
           return genericType.IsBaseTypeDefinition(baseType);
        }

        public bool IsAccessibleAs(IType b)
        {
            return genericType.IsAccessibleAs(b);
        }

        public bool IsInternalAccessible(IAssembly asm)
        {
            return genericType.IsInternalAccessible(asm);
        }

        public SymbolKind SymbolKind
        {
            get { return genericType.SymbolKind; }
        }

        public ISymbolReference ToReference()
        {
            return genericType.ToReference();
        }

        public Accessibility Accessibility
        {
            get { return genericType.Accessibility; }
        }

        public bool IsPrivate
        {
            get { return genericType.IsPrivate; }
        }

        public bool IsPublic
        {
            get { return genericType.IsPublic; }
        }

        public bool IsProtected
        {
            get { return genericType.IsProtected; }
        }

        public bool IsInternal
        {
            get { return genericType.IsInternal; }
        }

        public bool IsProtectedOrInternal
        {
            get { return genericType.IsProtectedOrInternal; }
        }

#endregion
    }
}
