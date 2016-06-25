using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base.GoldParser.Semantic;

namespace VSC.AST
{
    public class SelfExpression : Expression
    {
        [Rule("<accessible primary expression> ::= ~self")]
        public SelfExpression()
        {

        }
    }
}
