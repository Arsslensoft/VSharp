using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class ComposedType : TypeExpr
    {
        FullNamedExpression left;
        ComposedTypeSpecifier spec;
        public override IType Resolve(ITypeResolveContext ctx)
        {
            return Result.type;
        }
        public ComposedType(FullNamedExpression left, ComposedTypeSpecifier spec)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");

            this.left = left;
            this.spec = spec;
            this.loc = left.Location;
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            left = (FullNamedExpression)left.DoResolve(rc);

            IType t = left.Result.type;
            var nxt = spec;
            while (nxt != null)
            {
                if (nxt.IsNullable)
                    t = NullableType.Create(rc.Compilation, t);

                else if (nxt.IsPointer)
                    t = new PointerTypeSpec(t);
                else
                    t = new ArrayType(rc.Compilation, t, nxt.Dimension);

                nxt = spec.Next;
            }

          
            
            Result =  new TypeResolveResult(t);
            return this;
        }

        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }
    }
}