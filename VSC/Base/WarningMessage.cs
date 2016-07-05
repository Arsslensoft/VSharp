namespace VSC
{
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
}