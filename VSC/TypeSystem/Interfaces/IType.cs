using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
	/// <summary>
	/// This interface represents a resolved type in the type system.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A type is potentially
	/// - a type definition (<see cref="ITypeDefinition"/>, i.e. a class, struct, interface, delegate, or built-in primitive type)
	/// - a parameterized type (<see cref="ParameterizedTypeSpec"/>, e.g. List&lt;int>)
	/// - a type parameter (<see cref="ITypeParameter"/>, e.g. T)
	/// - an array (<see cref="ArrayType"/>)
	/// - a pointer (<see cref="PointerTypeSpec"/>)
	/// - a managed reference (<see cref="NullableTypeSpec"/>)
	/// - one of the special types (<see cref="SpecialTypeSpec.UnknownTypeSpec"/>, <see cref="SpecialTypeSpec.NullType"/>,
	///      <see cref="SpecialTypeSpec.Dynamic"/>, <see cref="SpecialTypeSpec.UnboundTypeArgument"/>)
	/// 
	/// The <see cref="IType.Kind"/> property can be used to switch on the kind of a type.
	/// </para>
	/// <para>
	/// IType uses the null object pattern: <see cref="SpecialTypeSpec.UnknownTypeSpec"/> serves as the null object.
	/// Methods or properties returning IType never return null unless documented otherwise.
	/// </para>
	/// <para>
	/// Types should be compared for equality using the <see cref="IEquatable{IType}.Equals(IType)"/> method.
	/// Identical types do not necessarily use the same object reference.
	/// </para>
	/// </remarks>
	public interface IType : INamedElement, IEquatable<IType>
	{
		/// <summary>
		/// Gets the type kind.
		/// </summary>
		TypeKind Kind { get; }
		
		/// <summary>
		/// Gets whether the type is a reference type or value type.
		/// </summary>
		/// <returns>
		/// true, if the type is a reference type.
		/// false, if the type is a value type.
		/// null, if the type is not known (e.g. unconstrained generic type parameter or type not found)
		/// </returns>
		bool? IsReferenceType { get; }
		
		/// <summary>
		/// Gets the underlying type definition.
		/// Can return null for types which do not have a type definition (for example arrays, pointers, type parameters).
		/// </summary>
		ITypeDefinition GetDefinition();
		
		/// <summary>
		/// Gets the parent type, if this is a nested type.
		/// Returns null for top-level types.
		/// </summary>
		IType DeclaringType { get; }
		
		/// <summary>
		/// Gets the number of type parameters.
		/// </summary>
		int TypeParameterCount { get; }

		/// <summary>
		/// Gets the type arguments passed to this type.
		/// If this type is a generic type definition that is not parameterized, this property returns the type parameters,
		/// as if the type was parameterized with its own type arguments (<c>class C&lt;T&gt; { C&lt;T&gt; field; }</c>).
		/// 
		/// NOTE: The type will change to IReadOnlyList&lt;IType&gt; in future versions.
		/// </summary>
		IList<IType> TypeArguments { get; }

		/// <summary>
		/// If true the type represents an instance of a generic type.
		/// </summary>
		bool IsParameterized { get; }

		/// <summary>
		/// Calls ITypeVisitor.Visit for this type.
		/// </summary>
		/// <returns>The return value of the ITypeVisitor.Visit call</returns>
		IType AcceptVisitor(TypeVisitor visitor);
		
		/// <summary>
		/// Calls ITypeVisitor.Visit for all children of this type, and reconstructs this type with the children based
		/// on the return values of the visit calls.
		/// </summary>
		/// <returns>A copy of this type, with all children replaced by the return value of the corresponding visitor call.
		/// If the visitor returned the original types for all children (or if there are no children), returns <c>this</c>.
		/// </returns>
		IType VisitChildren(TypeVisitor visitor);
		
		/// <summary>
		/// Gets the direct base types.
		/// </summary>
		/// <returns>Returns the direct base types including interfaces</returns>
		IEnumerable<IType> DirectBaseTypes { get; }
		
		/// <summary>
		/// Creates a type reference that can be used to look up a type equivalent to this type in another compilation.
		/// </summary>
		/// <remarks>
		/// If this type contains open generics, the resulting type reference will need to be looked up in an appropriate generic context.
		/// Otherwise, the main resolve context of a compilation is sufficient.
		/// </remarks>
		ITypeReference ToTypeReference();

		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type.
		/// Returns TypeParameterSubstitution.Identity if the type is not parametrized.
		/// </summary>
		TypeParameterSubstitution GetSubstitution();
		
		/// <summary>
		/// Gets a type visitor that performs the substitution of class type parameters with the type arguments
		/// of this parameterized type,
		/// and also substitutes method type parameters with the specified method type arguments.
		/// Returns TypeParameterSubstitution.Identity if the type is not parametrized.
		/// </summary>
		TypeParameterSubstitution GetSubstitution(IList<IType> methodTypeArguments);

        /// <summary>
        /// Gets inner classes (including inherited inner classes).
        /// </summary>
        /// <param name="filter">The filter used to select which types to return.
        /// The filter is tested on the original type definitions (before parameterization).</param>
        /// <param name="options">Specified additional options for the GetMembers() operation.</param>
        /// <remarks>
        /// <para>
        /// If the nested type is generic, this method will return a parameterized type,
        /// where the additional type parameters are set to <see cref="SpecialType.UnboundTypeArgument"/>.
        /// </para>
        /// <para>
        /// Type parameters belonging to the outer class will have the value copied from the outer type
        /// if it is a parameterized type. Otherwise, those existing type parameters will be self-parameterized,
        /// and thus 'leaked' to the caller in the same way the GetMembers() method does not specialize members
        /// from an <see cref="ITypeDefinition"/> and 'leaks' type parameters in member signatures.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// class Base&lt;T> {
        /// 	class Nested&lt;X> {}
        /// }
        /// class Derived&lt;A, B> : Base&lt;B> {}
        /// 
        /// Derived[string,int].GetNestedTypes() = { Base`1+Nested`1[int, unbound] }
        /// Derived.GetNestedTypes() = { Base`1+Nested`1[`1, unbound] }
        /// Base[`1].GetNestedTypes() = { Base`1+Nested`1[`1, unbound] }
        /// Base.GetNestedTypes() = { Base`1+Nested`1[`0, unbound] }
        /// </code>
        /// </example>
        IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None);

        // Note that we cannot 'leak' the additional type parameter as we leak the normal type parameters, because
        // the index might collide. For example,
        //   class Base<T> { class Nested<X> {} }
        //   class Derived<A, B> : Base<B> { }
        // 
        // Derived<string, int>.GetNestedTypes() = Base+Nested<int, UnboundTypeArgument>
        // Derived.GetNestedTypes() = Base+Nested<`1, >
        //  Here `1 refers to B, and there's no way to return X as it would collide with B.

        /// <summary>
        /// Gets inner classes (including inherited inner classes)
        /// that have <c>typeArguments.Count</c> additional type parameters.
        /// </summary>
        /// <param name="typeArguments">The type arguments passed to the inner class</param>
        /// <param name="filter">The filter used to select which types to return.
        /// The filter is tested on the original type definitions (before parameterization).</param>
        /// <param name="options">Specified additional options for the GetMembers() operation.</param>
        /// <remarks>
        /// Type parameters belonging to the outer class will have the value copied from the outer type
        /// if it is a parameterized type. Otherwise, those existing type parameters will be self-parameterized,
        /// and thus 'leaked' to the caller in the same way the GetMembers() method does not specialize members
        /// from an <see cref="ITypeDefinition"/> and 'leaks' type parameters in member signatures.
        /// </remarks>
        IEnumerable<IType> GetNestedTypes(IList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all instance constructors for this type.
		/// </summary>
		/// <param name="filter">The filter used to select which constructors to return.
		/// The filter is tested on the original method definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// <para>The result does not include static constructors.
		/// Constructors in base classes are not returned by default, as GetMemberOptions.IgnoreInheritedMembers is the default value.</para>
		/// <para>
		/// For methods on parameterized types, type substitution will be performed on the method signature,
		/// and the appropriate <see cref="Implementation.SpecializedMethod"/> will be returned.
		/// </para>
		/// </remarks>
		IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers);
		
		/// <summary>
		/// Gets all methods that can be called on this type.
		/// </summary>
		/// <param name="filter">The filter used to select which methods to return.
		/// The filter is tested on the original method definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// <para>
		/// The result does not include constructors or accessors.
		/// </para>
		/// <para>
		/// For methods on parameterized types, type substitution will be performed on the method signature,
		/// and the appropriate <see cref="Implementation.SpecializedMethod"/> will be returned.
		/// </para>
		/// <para>
		/// If the method being returned is generic, and this type is a parameterized type where the type
		/// arguments involve another method's type parameters, the resulting specialized signature
		/// will be ambiguous as to which method a type parameter belongs to.
		/// For example, "List[[``0]].GetMethods()" will return "ConvertAll(Converter`2[[``0, ``0]])".
		/// 
		/// If possible, use the other GetMethods() overload to supply type arguments to the method,
		/// so that both class and method type parameter can be substituted at the same time, so that
		/// the ambiguity can be avoided.
		/// </para>
		/// </remarks>
		IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all generic methods that can be called on this type with the specified type arguments.
		/// </summary>
		/// <param name="typeArguments">The type arguments used for the method call.</param>
		/// <param name="filter">The filter used to select which methods to return.
		/// The filter is tested on the original method definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// <para>The result does not include constructors or accessors.</para>
		/// <para>
		/// Type substitution will be performed on the method signature, creating a <see cref="Implementation.SpecializedMethod"/>
		/// with the specified type arguments.
		/// </para>
		/// <para>
		/// When the list of type arguments is empty, this method acts like the GetMethods() overload without
		/// the type arguments parameter - that is, it also returns generic methods,
		/// and the other overload's remarks about ambiguous signatures apply here as well.
		/// </para>
		/// </remarks>
		IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all properties that can be called on this type.
		/// </summary>
		/// <param name="filter">The filter used to select which properties to return.
		/// The filter is tested on the original property definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// For properties on parameterized types, type substitution will be performed on the property signature,
		/// and the appropriate <see cref="Implementation.SpecializedProperty"/> will be returned.
		/// </remarks>
		IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all fields that can be accessed on this type.
		/// </summary>
		/// <param name="filter">The filter used to select which constructors to return.
		/// The filter is tested on the original field definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// For fields on parameterized types, type substitution will be performed on the field's return type,
		/// and the appropriate <see cref="Implementation.SpecializedField"/> will be returned.
		/// </remarks>
		IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all events that can be accessed on this type.
		/// </summary>
		/// <param name="filter">The filter used to select which events to return.
		/// The filter is tested on the original event definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// For fields on parameterized types, type substitution will be performed on the event's return type,
		/// and the appropriate <see cref="Implementation.SpecializedEvent"/> will be returned.
		/// </remarks>
		IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all members that can be called on this type.
		/// </summary>
		/// <param name="filter">The filter used to select which members to return.
		/// The filter is tested on the original member definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// <para>
		/// The resulting list is the union of GetFields(), GetProperties(), GetMethods() and GetEvents().
		/// It does not include constructors.
		/// For parameterized types, type substitution will be performed.
		/// </para>
		/// <para>
		/// For generic methods, the remarks about ambiguous signatures from the
		/// <see cref="GetMethods(Predicate{IUnresolvedMethod}, GetMemberOptions)"/> method apply here as well.
		/// </para>
		/// </remarks>
		IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None);
		
		/// <summary>
		/// Gets all accessors belonging to properties or events on this type.
		/// </summary>
		/// <param name="filter">The filter used to select which members to return.
		/// The filter is tested on the original member definitions (before specialization).</param>
		/// <param name="options">Specified additional options for the GetMembers() operation.</param>
		/// <remarks>
		/// Accessors are not returned by GetMembers() or GetMethods().
		/// </remarks>
		IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None);
	}
}
