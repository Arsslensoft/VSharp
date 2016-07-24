using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class EventAccessor : AbstractPropertyEventMethod
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        protected readonly EventDeclaration method;
       
        public const string AddPrefix = "add_";
        public const string RemovePrefix = "remove_";
        protected EventAccessor(EventDeclaration method, string prefix,Modifiers mods, VSharpAttributes attrs, Location loc)
            : base(method, new TypeExpression(KnownTypeReference.Void, loc), mods, AllowedModifiers, prefix, ParametersCompiled.CreateImplicitParameter(method.TypeExpression, loc), attrs, loc)
        {
            this.method = method;
        }
        void CreateResolvedMethod(ResolveContext rc)
        {
            ResolveContext oldResolver = rc;
            try
            {
                ITypeReference explicitInterfaceType = member_name.ExplicitInterface as ITypeReference;
                var member = Resolve(
                     rc.CurrentTypeResolveContext, SymbolKind, Name,
                     explicitInterfaceType, null, Parameters.Select(x => x.Type).ToArray());

                rc = rc.WithCurrentMember(member);
                ResolvedMethod = member as ResolvedMethodSpec;

                ResolveWithCurrentContext(rc);
            }
            finally
            {
                rc = oldResolver;
            }
        }
        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            // TODO:Resolve block of the accessor
            
        }
        public override bool DoResolve(TypeSystem.Resolver.ResolveContext rc)
        {
             CheckAbstractExtern(rc);
            CreateResolvedMethod(rc);
            return base.DoResolve(rc);
        }
    }
}