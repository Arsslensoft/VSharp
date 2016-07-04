using System;
using System.Collections.Generic;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Helper class for the GetAllBaseTypes() implementation.
	/// </summary>
	sealed class BaseTypeCollector : List<IType>
	{
		readonly Stack<IType> activeTypes = new Stack<IType>();
		
		/// <summary>
		/// If this option is enabled, the list will not contain interfaces when retrieving the base types
		/// of a class.
		/// </summary>
		internal bool SkipImplementedInterfaces;
		
		public void CollectBaseTypes(IType type)
		{
			IType def = type.GetDefinition() ?? type;
			
			// Maintain a stack of currently active type definitions, and avoid having one definition
			// multiple times on that stack.
			// This is necessary to ensure the output is finite in the presence of cyclic inheritance:
			// class C<X> : C<C<X>> {} would not be caught by the 'no duplicate output' check, yet would
			// produce infinite output.
			if (activeTypes.Contains(def))
				return;
			activeTypes.Push(def);
			// Note that we also need to push non-type definitions, e.g. for protecting against
			// cyclic inheritance in type parameters (where T : S where S : T).
			// The output check doesn't help there because we call Addition(type) only at the end.
			// We can't simply call this.Addition(type); at the start because that would return in an incorrect order.
			
			// Avoid outputting a type more than once - necessary for "diamond" multiple inheritance
			// (e.g. C implements I1 and I2, and both interfaces derive from Object)
			if (!this.Contains(type)) {
				foreach (IType baseType in type.DirectBaseTypes) {
					if (SkipImplementedInterfaces && def != null && def.Kind != TypeKind.Interface && def.Kind != TypeKind.TypeParameter) {
						if (baseType.Kind == TypeKind.Interface) {
							// skip the interface
							continue;
						}
					}
					CollectBaseTypes(baseType);
				}
				// Addition(type) at the end - we want a type to be output only after all its base types were added.
				this.Add(type);
				// Note that this is not the same as putting the this.Addition() call in front and then reversing the list.
				// For the diamond inheritance, Addition() at the start produces "C, I1, Object, I2",
				// while Addition() at the end produces "Object, I1, I2, C".
			}
			activeTypes.Pop();
		}
	}
}
