using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.AST
{
   public abstract class Statement
    {
        public Location loc;
        protected bool reachable;

        public bool IsUnreachable
        {
            get
            {
                return !reachable;
            }
        }

     
    }
}
