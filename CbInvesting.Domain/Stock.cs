using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain
{
    public class Stock
    {
        public string Symbol { get; set; }
        public string Description { get; set; }
        public string ExchangeName { get; set; }
        public int Volume { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal LastPrice { get; set; }
        public decimal AskPrice { get; set; }
        public decimal BidPrice { get; set; }
        public decimal NetChange { get; set; }
        public double NetPercentChangeInDouble { get; set; }
        public decimal RegularMarketLastPrice { get; set; }
        public double RegularMarketPercentChangeInDouble { get; set; }
        public double LastChangePct { get; set; }
    }
}
