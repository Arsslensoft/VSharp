using System;
using System.Collections.Generic;
using VSC.TypeSystem;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Method/field/property/event.
	/// </summary>
	public interface IMember : IEntity
	{
		/// <summary>
		/// Gets the original member definition for this member.
		/// Returns <c>this</c> if this is not a specialized member.
		/// Specialized members are the result of overload resolution with type substitution.
		/// </summary>
		IMember MemberDefinition { get; }
		
		/// <summary>
		/// Gets the unresolved member instance from which this member was created.
		/// This property may return <c>null</c> for special members that do not have a corresponding unresolved member instance.
		/// </summary>
		/// <remarks>
		/// For specialized members, this property returns the unresolved member for the original member definition.
		/// For partial methods, this property returns the implementing partial method declaration, if one exists, and the
		/// defining partial method declaration otherwise.
		/// For the members used to represent the built-in V# operators like "operator +(int, int);", this property returns <c>null</c>.
		/// </remarks>
		IUnresolvedMember UnresolvedMember { get; }
		
		/// <summary>
		/// Gets the return type of this member.
		/// This property never returns <c>null</c>.
		/// </summary>
		IType ReturnType { get; }
		
		/// <summary>
		/// Gets the interface members implemented by this member (both implicitly and explicitly).
		/// </summary>
		IList<IMember> ImplementedInterfaceMembers { get; }
		
		/// <summary>
		/// Gets whether this member is explicitly implementing an interface.
		/// </summary>
		bool IsExplicitInterfaceImplementation { get; }
		
		/// <summary>
		/// Gets if the member is virtual. Is true only if the "virtual" modifier was used, but non-virtual
		/// members can be overridden, too; if they are abstract or overriding a method.
		/// </summary>
		bool IsVirtual { get; }
		
		/// <summary>
		/// Gets whether this member is overriding another member.
		/// </summary>
		bool IsOverride { get; }
		
		/// <summary>
		/// Gets if the member can be overridden. Returns true when the member is "abstract", "virtual" or "override" but not "sealed".
		/// </summary>
		bool IsOverridable { get; }
		
		/// <summary>
		/// Creates a member reference that can be used to rediscover this member in another compilation.
		/// </summary>
		/// <remarks>
		/// If this member is specialized using open generic types, the resulting member reference will need to be looked up in an appropriate generic context.
		/// Otherwise, the main resolve context of a compilation is sufficient.
		/// </remarks>
		[Obsolete("Use the ToReference method instead.")]
		IMemberReference ToMemberReference();
		
				/// <summary>
		/// Creates a member reference that can be used to rediscover this member in another compilation.
		/// </summary>
		/// <remarks>
		/// If this member is specialized using open generic types, the resulting member reference will need to be looked up in an appropriate generic context.
		/// Otherwise, the main resolve context of a compilation is sufficient.
		/// </remarks>
		new IMemberReference ToReference();

		/// <summary>
		/// Gets the substitution belonging to this specialized member.
		/// Returns TypeParameterSubstitution.Identity for not specialized members.
		/// </summary>
		TypeParameterSubstitution Substitution {
			get;
		}

		/// <summary>
		/// Specializes this member with the given substitution.
		/// If this member is already specialized, the new substitution is composed with the existing substition.
		/// </summary>
		IMember Specialize(TypeParameterSubstitution substitution);
	}
}
