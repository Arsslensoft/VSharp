using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
   public class BinaryExpression : Expression
    {
       public Expression _left;
       public Expression _right;
       public Type _right_type;

       public ResolveResult _resolved_left;
       public ResolveResult _resolved_right;
       public BinaryOperatorType _operator;
       public ResolveResult _result;

       public override object DoResolve(Context.ResolveContext rc)
       {
             _result = rc.Resolve(this);
           return this;
       }
    }
}
