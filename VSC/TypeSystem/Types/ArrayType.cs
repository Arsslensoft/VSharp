using System;
using System.Collections.Generic;
using VSC.Base;
using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents an array type.
	/// </summary>
	public sealed class ArrayType : ElementTypeSpec, ICompilationProvider
	{
		readonly int dimensions;
		readonly ICompilation compilation;
		
		public ArrayType(ICompilation compilation, IType elementType, int dimensions = 1) : base(elementType)
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
		
		public override TypeKind Kind {
			get { return TypeKind.Array; }
		}
		
		public ICompilation Compilation {
			get { return compilation; }
		}
		
		public int Dimensions {
			get { return dimensions; }
		}
		
		public override string NameSuffix {
			get {
				return "[" + new string(',', dimensions-1) + "]";
			}
		}
		
		public override bool? IsReferenceType {
			get { return true; }
		}
		
		public override int GetHashCode()
		{
			return unchecked(elementType.GetHashCode() * 71681 + dimensions);
		}
		
		public override bool Equals(IType other)
		{
			ArrayType a = other as ArrayType;
			return a != null && elementType.Equals(a.elementType) && a.dimensions == dimensions;
		}
		
		public override ITypeReference ToTypeReference()
		{
			return new ArrayTypeReference(elementType.ToTypeReference(), dimensions);
		}
		
		public override IEnumerable<IType> DirectBaseTypes {
			get {
				List<IType> baseTypes = new List<IType>();
				IType t = compilation.FindType(KnownTypeCode.Array);
				if (t.Kind != TypeKind.Unknown)
					baseTypes.Add(t);
				if (dimensions == 1 && elementType.Kind != TypeKind.Pointer) {
					// single-dimensional arrays implement IList<T>
					ITypeDefinition def = compilation.FindType(KnownTypeCode.IListOfT) as ITypeDefinition;
					if (def != null)
						baseTypes.Add(new ParameterizedTypeSpec(def, new[] { elementType }));
					// And in .NET 4.5 they also implement IReadOnlyList<T>
					def = compilation.FindType(KnownTypeCode.IReadOnlyListOfT) as ITypeDefinition;
					if (def != null)
						baseTypes.Add(new ParameterizedTypeSpec(def, new[] { elementType }));
				}
				return baseTypes;
			}
		}
		
		public override IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return compilation.FindType(KnownTypeCode.Array).GetMethods(filter, options);
		}
		
		public override IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return compilation.FindType(KnownTypeCode.Array).GetMethods(typeArguments, filter, options);
		}
		
		public override IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IMethod>.Instance;
			else
				return compilation.FindType(KnownTypeCode.Array).GetAccessors(filter, options);
		}
		
		public override IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers)
				return EmptyList<IProperty>.Instance;
			else
				return compilation.FindType(KnownTypeCode.Array).GetProperties(filter, options);
		}
		
		// NestedTypes, Events, Fields: System.Array doesn't have any; so we can use the AbstractType default implementation
		// that simply returns an empty list
		
		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitArrayType(this);
		}
		
		public override IType VisitChildren(TypeVisitor visitor)
		{
			IType e = elementType.AcceptVisitor(visitor);
			if (e == elementType)
				return this;
			else
				return new ArrayType(compilation, e, dimensions);
		}
	}
}
