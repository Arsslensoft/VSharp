using System.Collections.Generic;

namespace VSC.TypeSystem
{
    /// <summary>
    /// Method/field/property/event.
    /// </summary>
    public interface IUnresolvedMember : IUnresolvedEntity, IMemberReference
    {
        /// <summary>
        /// Gets the return type of this member.
        /// This property never returns null.
        /// </summary>
        ITypeReference ReturnType { get; }
		
        /// <summary>
        /// Gets whether this member is explicitly implementing an interface.
        /// If this property is true, the member can only be called through the interfaces it implements.
        /// </summary>
        bool IsExplicitInterfaceImplementation { get; }
		
        /// <summary>
        /// Gets the interfaces that are explicitly implemented by this member.
        /// </summary>
        IList<IMemberReference> ExplicitInterfaceImplementations { get; }
		
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
        /// Resolves the member.
        /// </summary>
        /// <param name="context">
        /// Context for looking up the member. The context must specify the current assembly.
        /// A <see cref="SimpleTypeResolveContext"/> that specifies the current assembly is sufficient.
        /// </param>
        /// <returns>
        /// Returns the resolved member, or <c>null</c> if the member could not be found.
        /// </returns>
        new IMember Resolve(ITypeResolveContext context);
		
        /// <summary>
        /// Creates the resolved member.
        /// </summary>
        /// <param name="context">
        /// The language-specific context that includes the parent type definition.
        /// <see cref="IUnresolvedTypeDefinition.CreateResolveContext"/>
        /// </param>
        IMember CreateResolved(ITypeResolveContext context);
    }
}