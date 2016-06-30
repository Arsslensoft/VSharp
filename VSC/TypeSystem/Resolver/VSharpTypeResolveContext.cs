using System;
using VSC.Context;
using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem.Resolver
{
	public sealed class VSharpTypeResolveContext : ITypeResolveContext
	{
		readonly IAssembly assembly;
		readonly ResolvedUsingScope currentUsingScope;
		readonly ITypeDefinition currentTypeDefinition;
        readonly ITypeDefinition defaultTypeDefinition;
		readonly IMember currentMember;
		readonly string[] methodTypeParameterNames;
		
		public VSharpTypeResolveContext(IAssembly assembly, ResolvedUsingScope usingScope = null, ITypeDefinition typeDefinition = null, IMember member = null)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			this.assembly = assembly;
			this.currentUsingScope = usingScope;
			this.currentTypeDefinition = typeDefinition;
            this.defaultTypeDefinition = new ResolvedTypeDefinitionSpec(this, SymbolResolveContext.DefaultTypes.ToArray());
			this.currentMember = member;
		}
		
		private VSharpTypeResolveContext(IAssembly assembly, ResolvedUsingScope usingScope, ITypeDefinition typeDefinition, IMember member, string[] methodTypeParameterNames)
		{
			this.assembly = assembly;
			this.currentUsingScope = usingScope;
			this.currentTypeDefinition = typeDefinition;
			this.currentMember = member;
			this.methodTypeParameterNames = methodTypeParameterNames;
		}
		
		public ResolvedUsingScope CurrentUsingScope {
			get { return currentUsingScope; }
		}
		
		public ICompilation Compilation {
			get { return assembly.Compilation; }
		}
		
		public IAssembly CurrentAssembly {
			get { return assembly; }
		}
		
		public ITypeDefinition CurrentTypeDefinition {
			get { return currentTypeDefinition; }
		}
		
		public IMember CurrentMember {
			get { return currentMember; }
		}
		
		public VSharpTypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return new VSharpTypeResolveContext(assembly, currentUsingScope, typeDefinition, currentMember, methodTypeParameterNames);
		}
		
		ITypeResolveContext ITypeResolveContext.WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return WithCurrentTypeDefinition(typeDefinition);
		}
		
		public VSharpTypeResolveContext WithCurrentMember(IMember member)
		{
			return new VSharpTypeResolveContext(assembly, currentUsingScope, currentTypeDefinition, member, methodTypeParameterNames);
		}
		
		ITypeResolveContext ITypeResolveContext.WithCurrentMember(IMember member)
		{
			return WithCurrentMember(member);
		}
		
		public VSharpTypeResolveContext WithUsingScope(ResolvedUsingScope usingScope)
		{
			return new VSharpTypeResolveContext(assembly, usingScope, currentTypeDefinition, currentMember, methodTypeParameterNames);
		}
	}
}
