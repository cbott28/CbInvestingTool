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
            Analyzer analyzer = new Analyzer(stockService, priceHistoryService);

            var volumeAverage = 0;
            var senokuSpanB = 0m;
            var vwap = 0m;
            var lastTypicalPrice = 0m;
            var processDate = DateTime.Now.AddDays(-1);
            foreach (var stock in stocks)
            {
                if (stock.LastPrice >= 1m && stock.LastPrice <= 5m)
                {
                    volumeAverage = analyzer.GetVolumeAverage(stock.Symbol, processDate.AddDays(-20), processDate);
                    senokuSpanB = analyzer.GetSenokuSpanB(stock.Symbol, processDate);
                    vwap = analyzer.GetVwap(stock.Symbol, processDate);
                    List<TradingDay> priceHistory = priceHistoryService.GetPriceHistory(stock.Symbol, processDate, processDate);

                    if (priceHistory.Count > 0)
                    {
                        lastTypicalPrice = (priceHistory.Last().HighPrice + priceHistory.Last().LowPrice + priceHistory.Last().ClosePrice) / 3;

                        if (lastTypicalPrice < senokuSpanB && priceHistory.Last().Volume > (volumeAverage * 0.5) &&
                            priceHistory.Last().OpenPrice > vwap && priceHistory.Last().ClosePrice < vwap)
                            Console.WriteLine("Stock: {0}, Price: {1}, VolAvg: {2}, SenSpanB: {3}, VWAP: {4}", stock.Symbol, lastTypicalPrice, volumeAverage, senokuSpanB, vwap);
                    }
                }                
            }
            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }
}
