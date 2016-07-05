namespace VSC.AST
{
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
}