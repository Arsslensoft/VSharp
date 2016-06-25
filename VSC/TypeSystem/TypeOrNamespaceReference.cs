using System;
using VSC.Base;
using VSC.TypeSystem.Resolver;


namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a reference which could point to a type or namespace.
	/// </summary>
	[Serializable]
	public abstract class TypeOrNamespaceReference : ITypeReference
	{
		/// <summary>
		/// Resolves the reference and returns the ResolveResult.
		/// </summary>
		public abstract ResolveResult Resolve(VSharpResolver resolver);
		
		/// <summary>
		/// Returns the type that is referenced; or an <c>UnknownType</c> if the type isn't found.
		/// </summary>
		public abstract IType ResolveType(VSharpResolver resolver);
		
		/// <summary>
		/// Returns the namespace that is referenced; or null if no such namespace is found.
		/// </summary>
		public INamespace ResolveNamespace(VSharpResolver resolver)
		{
			NamespaceResolveResult nrr = Resolve(resolver) as NamespaceResolveResult;
			return nrr != null ? nrr.Namespace : null;
		}
		
		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			// Strictly speaking, we might have to resolve the type in a nested compilation, similar
			// to what we're doing with ConstantExpression.
			// However, in almost all cases this will work correctly - if the resulting type is only available in the
			// nested compilation and not in this, we wouldn't be able to map it anyways.
			var ctx = context as VSharpTypeResolveContext;
			if (ctx == null) {
				ctx = new VSharpTypeResolveContext(context.CurrentAssembly ?? context.Compilation.MainAssembly, null, context.CurrentTypeDefinition, context.CurrentMember);
			}
			return ResolveType(new VSharpResolver(ctx));
			
			// A potential issue might be this scenario:
			
			// Assembly 1:
			//  class A { public class Nested {} }
			
			// Assembly 2: (references asm 1)
			//  class B : A {}
			
			// Assembly 3: (references asm 1 and 2)
			//  class C { public B.Nested Field; }
			
			// Assembly 4: (references asm 1 and 3, but not 2):
			//  uses C.Field;
			
			// Here we would not be able to resolve 'B.Nested' in the compilation of assembly 4, as type B is missing there.
		}
	}
}
