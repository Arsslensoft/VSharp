namespace VSC
{
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
}