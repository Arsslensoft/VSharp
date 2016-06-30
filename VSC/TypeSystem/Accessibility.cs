using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Enum that describes the accessibility of an entity.
	/// </summary>
	public enum Accessibility : byte
	{
		// note: some code depends on the fact that these values are within the range 0-7
		
		/// <summary>
		/// The entity is completely inaccessible. This is used for V# explicit interface implementations.
		/// </summary>
		None,
		/// <summary>
		/// The entity is only accessible within the same class.
		/// </summary>
		Private,
		/// <summary>
		/// The entity is accessible everywhere.
		/// </summary>
		Public,
		/// <summary>
		/// The entity is only accessible within the same class and in derived classes.
		/// </summary>
		Protected,
		/// <summary>
		/// The entity is accessible within the same project content.
		/// </summary>
		Internal,
		/// <summary>
		/// The entity is accessible both everywhere in the project content, and in all derived classes.
		/// </summary>
		/// <remarks>This corresponds to V# 'protected internal'.</remarks>
		ProtectedOrInternal,
		/// <summary>
		/// The entity is accessible in derived classes within the same project content.
		/// </summary>
		/// <remarks>V# does not support this accessibility.</remarks>
		ProtectedAndInternal,
	}
	
	public interface IHasAccessibility
	{
		/// <summary>
		/// Gets the accessibility of this entity.
		/// </summary>
		Accessibility Accessibility { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance is private.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is private; otherwise, <c>false</c>.
		/// </value>
		bool IsPrivate { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance is public.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is public; otherwise, <c>false</c>.
		/// </value>
		bool IsPublic { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance is protected.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is protected; otherwise, <c>false</c>.
		/// </value>
		bool IsProtected { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance is internal.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is internal; otherwise, <c>false</c>.
		/// </value>
		bool IsInternal { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance is protected or internal.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is protected or internal; otherwise, <c>false</c>.
		/// </value>
		bool IsProtectedOrInternal { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance is protected and internal.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is protected and internal; otherwise, <c>false</c>.
		/// </value>
		bool IsProtectedAndInternal { get; }
	}
}
