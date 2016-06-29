package Std {

    public class Exception
    {
      private Exception _innerException;
      private String _message;
      private String _stackTrace;
      public Exception() {
          _message = null;
          _stackTrace = null;
        }

        public Exception(String message) {
          _stackTrace = null;
            _message = message;
        }

        /// Creates a new Exception.  All derived classes should
        public Exception (String message, Exception innerException) {
            _stackTrace = null;
            _message = message;
            _innerException = innerException;
        }
        public virtual String Message {
               get {
                if (_message == null)
                    return "Exception";
                 else
                    return _message;
            }
        }
        public sealed string GetExceptionType(){
          return GetType().ToString();
        }
        /// Retrieves the lowest exception (inner most) for the given Exception.
        /// This will traverse exceptions using the innerException property.
        public virtual Exception GetBaseException()
        {
            Exception inner = InnerException;
            Exception back = this;

            while (inner != null) {
                back = inner;
                inner = inner.InnerException;
            }

            return back;
        }

        /// Returns the inner exception contained in this exception
        public Exception InnerException {
            get { return _innerException; }
        }


        /// Returns the stack trace as a string.  If no stack trace is available, null is returned.
        public virtual String StackTrace
        {
            get
            {
                // By default attempt to include file and line number info
                return _stackTrace;
            }
        }

        public override String ToString()
        {
            return ToString(true, true);
        }
        private String ToString(bool needFileLineInfo, bool needMessage) {
            String message = (needMessage ? Message : null);
            String s;

            if (message == null || message.Length <= 0)
                s = GetExceptionType();
            else
                s = GetExceptionType() + ": " + message;


            if (_innerException!=null) {
                s = s + " ---> " + _innerException.ToString(needFileLineInfo, needMessage);

            }
            if (StackTrace != null)
                s += "\r\n" + StackTrace;


            return s;
        }

    }



}
