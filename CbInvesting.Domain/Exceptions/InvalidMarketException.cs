using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Exceptions
{
    public class InvalidMarketException : Exception, ISerializable
    {
        public InvalidMarketException() : base()
        {
        }

        public InvalidMarketException(string message) : base(message)
        {
        }

        public InvalidMarketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidMarketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
