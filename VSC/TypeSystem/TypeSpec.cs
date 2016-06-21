using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem.Interfaces;

namespace VSC.TypeSystem
{

    /// <summary>
    /// Default implementation for IType interface.
    /// </summary>
    [Serializable]
    public abstract class TypeSpec : IType
    {
        public virtual string FullName
        {
            get
            {
                string ns = this.Namespace;
                string name = this.Name;
                if (string.IsNullOrEmpty(ns))
                {
                    return name;
                }
                else
                {
                    return ns + "." + name;
                }
            }
        }

        public abstract string Name { get; }

        public virtual string Namespace
        {
            get { return string.Empty; }
        }

        public virtual string ReflectionName
        {
            get { return this.FullName; }
        }

        public abstract bool? IsReferenceType { get; }

        public abstract TypeKind Kind { get; }

        public virtual int TypeParameterCount
        {
            get { return 0; }
        }

        readonly static IList<IType> emptyTypeArguments = new IType[0];
        public virtual IList<IType> TypeArguments
        {
            get { return emptyTypeArguments; }
        }

        public virtual IType DeclaringType
        {
            get { return null; }
        }

        public virtual bool IsParameterized
        {
            get { return false; }
        }

        public virtual ITypeDefinition GetDefinition()
        {
            return null;
        }

        public virtual IEnumerable<IType> DirectBaseTypes
        {
            get { return EmptyList<IType>.Instance; }
        }

        public abstract ITypeReference ToTypeReference();

        public virtual IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            return EmptyList<IMethod>.Instance;
        }

        public virtual IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            return EmptyList<IMethod>.Instance;
        }

        public virtual IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
        {
            return EmptyList<IMethod>.Instance;
        }

        public virtual IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            return EmptyList<IProperty>.Instance;
        }

        public virtual IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            return EmptyList<IField>.Instance;
        }

        public virtual IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            return EmptyList<IEvent>.Instance;
        }

        public virtual IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            IEnumerable<IMember> members = GetMethods(filter, options);
            return members
                .Concat(GetProperties(filter, options))
                .Concat(GetFields(filter, options))
                .Concat(GetEvents(filter, options));
        }
        public virtual IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            return EmptyList<IMethod>.Instance;
        }


        public override sealed bool Equals(object obj)
        {
            return Equals(obj as IType);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool Equals(IType other)
        {
            return this == other; // use reference equality by default
        }

        public override string ToString()
        {
            return this.ReflectionName;
        }

    }
    [Serializable]
    public abstract class TypeReferenceSpec : ITypeReference, ISupportsInterning
    {
        public virtual IType Resolve(ITypeResolveContext context)
        {
            throw new NotSupportedException();
        }


    public    virtual int GetHashCodeForInterning()
        {
            throw new NotSupportedException();
        }

    public virtual bool EqualsForInterning(ISupportsInterning other)
        {
            throw new NotSupportedException();
        }
    }
    public abstract class ElementTypeSpec : TypeSpec
    {
        [CLSCompliant(false)]
		protected IType elementType;

        protected ElementTypeSpec(IType elementType)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			this.elementType = elementType;
		}
		
		public override string Name {
			get { return elementType.Name + NameSuffix; }
		}
		
		public override string Namespace {
			get { return elementType.Namespace; }
		}
		
		public override string FullName {
			get { return elementType.FullName + NameSuffix; }
		}
		
		public override string ReflectionName {
			get { return elementType.ReflectionName + NameSuffix; }
		}
		
		public abstract string NameSuffix { get; }
		
		public IType ElementType {
			get { return elementType; }
		}
    }
    public sealed class PointerTypeSpec : ElementTypeSpec
    {
        public PointerTypeSpec(IType elementType)
            : base(elementType)
        {
        }

        public override TypeKind Kind
        {
            get { return TypeKind.Pointer; }
        }

        public override string NameSuffix
        {
            get
            {
                return "*";
            }
        }

        public override bool? IsReferenceType
        {
            get { return null; }
        }

        public override int GetHashCode()
        {
            return elementType.GetHashCode() ^ 91725811;
        }

        public override bool Equals(IType other)
        {
            PointerTypeSpec a = other as PointerTypeSpec;
            return a != null && elementType.Equals(a.elementType);
        }
 
        public override ITypeReference ToTypeReference()
        {
            return new PointerTypeReferenceSpec(elementType.ToTypeReference());
        }
    }
    public sealed class NullableTypeSpec : ElementTypeSpec
    {
        public NullableTypeSpec(IType elementType)
            : base(elementType)
        {
        }

        public override TypeKind Kind
        {
            get { return TypeKind.ByReference; }
        }

        public override string NameSuffix
        {
            get
            {
                return "?";
            }
        }

        public override bool? IsReferenceType
        {
            get { return true; }
        }

        public override int GetHashCode()
        {
            return elementType.GetHashCode() ^ 91725813;
        }

        public override bool Equals(IType other)
        {
            NullableTypeSpec a = other as NullableTypeSpec;
            return a != null && elementType.Equals(a.elementType);
        }

        public override ITypeReference ToTypeReference()
        {
            return new NullableTypeReferenceSpec(elementType.ToTypeReference());
        }
    }
    /// <summary>
    /// Represents an array type.
    /// </summary>
    public sealed class ArrayTypeSpec : ElementTypeSpec, ICompilationProvider
    {
        readonly int dimensions;
        readonly ICompilation compilation;

        public ArrayTypeSpec(ICompilation compilation, IType elementType, int dimensions = 1)
            : base(elementType)
        {
            if (compilation == null)
                throw new ArgumentNullException("compilation");
            if (dimensions <= 0)
                throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
            this.compilation = compilation;
            this.dimensions = dimensions;

            ICompilationProvider p = elementType as ICompilationProvider;
            if (p != null && p.Compilation != compilation)
                throw new InvalidOperationException("Cannot create an array type using a different compilation from the element type.");
        }

        public override TypeKind Kind
        {
            get { return TypeKind.Array; }
        }

        public ICompilation Compilation
        {
            get { return compilation; }
        }

        public int Dimensions
        {
            get { return dimensions; }
        }

        public override string NameSuffix
        {
            get
            {
                return "[" + new string(',', dimensions - 1) + "]";
            }
        }

        public override bool? IsReferenceType
        {
            get { return true; }
        }

        public override int GetHashCode()
        {
            return unchecked(elementType.GetHashCode() * 71681 + dimensions);
        }

        public override bool Equals(IType other)
        {
            ArrayTypeSpec a = other as ArrayTypeSpec;
            return a != null && elementType.Equals(a.elementType) && a.dimensions == dimensions;
        }

        public override ITypeReference ToTypeReference()
        {
            return new ArrayTypeReferenceSpec(elementType.ToTypeReference(), dimensions);
        }
    }
    /// <summary>
    /// ParameterizedType represents an instance of a generic type.
    /// Example: List&lt;string&gt;
    /// </summary>
    /// <remarks>
    /// When getting the members, this type modifies the lists so that
    /// type parameters in the signatures of the members are replaced with
    /// the type arguments.
    /// </remarks>
    [Serializable]
    public sealed class ParameterizedTypeSpec : TypeSpec, ICompilationProvider
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
                throw new ArgumentException("Cannot use ParameterizedType with 0 type arguments.");
            if (genericType.TypeParameterCount != this.typeArguments.Length)
                throw new ArgumentException("Number of type arguments must match the type definition's number of type parameters");
            for (int i = 0; i < this.typeArguments.Length; i++)
            {
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

        public override TypeKind Kind
        {
            get { return genericType.Kind; }
        }

        public ICompilation Compilation
        {
            get { return genericType.Compilation; }
        }



        public override bool? IsReferenceType
        {
            get { return genericType.IsReferenceType; }
        }

        public override IType DeclaringType
        {
            get
            {
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

        public override int TypeParameterCount
        {
            get { return typeArguments.Length; }
        }

        public override string FullName
        {
            get { return genericType.FullName; }
        }

        public override string Name
        {
            get { return genericType.Name; }
        }

        public override string Namespace
        {
            get { return genericType.Namespace; }
        }

        public override string ReflectionName
        {
            get
            {
                StringBuilder b = new StringBuilder(genericType.ReflectionName);
                b.Append('[');
                for (int i = 0; i < typeArguments.Length; i++)
                {
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

        public override IList<IType> TypeArguments
        {
            get
            {
                return typeArguments;
            }
        }

        public override bool IsParameterized
        {
            get
            {
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
        /// For <c>ParameterizedType</c>, this method never returns null.
        /// </summary>
        public override ITypeDefinition GetDefinition()
        {
            return genericType;
        }

        public override ITypeReference ToTypeReference()
        {
            return new ParameterizedTypeReference(genericType.ToTypeReference(), typeArguments.Select(t => t.ToTypeReference()));
        }



        public override IEnumerable<IType> DirectBaseTypes
        {
            get
            {
                return genericType.DirectBaseTypes;
            }
        }


        public override IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetConstructors(filter, options);
            //else
            //    return GetMembersHelper.GetConstructors(this, filter, options);
            throw new NotSupportedException();
        }

        public override IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetMethods(filter, options);
            //else
            //    return GetMembersHelper.GetMethods(this, filter, options);
            throw new NotSupportedException();
        }

        public override IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetMethods(typeArguments, filter, options);
            //else
            //    return GetMembersHelper.GetMethods(this, typeArguments, filter, options);

            throw new NotSupportedException();
        }

        public override IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetProperties(filter, options);
            //else
            //    return GetMembersHelper.GetProperties(this, filter, options);

            throw new NotSupportedException();
        }

        public override IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetFields(filter, options);
            //else
            //    return GetMembersHelper.GetFields(this, filter, options);

            throw new NotSupportedException();
        }

        public override IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetEvents(filter, options);
            //else
            //    return GetMembersHelper.GetEvents(this, filter, options);

            throw new NotSupportedException();
        }

        public override IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetMembers(filter, options);
            //else
            //    return GetMembersHelper.GetMembers(this, filter, options);

            throw new NotSupportedException();
        }

        public override IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
        {
            //if ((options & GetMemberOptions.ReturnMemberDefinitions) == GetMemberOptions.ReturnMemberDefinitions)
            //    return genericType.GetAccessors(filter, options);
            //else
            //    return GetMembersHelper.GetAccessors(this, filter, options);

            throw new NotSupportedException();
        }


        public override bool Equals(IType other)
        {
            ParameterizedTypeSpec c = other as ParameterizedTypeSpec;
            if (c == null || !genericType.Equals(c.genericType) || typeArguments.Length != c.typeArguments.Length)
                return false;
            for (int i = 0; i < typeArguments.Length; i++)
            {
                if (!typeArguments[i].Equals(c.typeArguments[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = genericType.GetHashCode();
            unchecked
            {
                foreach (var ta in typeArguments)
                {
                    hashCode *= 1000000007;
                    hashCode += 1000000009 * ta.GetHashCode();
                }
            }
            return hashCode;
        }

 
    }
    /// <summary>
    /// Contains static implementations of special types.
    /// </summary>
    [Serializable]
    public sealed class SpecialTypeSpec : TypeSpec, ITypeReference
    {
        /// <summary>
        /// Gets the type representing resolve errors.
        /// </summary>
        public readonly static SpecialTypeSpec UnknownType = new SpecialTypeSpec(TypeKind.Unknown, "?", isReferenceType: null);

        /// <summary>
        /// The null type is used as type of the null literal. It is a reference type without any members; and it is a subtype of all reference types.
        /// </summary>
        public readonly static SpecialTypeSpec NullType = new SpecialTypeSpec(TypeKind.Null, "null", isReferenceType: true);


        /// <summary>
        /// A type used for unbound type arguments in partially parameterized types.
        /// </summary>
        /// <see cref="IType.GetNestedTypes(Predicate{ITypeDefinition}, GetMemberOptions)"/>
        public readonly static SpecialTypeSpec UnboundTypeArgument = new SpecialTypeSpec(TypeKind.UnboundTypeArgument, "", isReferenceType: null);

        readonly TypeKind kind;
        readonly string name;
        readonly bool? isReferenceType;

        private SpecialTypeSpec(TypeKind kind, string name, bool? isReferenceType)
        {
            this.kind = kind;
            this.name = name;
            this.isReferenceType = isReferenceType;
        }

        public override ITypeReference ToTypeReference()
        {
            return this;
        }

        public override string Name
        {
            get { return name; }
        }

        public override TypeKind Kind
        {
            get { return kind; }
        }

        public override bool? IsReferenceType
        {
            get { return isReferenceType; }
        }

        IType ITypeReference.Resolve(ITypeResolveContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            return this;
        }

#pragma warning disable 809
        [Obsolete("Please compare special types using the kind property instead.")]
        public override bool Equals(IType other)
        {
            // We consider a special types equal when they have equal types.
            // However, an unknown type with additional information is not considered to be equal to the SpecialType with TypeKind.Unknown.
            return other is SpecialTypeSpec && other.Kind == kind;
        }

        public override int GetHashCode()
        {
            return 81625621 ^ (int)kind;
        }
    }

    
    
    /// <summary>
    /// ParameterizedTypeReference is a reference to generic class that specifies the type parameters.
    /// Example: List&lt;string&gt;
    /// </summary>
    [Serializable]
    public sealed class ParameterizedTypeReference : TypeReferenceSpec
    {
        readonly ITypeReference genericType;
        readonly ITypeReference[] typeArguments;

        public ParameterizedTypeReference(ITypeReference genericType, IEnumerable<ITypeReference> typeArguments)
        {
            if (genericType == null)
                throw new ArgumentNullException("genericType");
            if (typeArguments == null)
                throw new ArgumentNullException("typeArguments");
            this.genericType = genericType;
            this.typeArguments = typeArguments.ToArray();
            for (int i = 0; i < this.typeArguments.Length; i++)
            {
                if (this.typeArguments[i] == null)
                    throw new ArgumentNullException("typeArguments[" + i + "]");
            }
        }

        public ITypeReference GenericType
        {
            get { return genericType; }
        }

        public ReadOnlyCollection<ITypeReference> TypeArguments
        {
            get
            {
                return Array.AsReadOnly(typeArguments);
            }
        }

        public IType Resolve(ITypeResolveContext context)
        {
            IType baseType = genericType.Resolve(context);
            ITypeDefinition baseTypeDef = baseType.GetDefinition();
            if (baseTypeDef == null)
                return baseType;
            int tpc = baseTypeDef.TypeParameterCount;
            if (tpc == 0)
                return baseTypeDef;
            IType[] resolvedTypes = new IType[tpc];
            for (int i = 0; i < resolvedTypes.Length; i++)
            {
                if (i < typeArguments.Length)
                    resolvedTypes[i] = typeArguments[i].Resolve(context);
                else
                    resolvedTypes[i] = SpecialTypeSpec.UnknownType;
            }
            return new ParameterizedTypeSpec(baseTypeDef, resolvedTypes);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder(genericType.ToString());
            b.Append('[');
            for (int i = 0; i < typeArguments.Length; i++)
            {
                if (i > 0)
                    b.Append(',');
                b.Append('[');
                b.Append(typeArguments[i].ToString());
                b.Append(']');
            }
            b.Append(']');
            return b.ToString();
        }

        public override int GetHashCodeForInterning()
        {
            int hashCode = genericType.GetHashCode();
            unchecked
            {
                foreach (ITypeReference t in typeArguments)
                {
                    hashCode *= 27;
                    hashCode += t.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool EqualsForInterning(ISupportsInterning other)
        {
            ParameterizedTypeReference o = other as ParameterizedTypeReference;
            if (o != null && genericType == o.genericType && typeArguments.Length == o.typeArguments.Length)
            {
                for (int i = 0; i < typeArguments.Length; i++)
                {
                    if (typeArguments[i] != o.typeArguments[i])
                        return false;
                }
                return true;
            }
            return false;
        }
    }
    [Serializable]
    public sealed class PointerTypeReferenceSpec : TypeReferenceSpec
    {
        readonly ITypeReference elementType;

        public PointerTypeReferenceSpec(ITypeReference elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            this.elementType = elementType;
        }

        public ITypeReference ElementType
        {
            get { return elementType; }
        }

        public IType Resolve(ITypeResolveContext context)
        {
            return new PointerTypeSpec(elementType.Resolve(context));
        }

        public override string ToString()
        {
            return elementType.ToString() + "*";
        }

        public override int GetHashCodeForInterning()
        {
            return elementType.GetHashCode() ^ 91725812;
        }

        public override bool EqualsForInterning(ISupportsInterning other)
        {
            PointerTypeReferenceSpec o = other as PointerTypeReferenceSpec;
            return o != null && this.elementType == o.elementType;
        }
    }
    [Serializable]
    public sealed class NullableTypeReferenceSpec : TypeReferenceSpec
    {
        readonly ITypeReference elementType;

        public NullableTypeReferenceSpec(ITypeReference elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            this.elementType = elementType;
        }

        public ITypeReference ElementType
        {
            get { return elementType; }
        }

        public IType Resolve(ITypeResolveContext context)
        {
            return new NullableTypeSpec(elementType.Resolve(context));
        }

        public override string ToString()
        {
            return elementType.ToString() + "?";
        }

        public override int GetHashCodeForInterning()
        {
            return elementType.GetHashCode() ^ 91725814;
        }

        public override bool EqualsForInterning(ISupportsInterning other)
        {
            NullableTypeReferenceSpec o = other as NullableTypeReferenceSpec;
            return o != null && this.elementType == o.elementType;
        }
    }
    [Serializable]
    public sealed class ArrayTypeReferenceSpec : TypeReferenceSpec
    {
        readonly ITypeReference elementType;
        readonly int dimensions;

        public ArrayTypeReferenceSpec(ITypeReference elementType, int dimensions = 1)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            if (dimensions <= 0)
                throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
            this.elementType = elementType;
            this.dimensions = dimensions;
        }

        public ITypeReference ElementType
        {
            get { return elementType; }
        }

        public int Dimensions
        {
            get { return dimensions; }
        }

        public IType Resolve(ITypeResolveContext context)
        {
            return new ArrayTypeSpec(context.Compilation, elementType.Resolve(context), dimensions);
        }

        public override string ToString()
        {
            return elementType.ToString() + "[" + new string(',', dimensions - 1) + "]";
        }

        public override int GetHashCodeForInterning()
        {
            return elementType.GetHashCode() ^ dimensions;
        }

        public override bool EqualsForInterning(ISupportsInterning other)
        {
            ArrayTypeReferenceSpec o = other as ArrayTypeReferenceSpec;
            return o != null && elementType == o.elementType && dimensions == o.dimensions;
        }
    }



    public class BuiltinTypes
    {
        //public readonly BuiltinTypeSpec Object;
        //public readonly BuiltinTypeSpec ValueType;
        //public readonly BuiltinTypeSpec Attribute;

        //public readonly BuiltinTypeSpec Int;
        //public readonly BuiltinTypeSpec UInt;
        //public readonly BuiltinTypeSpec Long;
        //public readonly BuiltinTypeSpec ULong;
        //public readonly BuiltinTypeSpec Float;
        //public readonly BuiltinTypeSpec Double;
        //public readonly BuiltinTypeSpec Char;
        //public readonly BuiltinTypeSpec Short;
        //public readonly BuiltinTypeSpec Decimal;
        //public readonly BuiltinTypeSpec Bool;
        //public readonly BuiltinTypeSpec SByte;
        //public readonly BuiltinTypeSpec Byte;
        //public readonly BuiltinTypeSpec UShort;
        //public readonly BuiltinTypeSpec String;

    

    
    }
 


}
