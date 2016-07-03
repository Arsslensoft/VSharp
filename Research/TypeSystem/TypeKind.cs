using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// .
	/// </summary>
	public enum TypeKind : byte
	{
		/// <summary>Language-specific type that is not part of NRefactory.TypeSystem itself.</summary>
		Other,
		
		/// <summary>A <see cref="ITypeDefinition"/> or <see cref="ParameterizedTypeSpec"/> that is a class.</summary>
		Class,
		/// <summary>A <see cref="ITypeDefinition"/> or <see cref="ParameterizedTypeSpec"/> that is an interface.</summary>
		Interface,
		/// <summary>A <see cref="ITypeDefinition"/> or <see cref="ParameterizedTypeSpec"/> that is a struct.</summary>
		Struct,
		/// <summary>A <see cref="ITypeDefinition"/> or <see cref="ParameterizedTypeSpec"/> that is a delegate.</summary>
		/// <remarks><c>System.Delegate</c> itself is TypeKind.Class</remarks>
		Delegate,
		/// <summary>A <see cref="ITypeDefinition"/> that is an enum.</summary>
		/// <remarks><c>System.Enum</c> itself is TypeKind.Class</remarks>
		Enum,
		/// <summary>A <see cref="ITypeDefinition"/> that is a module (VB).</summary>
		Module,
		
		/// <summary>The <c>System.Void</c> type.</summary>
		/// <see cref="KnownTypeReference.Void"/>
		Void,
		
		/// <see cref="SpecialTypeSpec.UnknownTypeSpec"/>
		Unknown,
		/// <summary>The type of the null literal.</summary>
		/// <see cref="SpecialTypeSpec.NullType"/>
		Null,
		/// <summary>Type representing the V# 'dynamic' type.</summary>
		/// <see cref="SpecialTypeSpec.Dynamic"/>
		Dynamic,
		/// <summary>Represents missing type arguments in partially parameterized types.</summary>
		/// <see cref="SpecialTypeSpec.UnboundTypeArgument"/>
		UnboundTypeArgument,
		
		/// <summary>The type is a type parameter.</summary>
		/// <see cref="ITypeParameter"/>
		TypeParameter,
		
		/// <summary>An array type</summary>
		/// <see cref="ArrayType"/>
		Array,
		/// <summary>A pointer type</summary>
		/// <see cref="PointerTypeSpec"/>
		Pointer,
		/// <summary>A managed reference type</summary>
        /// <see cref="ByReferenceTypeSpec"/>
		ByReference,
		/// <summary>An anonymous type</summary>
		/// <see cref="AnonymousTypeSpec"/>
		Anonymous,
		
		/// <summary>Intersection of several types</summary>
		/// <see cref="IntersectionType"/>
		Intersection,
		/// <see cref="SpecialTypeSpec.ArgList"/>
		ArgList,

        Union
	}
}
