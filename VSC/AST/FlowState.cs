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
}