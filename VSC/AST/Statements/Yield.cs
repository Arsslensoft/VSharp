using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.AST
{
    public class Yield : YieldStatement
    {
        public Yield(Expression expr, Location loc)
            : base(expr, loc)
        {
        }
    }
}
