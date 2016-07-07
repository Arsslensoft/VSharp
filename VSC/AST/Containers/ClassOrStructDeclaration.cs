using System.Diagnostics.Eventing.Reader;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class ClassOrStructDeclaration : TypeDeclaration
    {
        public ClassOrStructDeclaration(PackageContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns,mods,allowed, name,attr, l,kind,file)
        {

        }
        public ClassOrStructDeclaration(TypeContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns,mods,allowed, name, attr, l,kind,file)
        {
            
        }



        public void ResolveWithCurrentContext(ResolveContext rc)
        {
            
            // resolve type parameters and constraints
            foreach (var tp in typeParameters)
                (tp as UnresolvedTypeParameterSpec).Resolve(rc);



            base.Resolve(rc);

        }
        public ResolveResult ResolveTypeDefinition(string name, int typeParameterCount, ResolveContext rc)
        {
            ResolveContext previousResolver = rc;
            try
            {
                ITypeDefinition newTypeDefinition = null;
                if (rc.CurrentTypeDefinition != null)
                {
                    int totalTypeParameterCount = rc.CurrentTypeDefinition.TypeParameterCount + typeParameterCount;
                    foreach (ITypeDefinition nestedType in rc.CurrentTypeDefinition.NestedTypes)
                    {
                        if (nestedType.Name == name && nestedType.TypeParameterCount == totalTypeParameterCount)
                        {
                            newTypeDefinition = nestedType;
                            break;
                        }
                    }
                }
                else if (rc.CurrentUsingScope != null)
                {
                    newTypeDefinition = rc.CurrentUsingScope.Namespace.GetTypeDefinition(name, typeParameterCount);
                }
                if (newTypeDefinition != null)
                    rc = rc.WithCurrentTypeDefinition(newTypeDefinition);

                    // resolve children
                ResolveWithCurrentContext(rc);

                return newTypeDefinition != null ? new TypeResolveResult(newTypeDefinition) :(ResolveResult) ErrorResolveResult.UnknownError;
            }
            finally
            {
                rc = previousResolver;
            }
        }
        public override bool Resolve(ResolveContext rc)
        {
          

            // Resolve type definition
            ResolveResult rr = ResolveTypeDefinition(Name, typeParameters.Count,rc);
            if (rr.IsError)
            {
                
            }

         
            return !rr.IsError;
        }
    }
}