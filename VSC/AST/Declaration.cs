using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;

namespace VSC.AST
{
    public abstract class ResolvableSemantic : Semantic, IResolve
    {

        public virtual bool Resolve(SymbolResolveContext rc)
        {
            return true;
        }
        public virtual object DoResolve(ResolveContext rc)
        {
            return this;
        }
  
    }
    public abstract class Declaration : Semantic, IFlow, IEmit, IResolve
    {

        public virtual bool Emit(EmitContext ec)
        {
            return true;
        }
        public virtual FlowState DoFlowAnalysis(FlowAnalysisContext fc)
        {
            return FlowState.Valid;
        }
        public virtual bool Resolve(SymbolResolveContext rc)
        {
            return true;
        }
        public virtual object DoResolve(ResolveContext rc)
        {
            return this;
        }
      
    }
}
