using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.TypeSystem
{
    [Flags]
    public enum CallingConventions
    {
        Standard = 1,
        VarArgs = 2,
        Any = 3,
        HasThis = 4

    }
}
