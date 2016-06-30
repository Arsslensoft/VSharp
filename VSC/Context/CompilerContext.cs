using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC.Context
{
    public class CompilerContext
    {
        public CompilerContext(CompilerSettings settings = null)
        {
            Settings = settings;
            Report = new Report(this, new ConsoleReportPrinter());
        }
        public Report Report { get; set; }
        public CompilerSettings Settings { get; set; }
    }
}
