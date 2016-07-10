using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSC
{
  public  class ListReportPrinter : ReportPrinter
    {
      public ListReportPrinter()
      {
          Messages = new List<AbstractMessage>();
      }
      public List<AbstractMessage> Messages { get; set; }
      public override void Print(AbstractMessage msg, bool showFullPath)
      {
          Messages.Add(msg);
      }
    }
}
