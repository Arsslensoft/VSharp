package Std {

    public class InvalidOperationException : Exception
    {
        public InvalidOperationException()
        {
        }

        public InvalidOperationException(String message)
            : super(message) {

        }

        public InvalidOperationException(String message, Exception innerException)
            : super(message, innerException) {

        }
    }
}
