using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class NullLiteral : NullConstant
    {
        //
        // Default type of null is an object
        //
        public NullLiteral(Location loc)
            : base(loc)
        {
        }

        public override string GetValueAsLiteral()
        {
            return "null";
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

 
    }
}
