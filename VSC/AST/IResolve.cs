using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
   public interface IResolveExpression
    {
       Expression DoResolve(ResolveContext rc);
       ResolveResult GetResolveResult(ResolveContext rc);
    }

    public interface IResolve
    {
        bool Resolve(ResolveContext rc);
    }
}
