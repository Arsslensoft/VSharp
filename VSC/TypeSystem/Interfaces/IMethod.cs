using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using VSC.TypeSystem;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Represents a method, constructor, destructor or operator.
	/// </summary>
	public interface IMethod : IParameterizedMember
	{
		/// <summary>
		/// Gets the unresolved method parts.
		/// For partial methods, this returns all parts.
		/// Otherwise, this returns an array with a single element (new[] { UnresolvedMember }).
		/// NOTE: The type will change to IReadOnlyList&lt;IUnresolvedMethod&gt; in future versions.
		/// </summary>
		IList<IUnresolvedMethod> Parts { get; }
		
		/// <summary>
		/// Gets the attributes associated with the return type. (e.g. [return: MarshalAs(...)])
		/// NOTE: The type will change to IReadOnlyList&lt;IAttribute&gt; in future versions.
		/// </summary>
		IList<IAttribute> ReturnTypeAttributes { get; }

		/// <summary>
		/// Gets the type parameters of this method; or an empty list if the method is not generic.
		/// NOTE: The type will change to IReadOnlyList&lt;ITypeParameter&gt; in future versions.
		/// </summary>
		IList<ITypeParameter> TypeParameters { get; }

		/// <summary>
		/// Gets whether this is a generic method that has been parameterized.
		/// </summary>
		bool IsParameterized { get; }
		
		/// <summary>
		/// Gets the type arguments passed to this method.
		/// If the method is generic but not parameterized yet, this property returns the type parameters,
		/// as if the method was parameterized with its own type arguments (<c>void M&lt;T&gt;() { M&lt;T&gt;(); }</c>).
		/// 
		/// NOTE: The type will change to IReadOnlyList&lt;IType&gt; in future versions.
		/// </summary>
		IList<IType> TypeArguments { get; }

		bool IsExtensionMethod { get; }
		bool IsConstructor { get; }
		bool IsDestructor { get; }
		bool IsOperator { get; }

        /// <summary>
        /// Gets whether the method is a V#-style superseded method.
        /// To test if it is a superseded method declaration.
        /// </summary>
        bool IsSupersede { get; }


		/// <summary>
		/// Gets whether the method has a body.
		/// This property returns <c>false</c> for <c>abstract</c> or <c>extern</c> methods,
		/// or for <c>partial</c> methods without implementation.
		/// </summary>
		bool HasBody { get; }
		
		/// <summary>
		/// Gets whether the method is a property/event accessor.
		/// </summary>
		bool IsAccessor { get; }
		
		/// <summary>
		/// If this method is an accessor, returns the corresponding property/event.
		/// Otherwise, returns null.
		/// </summary>
		IMember AccessorOwner { get; }

		/// <summary>
		/// If this method is reduced from an extension method return the original method, <c>null</c> otherwise.
		/// A reduced method doesn't contain the extension method parameter. That means that has one parameter less than it's definition.
		/// </summary>
		IMethod ReducedFrom { get; }
		
		/// <summary>
		/// Specializes this method with the given substitution.
		/// If this method is already specialized, the new substitution is composed with the existing substition.
		/// </summary>
		new IMethod Specialize(TypeParameterSubstitution substitution);
	}
}
