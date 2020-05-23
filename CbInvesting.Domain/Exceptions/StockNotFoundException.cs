using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Exceptions
{
    public class StockNotFoundException : Exception, ISerializable
    {
        public StockNotFoundException() : base()
        {
        }

        public StockNotFoundException(string message) : base(message)
        {
        }

        public StockNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StockNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
