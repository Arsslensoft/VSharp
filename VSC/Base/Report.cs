using System;
using System.Collections.Generic;
using VSC.Context;
using System.Linq;

namespace VSC {

	//
	// Errors and warnings manager
	//
	public class Report
	{
		public const int RuntimeErrorId = 10000;
		Dictionary<int, WarningRegions> warning_regions_table;
		ReportPrinter printer;
		bool reporting_disabled = false;
		readonly CompilerSettings settings;
        public const int FatalErrorCount = 50;

		// 
		// IF YOU ADD A NEW WARNING YOU HAVE TO ADD ITS ID HERE
		//
		public static readonly int[] AllWarnings = new int[] {
			1
		};

		static HashSet<int> AllWarningsHashSet;

		public Report (CompilerContext context, ReportPrinter printer)
		{
			if (context == null)
				throw new ArgumentNullException ("settings");
			if (printer == null)
				throw new ArgumentNullException ("printer");

			this.settings = context.Settings;
			this.printer = printer;
		}

		public void DisableReporting ()
		{
            reporting_disabled = true;
		}
		public void EnableReporting ()
		{
            reporting_disabled = false;
		}

		public void FeatureIsNotAvailable (CompilerContext compiler, Location loc, string feature)
		{
			string version;
			switch (compiler.Settings.Version) {
			case LanguageVersion.V_1:
				version = "1.0";
				break;
		
			default:
				throw new InternalErrorException ("Invalid feature version", compiler.Settings.Version);
			}

			Error (1644, loc,
				"Feature `{0}' cannot be used because it is not part of the V# {1} language specification",
				      feature, version);
		}
		public void FeatureIsNotSupported (Location loc, string feature)
		{
			Error (1644, loc,
				"Feature `{0}' is not supported in V# compiler.",
				feature);
		}	        
		public void RuntimeMissingSupport (Location loc, string feature) 
		{
			Error (-88, loc, "Your V# Runtime does not support `{0}'.", feature);
		}

		public bool CheckWarningCode (int code, Location loc)
		{
			if (AllWarningsHashSet == null)
				AllWarningsHashSet = new HashSet<int> (AllWarnings);

			if (AllWarningsHashSet.Contains (code))
				return true;

			Warning (1691, 1, loc, "`{0}' is not a valid warning number", code);
			return false;
		}

		public WarningRegions RegisterWarningRegion (Location location)
		{
			WarningRegions regions;
			if (warning_regions_table == null) {
				regions = null;
				warning_regions_table = new Dictionary<int, WarningRegions> ();
			} else {
				warning_regions_table.TryGetValue (location.File.Index, out regions);
			}

			if (regions == null) {
				regions = new WarningRegions ();
				warning_regions_table.Add (location.File.Index, regions);
			}

			return regions;
		}
        public bool IsWarningEnabled(int code, int level)
        {
            if (settings.WarningLevel < level)
                return false;

            return !IsWarningDisabledGlobally(code);
        }
        public bool IsWarningDisabledGlobally(int code)
        {
            return settings.DisabledWarnings != null && settings.DisabledWarnings.Contains(code);
        }
        public bool IsWarningAsError(int code)
        {
            bool is_error = settings.WarningsAreErrors;

            // Check specific list
            if (settings.WarningsAsErrors != null)
                is_error |= settings.WarningsAsErrors.Contains(code);

        

            return is_error;
        }

		public void Warning (int code, int level, Location loc, string message)
		{
			if (reporting_disabled)
				return;

			if (!IsWarningEnabled (code, level))
				return;

			if (warning_regions_table != null && !loc.IsNull) {
				WarningRegions regions;
				if (warning_regions_table.TryGetValue (loc.File.Index, out regions) && !regions.IsWarningEnabled (code, loc.Line))
					return;
			}

			AbstractMessage msg;
			if (IsWarningAsError (code)) {
				message = "Warning as Error: " + message;
				msg = new ErrorMessage (code, loc, message);
			} else {
				msg = new WarningMessage (code, loc, message);
			}
			printer.Print (msg, true);
		}

		public void Warning (int code, int level, Location loc, string format, string arg)
		{
			Warning (code, level, loc, String.Format (format, arg));
		}

		public void Warning (int code, int level, Location loc, string format, string arg1, string arg2)
		{
			Warning (code, level, loc, String.Format (format, arg1, arg2));
		}

		public void Warning (int code, int level, Location loc, string format, params object[] args)
		{
			Warning (code, level, loc, String.Format (format, args));
		}

		public void Warning (int code, int level, string message)
		{
			Warning (code, level, Location.Null, message);
		}

		public void Warning (int code, int level, string format, string arg)
		{
			Warning (code, level, Location.Null, format, arg);
		}

		public void Warning (int code, int level, string format, string arg1, string arg2)
		{
			Warning (code, level, Location.Null, format, arg1, arg2);
		}

		public void Warning (int code, int level, string format, params string[] args)
		{
			Warning (code, level, Location.Null, String.Format (format, args));
		}

		//
		// Warnings encountered so far
		//
		public int Warnings {
			get { return printer.WarningsCount; }
		}

		public void Error (int code, Location loc, string error)
		{
			if (reporting_disabled)
				return;

			ErrorMessage msg = new ErrorMessage (code, loc, error);

			printer.Print (msg, true);


            if (printer.ErrorsCount == FatalErrorCount)
				throw new FatalException (msg.Text);
		}

		public void Error (int code, Location loc, string format, string arg)
		{
			Error (code, loc, String.Format (format, arg));
		}

		public void Error (int code, Location loc, string format, string arg1, string arg2)
		{
			Error (code, loc, String.Format (format, arg1, arg2));
		}

		public void Error (int code, Location loc, string format, params string[] args)
		{
			Error (code, loc, String.Format (format, args));
		}

		public void Error (int code, string error)
		{
			Error (code, Location.Null, error);
		}

		public void Error (int code, string format, string arg)
		{
			Error (code, Location.Null, format, arg);
		}

		public void Error (int code, string format, string arg1, string arg2)
		{
			Error (code, Location.Null, format, arg1, arg2);
		}

		public void Error (int code, string format, params string[] args)
		{
			Error (code, Location.Null, String.Format (format, args));
		}

		//
		// Errors encountered so far
		//
		public int Errors {
			get { return printer.ErrorsCount; }
		}

		public bool IsDisabled {
			get {
				return reporting_disabled;
			}
		}

		public ReportPrinter Printer {
			get { return printer; }
		}

		public ReportPrinter SetPrinter (ReportPrinter printer)
		{
			ReportPrinter old = this.printer;
			this.printer = printer;
			return old;
		}
		
	}

    //
	// Generic base for any message writer
	//
}
