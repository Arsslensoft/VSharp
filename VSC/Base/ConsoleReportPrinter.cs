using System;
using System.IO;

namespace VSC
{
    public class ConsoleReportPrinter : StreamReportPrinter
    {
		

        public ConsoleReportPrinter ()
            : base (Console.Error)
        {
        }

        public ConsoleReportPrinter (TextWriter writer)
            : base (writer)
        {
        }
	
    }
}