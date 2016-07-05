using VSC.Context;

namespace VSC.AST
{
    public interface IEmitExpression  : IEmit
    {
        bool EmitToStack(EmitContext ec);
        bool EmitFromStack(EmitContext ec);    
    }
}