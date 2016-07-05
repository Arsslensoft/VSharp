using System.IO;
using System.Text;

namespace VSC
{
    public abstract class ReportPrinter
    {

        #region Properties

        public int ErrorsCount { get; protected set; }
		
        public int WarningsCount { get; private set; }
	
		
        #endregion


        protected virtual string FormatText (string txt)
        {
            return txt;
        }

        public virtual void Print (AbstractMessage msg, bool showFullPath)
        {
            if (msg.IsWarning) {
                ++WarningsCount;
            } else {
                ++ErrorsCount;
            }
        }

        protected void Print (AbstractMessage msg, TextWriter output, bool showFullPath)
        {
            StringBuilder txt = new StringBuilder ();
            if (!msg.Location.IsNull) {
                if (showFullPath)
                    txt.Append (msg.Location.ToStringFullName ());
                else
                    txt.Append (msg.Location.ToString ());

                txt.Append (" ");
            }

            txt.AppendFormat ("{0} VS{1:0000}: {2}", msg.MessageType, msg.Code, msg.Text);

            if (!msg.IsWarning)
                output.WriteLine (FormatText (txt.ToString ()));
            else
                output.WriteLine (txt.ToString ());


			
        }

		

        public void Reset ()
        {
            // HACK: Temporary hack for broken repl flow
            ErrorsCount = WarningsCount = 0;
        }
    }
}