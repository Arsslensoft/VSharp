using System.Collections.Generic;

namespace VSC.TypeSystem
{
    /// <summary>
    /// Represents an unresolved class, enum, interface, struct, delegate or VB module.
    /// For partial classes, an unresolved type definition represents only a single part.
    /// </summary>
    public interface IUnresolvedTypeDefinition : ITypeReference, IUnresolvedEntity
    {
        TypeKind Kind { get; }
		
        FullTypeName FullTypeName { get; }
        IList<ITypeReference> BaseTypes { get; }
        IList<IUnresolvedTypeParameter> TypeParameters { get; }
		
        IList<IUnresolvedTypeDefinition> NestedTypes { get; }
        IList<IUnresolvedMember> Members { get; }
		
        IEnumerable<IUnresolvedMethod> Methods { get; }
        IEnumerable<IUnresolvedProperty> Properties { get; }
        IEnumerable<IUnresolvedField> Fields { get; }
        IEnumerable<IUnresolvedEvent> Events { get; }
		
        /// <summary>
        /// Gets whether the type definition contains extension methods.
        /// Returns null when the type definition needs to be resolved in order to determine whether
        /// methods are extension methods.
        /// </summary>
        bool? HasExtensionMethods { get; }

        /// <summary>
        /// Gets whether the partial modifier is set on this part of the type definition.
        /// </summary>
        bool IsPartial { get; }
		
        /// <summary>
        /// Gets whether this unresolved type definition causes the addition of a default constructor
        /// if no other constructor is present.
        /// </summary>
        bool AddDefaultConstructorIfRequired { get; }
		
        /// <summary>
        /// Looks up the resolved type definition from the <paramref name="context"/> corresponding to this unresolved
        /// type definition.
        /// </summary>
        /// <param name="context">
        /// Context for looking up the type. The context must specify the current assembly.
        /// A <see cref="SimpleTypeResolveContext"/> that specifies the current assembly is sufficient.
        /// </param>
        /// <returns>
        /// Returns the resolved type definition.
        /// In case of an error, returns an <see cref="Implementation.UnknownTypeSpec"/> instance.
        /// Never returns null.
        /// </returns>
        new IType Resolve(ITypeResolveContext context);
		
        /// <summary>
        /// This method is used to add language-specific elements like the V# UsingScope
        /// to the type resolve context.
        /// </summary>
        /// <param name="parentContext">The parent context (e.g. the parent assembly),
        /// including the parent type definition for inner classes.</param>
        /// <returns>
        /// The parent context, modified to include language-specific elements (e.g. using scope)
        /// associated with this type definition.
        /// </returns>
        /// <remarks>
        /// Use <c>unresolvedTypeDef.CreateResolveContext(parentContext).WithTypeDefinition(typeDef)</c> to
        /// create the context for use within the type definition.
        /// </remarks>
        ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext);
    }
}