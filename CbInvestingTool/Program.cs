using CbInvesting.Domain;
using CbInvesting.Domain.Interfaces;
using CbInvesting.Domain.Services;
using CbInvestingTool.Persistence;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvestingTool
{
    class Program
    {
        static void Main(string[] args)
        {
            StockRepository stockRepository = new StockRepository();
            StockService stockService = new StockService(stockRepository);
            PriceHistoryRepository priceHistoryRepository = new PriceHistoryRepository();
            PriceHistoryService priceHistoryService = new PriceHistoryService(priceHistoryRepository);

            List<Stock> stocks = stockService.GetStocks();

            var volumeAverage = 0;
            var senokuSpanB = 0m;
            var vwap = 0m;
            var lastTypicalPrice = 0m;
            var processDate = DateTime.Now.AddDays(-1);
            foreach (var stock in stocks)
            {
                if (stock.LastPrice >= 1m && stock.LastPrice <= 5m)
                {
                    IEnumerable<TradingDay> priceHistory = priceHistoryService.GetPriceHistory(stock.Symbol, processDate.AddDays(-365), processDate).OrderBy(x => x.Date);
                    Analyzer analyzer = new Analyzer(stocks, priceHistory);

                    volumeAverage = analyzer.GetVolumeAverage(stock.Symbol, 20);
                    senokuSpanB = analyzer.GetSenokuSpanB(stock.Symbol);
                    vwap = analyzer.GetVwap(stock.Symbol);

                    if (priceHistory.Count() > 0)
                    {
                        lastTypicalPrice = (priceHistory.Last().HighPrice + priceHistory.Last().LowPrice + priceHistory.Last().ClosePrice) / 3;

                        if (lastTypicalPrice > senokuSpanB && lastTypicalPrice > vwap && priceHistory.Last().Volume > (volumeAverage * 0.5) &&
                            priceHistory.Last().OpenPrice < vwap && priceHistory.Last().ClosePrice > vwap)
                         Console.WriteLine("Stock: {0}, Price: {1}, VolAvg: {2}, SenSpanB: {3}, VWAP: {4}", stock.Symbol, lastTypicalPrice, volumeAverage, senokuSpanB, vwap);
                    }
                }                
            }
            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }
}
