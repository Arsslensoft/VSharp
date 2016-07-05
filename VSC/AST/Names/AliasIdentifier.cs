using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class AliasIdentifier
    {
        public string Value;
        public Location Location;

        public AliasIdentifier(string name, Location loc)
        {
            this.Value = name;
            this.Location = loc;
        }
    }
}
