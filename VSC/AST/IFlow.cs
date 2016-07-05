using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;

namespace VSC.AST
{
    public interface IFlow
    {
       FlowState DoFlowAnalysis(FlowAnalysisContext fc);
    }
}
