using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VSC
{
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
}