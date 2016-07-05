using System;

namespace VSC
{
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
}