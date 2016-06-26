using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;

namespace VSC.AST
{
   public interface IResolve
    {
       object DoResolve(ResolveContext rc);
       bool Resolve(SymbolResolveContext rc);
    }
}
