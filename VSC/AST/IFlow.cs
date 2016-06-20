using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;

namespace VSC.AST
{
    public struct FlowState
    {
        public static FlowState Valid = new FlowState(true, new Reachability());
        public static FlowState Unreachable = new FlowState(true, Reachability.CreateUnreachable());

        internal Reachability _reach;
        bool _success;
        public Reachability Reachable { get { return _reach; } set { _reach = value; } }
        public bool Success { get { return _success; } set { _success = value; } }

        public FlowState(bool succ, Reachability rc)
        {
            _reach = rc;
            _success = succ;
        }

        public static FlowState operator &(FlowState a, FlowState b)
        {
            return new FlowState(a._success & b._success, a._reach | b._reach);
        }
        public static FlowState operator |(FlowState a, FlowState b)
        {
            return new FlowState(a._success | b._success, a._reach | b._reach);
        }
    }
    public struct Reachability
    {
        readonly bool unreachable;
        public Location Loc;
        Reachability(bool unreachable)
        {
            Loc = Location.Null;
            this.unreachable = unreachable;
        }

        public bool IsUnreachable
        {
            get
            {
                return unreachable;
            }
        }

        public static Reachability CreateUnreachable()
        {
            return new Reachability(true);
        }

        public static Reachability operator &(Reachability a, Reachability b)
        {
            return new Reachability(a.unreachable && b.unreachable);
        }

        public static Reachability operator |(Reachability a, Reachability b)
        {
            return new Reachability(a.unreachable | b.unreachable);
        }
    }
   public interface IFlow
    {
       FlowState DoFlowAnalysis(FlowAnalysisContext fc);
    }
}
