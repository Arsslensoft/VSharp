using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.AST
{
    public abstract class YieldStatement : ResumableStatement
    {
        protected Expression expr;

        protected YieldStatement(Expression expr, Location l)
        {
            this.expr = expr;
            loc = l;
        }

        public Expression Expr
        {
            get { return this.expr; }
        }
    }
    public class Yield : YieldStatement
    {
        public Yield(Expression expr, Location loc)
            : base(expr, loc)
        {
        }
    }
    public class YieldBreak : ExitStatement
    {


        public YieldBreak(Location l)
        {
            loc = l;
        }

        protected override bool IsLocalExit
        {
            get
            {
                return false;
            }
        }
    }
}
