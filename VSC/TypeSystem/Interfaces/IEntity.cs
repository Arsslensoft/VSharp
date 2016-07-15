using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Represents a resolved entity.
	/// </summary>
	public interface IEntity : ISymbol, ICompilationProvider, INamedElement, IHasAccessibility
	{
		/// <summary>
		/// Gets the short name of the entity.
		/// </summary>
		new string Name { get; }
		
		/// <summary>
		/// Gets the complete entity region (including header+body)
		/// </summary>
		DomRegion Region { get; }
		
		/// <summary>
		/// Gets the entity body region.
		/// </summary>
		DomRegion BodyRegion { get; }
		
		/// <summary>
		/// Gets the declaring class.
		/// For members, this is the class that contains the member.
		/// For nested classes, this is the outer class. For top-level entities, this property returns null.
		/// </summary>
		ITypeDefinition DeclaringTypeDefinition { get; }
		
		/// <summary>
		/// Gets/Sets the declaring type (incl. type arguments, if any).
		/// This property will return null for top-level entities.
		/// If this is not a specialized member, the value returned is equal to <see cref="DeclaringTypeDefinition"/>.
		/// </summary>
		IType DeclaringType { get; }
		
		/// <summary>
		/// The assembly in which this entity is defined.
		/// This property never returns null.
		/// </summary>
		IAssembly ParentAssembly { get; }
		
		/// <summary>
		/// Gets the attributes on this entity.
		/// </summary>
		IList<IAttribute> Attributes { get; }
		
		
		/// <summary>
		/// Gets whether this entity is static.
		/// Returns true if either the 'static' or the 'const' modifier is set.
		/// </summary>
		bool IsStatic { get; }
		
		/// <summary>
		/// Returns whether this entity is abstract.
		/// </summary>
		/// <remarks>Static classes also count as abstract classes.</remarks>
		bool IsAbstract { get; }
		
		/// <summary>
		/// Returns whether this entity is sealed.
		/// </summary>
		/// <remarks>Static classes also count as sealed classes.</remarks>
		bool IsSealed { get; }
		
		/// <summary>
		/// Gets whether this member is declared to be shadowing another member with the same name.
		/// (V# 'new' keyword)
		/// </summary>
		bool IsShadowing { get; }
		
		/// <summary>
		/// Gets whether this member is generated by a macro/compiler feature.
		/// </summary>
		bool IsSynthetic { get; }

   

        bool IsBaseTypeDefinition(IType baseType);
        bool IsAccessibleAs(IType b);
        bool IsInternalAccessible(IAssembly asm);
	}
}
