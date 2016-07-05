namespace VSC
{
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
}