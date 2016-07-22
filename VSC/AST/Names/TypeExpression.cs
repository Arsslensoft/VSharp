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
            if (!_resolved)
                return type.Resolve(ctx);
            else return ResolvedType;
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            if (!_resolved)
                ResolvedType = Resolve(rc);


            return this;
        }

        public TypeExpression(ITypeReference t, Location l)
        {
         
            type = t;
            loc = l;
        }
        public TypeExpression(IType type, Location l)
        {
            ResolvedType = type;
            loc = l;
            _resolved = true;
        }
        public TypeExpression(IType type)
        {
            ResolvedType = type;
           
            _resolved = true;
        }


        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }
    }
}