using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;

namespace VSC.AST
{
    public interface IEmit
    {
        bool Emit(EmitContext ec);
       
    }
  public interface IEmitExpression  : IEmit
    {
      bool EmitToStack(EmitContext ec);
      bool EmitFromStack(EmitContext ec);    
    }

    
}
