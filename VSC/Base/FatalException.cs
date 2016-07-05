using System;

namespace VSC
{
    class FatalException : Exception
    {
        public FatalException (string message)
            : base (message)
        {
        }
    }
}