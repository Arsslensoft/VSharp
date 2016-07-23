using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class StackAlloc : Expression
    {
        IType otype;
        Expression texpr;
        Expression count;

        public StackAlloc(Expression type, Expression count, Location l)
        {
            texpr = type;
            this.count = count;
            loc = l;
        }

        public Expression TypeExpression
        {
            get
            {
                return texpr;
            }
        }

        public Expression CountExpression
        {
            get
            {
                return this.count;
            }
        }


        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            count = count.DoResolve(rc);
            if (count == null)
                return null;

            if (!count.Type.IsKnownType(KnownTypeCode.UInt32))
            {
                count = new CastExpression(KnownTypeReference.UInt32.Resolve(rc), count).DoResolve(rc);
                if (count == null)
                    return null;
            }

            Constant c = count as Constant;
            if (c != null && c.IsNegative)
                rc.Report.Error(0, loc, "Cannot use a negative size with stackalloc");
            

            if (rc.HasAny(ResolveContext.Options.CatchScope | ResolveContext.Options.FinallyScope))
                rc.Report.Error(0, loc, "Cannot use stackalloc in finally or catch");


            otype = texpr.ResolveAsType(rc);
            if (otype == null)
                return null;

            ResolvedType = new PointerTypeSpec(otype);
            _resolved = true;
            eclass = ExprClass.Value;

            return this;
        }
    }
}