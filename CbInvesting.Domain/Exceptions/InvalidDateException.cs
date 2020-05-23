using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Exceptions
{
    public class InvalidDateException : Exception, ISerializable
    {
        public InvalidDateException() : base()
        {
        }

        public InvalidDateException(string message) : base(message)
        {
        }

        public InvalidDateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidDateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
