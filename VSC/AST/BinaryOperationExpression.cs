using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.AST
{
   public class BinaryExpression : Expression
    {
       public Expression _left;
       public Expression _right;
       public Type _right_type;
    }
}
