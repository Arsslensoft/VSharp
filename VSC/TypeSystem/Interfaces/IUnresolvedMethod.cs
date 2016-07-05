using System.Collections.Generic;

namespace VSC.TypeSystem
{
    public interface IUnresolvedMethod : IUnresolvedParameterizedMember
    {
        /// <summary>
        /// Gets the attributes associated with the return type. (e.g. [return: MarshalAs(...)])
        /// </summary>
        IList<IUnresolvedAttribute> ReturnTypeAttributes { get; }
		
        IList<IUnresolvedTypeParameter> TypeParameters { get; }
		
        bool IsConstructor { get; }
        bool IsDestructor { get; }
        bool IsOperator { get; }

        /// <summary>
        /// Gets whether the method is a V#-style superseded method.
        /// To test if it is a superseded method declaration.
        /// </summary>
        bool IsSupersede { get; }

        /// <summary>
        /// Gets whether the method is a V#-style async method.
        /// </summary>
        bool IsAsync { get; }

		
        /// <summary>
        /// Gets whether the method has a body.
        /// This property returns <c>false</c> for <c>abstract</c> or <c>extern</c> methods,
        /// or for <c>partial</c> methods without implementation.
        /// </summary>
        bool HasBody { get; }
		
        /// <summary>
        /// If this method is an accessor, returns a reference to the corresponding property/event.
        /// Otherwise, returns null.
        /// </summary>
        IUnresolvedMember AccessorOwner { get; }
		
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
        new IMethod Resolve(ITypeResolveContext context);
    }
}