using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a reference to a type.
	/// Must be resolved before it can be used as type.
	/// </summary>
	public interface ITypeReference
	{
		// Keep this interface simple: I decided against having GetMethods/GetEvents etc. here,
		// so that the ResolveScope step is never hidden from the consumer.
		
		// I decided against implementing IFreezable here: IUnresolvedTypeDefinition can be used as ITypeReference,
		// but when freezing the reference, one wouldn't expect the definition to freeze.
		
		/// <summary>
		/// Resolves this type reference.
		/// </summary>
		/// <param name="context">
		/// Context to use for resolving this type reference.
		/// Which kind of context is required depends on the which kind of type reference this is;
		/// please consult the documentation of the method that was used to create this type reference,
		/// or that of the class implementing this method.
		/// </param>
		/// <returns>
		/// Returns the resolved type.
		/// In case of an error, returns an unknown type (<see cref="TypeKind.Unknown"/>).
		/// Never returns null.
		/// </returns>
		IType Resolve(ITypeResolveContext context);
	}
}