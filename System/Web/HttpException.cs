using System.Runtime.Serialization;

namespace System.Web
{
    [Serializable]
    internal class HttpException : Exception
    {
        private int errorNumber;
        private string mess;

        public HttpException()
        {
        }

        public HttpException(string message) : base(message)
        {
        }

        public HttpException(int errorNumber, string mess)
        {
            this.errorNumber = errorNumber;
            this.mess = mess;
        }

        public HttpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}