using CbInvesting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Services
{
    public class PriceHistoryService
    {
        private IPriceHistoryRepository _priceHistoryRepository;

        public PriceHistoryService(IPriceHistoryRepository priceHistoryRepository)
        {
            _priceHistoryRepository = priceHistoryRepository;
        }

        public List<TradingDay> GetPriceHistory(string symbol, DateTime startDate, DateTime endDate)
        {
            List<TradingDay> priceHistory = _priceHistoryRepository.GetPriceHistory(symbol, startDate, endDate);

            if (priceHistory.Count == 0)
            {
                System.Threading.Thread.Sleep(60000);
                priceHistory = _priceHistoryRepository.GetPriceHistory(symbol, startDate, endDate);
            }

            return priceHistory;
        }
    }
}
