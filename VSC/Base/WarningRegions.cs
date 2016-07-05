using System.Collections.Generic;
using VSC.Context;

namespace VSC
{
    /// <summary>
    /// Handles #pragma warning
    /// </summary>
    public class WarningRegions {

        abstract class PragmaCmd
        {
            public int Line;

            protected PragmaCmd (int line)
            {
                Line = line;
            }

            public abstract bool IsEnabled (int code, bool previous);
        }
		
        class Disable : PragmaCmd
        {
            int code;
            public Disable (int line, int code)
                : base (line)
            {
                this.code = code;
            }

            public override bool IsEnabled (int code, bool previous)
            {
                return this.code != code && previous;
            }
        }

        class DisableAll : PragmaCmd
        {
            public DisableAll (int line)
                : base (line) {}

            public override bool IsEnabled(int code, bool previous)
            {
                return false;
            }
        }

        class Enable : PragmaCmd
        {
            int code;
            public Enable (int line, int code)
                : base (line)
            {
                this.code = code;
            }

            public override bool IsEnabled(int code, bool previous)
            {
                return this.code == code || previous;
            }
        }

        class EnableAll : PragmaCmd
        {
            public EnableAll (int line)
                : base (line) {}

            public override bool IsEnabled(int code, bool previous)
            {
                return true;
            }
        }


        List<PragmaCmd> regions = new List<PragmaCmd> ();

        public void WarningDisable (int line)
        {
            regions.Add (new DisableAll (line));
        }

        public void WarningDisable (Location location, int code, Report Report)
        {
            if (Report.CheckWarningCode (code, location))
                regions.Add (new Disable (location.Line, code));
        }

        public void WarningEnable (int line)
        {
            regions.Add (new EnableAll (line));
        }

        public void WarningEnable (Location location, int code, CompilerContext context)
        {
            if (!context.Report.CheckWarningCode (code, location))
                return;

            if (context.Report.IsWarningDisabledGlobally (code))
                context.Report.Warning (1635, 1, location, "Cannot restore warning `VS{0:0000}' because it was disabled globally", code);

            regions.Add (new Enable (location.Line, code));
        }

        public bool IsWarningEnabled (int code, int src_line)
        {
            bool result = true;
            foreach (PragmaCmd pragma in regions) {
                if (src_line < pragma.Line)
                    break;

                result = pragma.IsEnabled (code, result);
            }
            return result;
        }
    }
}