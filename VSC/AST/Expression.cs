using System;
using VSC.AST;
using VSC.Base.GoldParser.Semantic;
using VSC.Context;

namespace VSC.AST
{
   
    public abstract class Expression : Semantic, IResolve, IEmitExpression, IFlow, IAstCloneable
    {
        public virtual bool Emit(EmitContext ec)
        {
            return true;
        }
        public virtual bool EmitFromStack(EmitContext ec)
        {
            return true;
        }
        public virtual bool EmitToStack(EmitContext ec)
        {
            return true;
        }
        public virtual FlowState DoFlowAnalysis(FlowAnalysisContext fc)
        {
            return FlowState.Valid;
        }
        public virtual bool Resolve(ResolveContext rc)
        {
            return true;
        }
        public virtual object DoResolve(ResolveContext rc)
        {
            return this;
        }
        public virtual object CloneTo(CloneContext ctx)
        {
            return this;
        }
    }
}
