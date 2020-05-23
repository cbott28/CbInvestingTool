using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Interfaces
{
    public interface IMarketRepository
    {
        MarketHours GetMarketHours(DateTime date);
    }
}
