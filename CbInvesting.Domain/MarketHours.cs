using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain
{
    public class MarketHours
    {
        public string Description { get; set; }
        public bool IsOpen { get; set; }
        public DateTime PreMarketStart { get; set; }
        public DateTime PreMarketEnd { get; set; }
        public DateTime RegularMarketStart { get; set; }
        public DateTime RegularMarketEnd { get; set; }
        public DateTime PostMarketStart { get; set; }
        public DateTime PostMarketEnd { get; set; }
    }
}
