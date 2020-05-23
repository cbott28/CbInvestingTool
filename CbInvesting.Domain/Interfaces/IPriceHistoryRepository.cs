using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Interfaces
{
    public interface IPriceHistoryRepository
    {
        List<TradingDay> GetPriceHistory(string symbol, DateTime startdate, DateTime endDate);
    }
}
