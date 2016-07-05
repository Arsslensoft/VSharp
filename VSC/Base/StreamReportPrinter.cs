using System.IO;

namespace VSC
{
    public class StreamReportPrinter : ReportPrinter
    {
        readonly TextWriter writer;

        public StreamReportPrinter (TextWriter writer)
        {
            this.writer = writer;
        }

        public override void Print (AbstractMessage msg, bool showFullPath)
        {
            Print (msg, writer, showFullPath);
            base.Print (msg, showFullPath);
        }
    }
}