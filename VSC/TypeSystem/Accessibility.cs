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

	}
}
