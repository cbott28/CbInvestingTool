using CbInvesting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Services
{
    public class MarketService
    {
        private IMarketRepository _marketRepository;

        public MarketService(IMarketRepository marketRepository)
        {
            _marketRepository = marketRepository;
        }

        public MarketHours GetMarketHours(DateTime date)
        {
            return _marketRepository.GetMarketHours(date);
        }
    }
}
