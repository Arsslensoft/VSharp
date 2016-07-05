using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class QueryExpression : AQueryClause
    {
        public QueryExpression(AQueryClause start)
            : base(null, null, start.Location)
        {
            this.next = start;
        }

        protected override string MethodName
        {
            get { throw new NotSupportedException(); }
        }
    }

    //
    // A query clause with an identifier (range variable)
    //

    //
    // Implicit query block
    //
}
