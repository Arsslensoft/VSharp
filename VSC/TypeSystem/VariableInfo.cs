using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.TypeSystem
{	
    // An expressions resolved as a direct variable reference
    //
    public interface IVariableReference 
    {
        bool IsHoisted { get; }
        string Name { get; }
        VariableInfo VariableInfo { get; }

        void SetHasAddressTaken();
    }
    // TODO:implement vinfo
  public  class VariableInfo
    {
    }
}
