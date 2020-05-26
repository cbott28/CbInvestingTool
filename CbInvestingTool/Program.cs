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
            List<Stock> topStocks = new List<Stock>();
            var processDate = new DateTime(2020, 05, 18);

            foreach (var stock in stocks)
            {
                if (stock.LastPrice >= 1m && stock.LastPrice <= 5m)
                {
                    IEnumerable<TradingDay> priceHistory = priceHistoryService.GetPriceHistory(stock.Symbol, DateTime.Now.AddDays(-365), DateTime.Now).OrderBy(x => x.Date);
                    Analyzer analyzer = new Analyzer(stocks, priceHistory);

                    var volumeAverage = analyzer.GetVolumeAverage(stock.Symbol, processDate, 20);
                    var senokuSpanB = analyzer.GetSenokuSpanB(stock.Symbol, processDate, 52);
                    var vwap = analyzer.GetVwap(stock.Symbol, processDate, 20);

                    if (volumeAverage > 0 && senokuSpanB > 0 && vwap > 0)
                    {
                        var tradingDay = priceHistory.Where(x => x.Date.Date <= processDate.Date).OrderByDescending(x => x.Date).First();
                        var lastTypicalPrice = (tradingDay.HighPrice + tradingDay.LowPrice + tradingDay.ClosePrice) / 3;

                        if (lastTypicalPrice < senokuSpanB 
                            && tradingDay.Volume > (volumeAverage * 0.5) &&
                            tradingDay.OpenPrice > vwap && tradingDay.ClosePrice < vwap)
                        {
                            stock.SpanBPercentage = ((lastTypicalPrice / senokuSpanB));
                            topStocks.Add(stock);
                        }                         
                    }
                }                
            }

            foreach (var stock in topStocks)
            {
                Console.WriteLine("Top Stock: {0}, Pct: {1}", stock.Symbol, stock.SpanBPercentage);
            }
            
            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }
}
