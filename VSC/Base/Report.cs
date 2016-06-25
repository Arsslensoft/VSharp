using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
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

	public abstract class AbstractMessage
	{
	
		protected readonly int code;
		protected readonly Location location;
		readonly string message;

		protected AbstractMessage (int code, Location loc, string msg)
		{
			this.code = code;
			if (code < 0)
				this.code = 8000 - code;

			this.location = loc;
			this.message = msg;
	
			
		}

		protected AbstractMessage (AbstractMessage aMsg)
		{
			this.code = aMsg.code;
			this.location = aMsg.location;
			this.message = aMsg.message;

		}

		public int Code {
			get { return code; }
		}

		public override bool Equals (object obj)
		{
			AbstractMessage msg = obj as AbstractMessage;
			if (msg == null)
				return false;

			return code == msg.code && location.Equals (msg.location) && message == msg.message;
		}

		public override int GetHashCode ()
		{
			return code.GetHashCode ();
		}

		public abstract bool IsWarning { get; }

		public Location Location {
			get { return location; }
		}

		public abstract string MessageType { get; }


		public string Text {
			get { return message; }
		}
	}

    public  sealed class WarningMessage : AbstractMessage
	{
		public WarningMessage (int code, Location loc, string message)
			: base (code, loc, message)
		{
		}

		public override bool IsWarning {
			get { return true; }
		}

		public override string MessageType {
			get {
				return "warning";
			}
		}
	}

	public sealed class ErrorMessage : AbstractMessage
	{
		public ErrorMessage (int code, Location loc, string message)
			: base (code, loc, message)
		{
		}

		public ErrorMessage (AbstractMessage aMsg)
			: base (aMsg)
		{
		}

		public override bool IsWarning {
			get { return false; }
		}

		public override string MessageType {
			get {
				return "error";
			}
		}
	}

	//
	// Generic base for any message writer
	//
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

	sealed class NullReportPrinter : ReportPrinter
	{
	}


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

	class TimeReporter
	{
		public enum TimerType
		{
			ParseTotal,
			SetupTotal,
			SemanticTotal,
			ResolveTotal,
			FlowAnalysisTotal,
			EmitTotal,
			CodeGenerationTotal
		}

		readonly Stopwatch[] timers;
		Stopwatch total;

		public TimeReporter (bool enabled)
		{
			if (!enabled)
				return;

			timers = new Stopwatch[System.Enum.GetValues(typeof (TimerType)).Length];
		}

		public void Start (TimerType type)
		{
			if (timers != null) {
				var sw = new Stopwatch ();
				timers[(int) type] = sw;
				sw.Start ();
			}
		}

		public void StartTotal ()
		{
			total = new Stopwatch ();
			total.Start ();
		}

		public void Stop (TimerType type)
		{
			if (timers != null) {
				timers[(int) type].Stop ();
			}
		}

		public void StopTotal ()
		{
			total.Stop ();
		}

		public void ShowStats ()
		{
			if (timers == null)
				return;

			Dictionary<TimerType, string> timer_names = new Dictionary<TimerType,string> {
				{ TimerType.ParseTotal, "Parsing source files" },
				{ TimerType.SetupTotal, "Compiler setup" },
				{ TimerType.ResolveTotal, "Resolving members and blocks" },
				{ TimerType.SemanticTotal, "Semantic analysis" },
				{ TimerType.FlowAnalysisTotal, "Flow analysis" },
				{ TimerType.EmitTotal, "Emitting members and blocks" },
				{ TimerType.CodeGenerationTotal, "Code generation" },
			};

			int counter = 0;
			double percentage = (double) total.ElapsedMilliseconds / 100;
			long subtotal = total.ElapsedMilliseconds;
			foreach (var timer in timers) {
				string msg = timer_names[(TimerType) counter++];
				var ms = timer == null ? 0 : timer.ElapsedMilliseconds;
				Console.WriteLine ("{0,4:0.0}% {1,5}ms {2}", ms / percentage, ms, msg);
				subtotal -= ms;
			}

			Console.WriteLine ("{0,4:0.0}% {1,5}ms Other tasks", subtotal / percentage, subtotal);
			Console.WriteLine ();
			Console.WriteLine ("Total elapsed time: {0}", total.Elapsed);
		}
	}

	public class InternalErrorException : Exception {

		public InternalErrorException ()
			: base ("Internal error")
		{
		}

		public InternalErrorException (string message)
			: base (message)
		{
		}

		public InternalErrorException (string message, params object[] args)
			: base (String.Format (message, args))
		{
		}

		public InternalErrorException (Exception exception, string message, params object[] args)
			: base (String.Format (message, args), exception)
		{
		}
		
		public InternalErrorException (Exception e, Location loc)
			: base (loc.ToString (), e)
		{
		}
	}

	class FatalException : Exception
	{
		public FatalException (string message)
			: base (message)
		{
		}
	}

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
