using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Fully resolved Expression that already evaluated to a type
    /// </summary>
    public class TypeExpression : TypeExpr
    {
        public override IType Resolve(ITypeResolveContext ctx)
        {
            return type.Resolve(ctx);
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            Result = new TypeResolveResult(Resolve(rc));
            return this;
        }

        public TypeExpression(ITypeReference t, Location l)
        {
         
            Type = t;
            loc = l;
        }
        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }
    }
}