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
            List<Stock> increasedStocks = new List<Stock>();
            //var processDate = DateTime.Now; 
            var processDate = new DateTime(2020, 07, 10);

            foreach (var stock in stocks)
            {
                if (stock.LastPrice >= 1m && stock.LastPrice <= 5m)
                {
                    IEnumerable<TradingDay> priceHistory = priceHistoryService.GetPriceHistory(stock.Symbol, DateTime.Now.AddDays(-365), DateTime.Now).OrderBy(x => x.Date);
                    Analyzer analyzer = new Analyzer(stocks, priceHistory);
                    var volumeAverage = analyzer.GetVolumeAverage(stock.Symbol, processDate, 20);
                    var senokuSpanB = analyzer.GetSenokuSpanB(stock.Symbol, processDate, 52);
                    var vwap = analyzer.GetVwap(stock.Symbol, processDate, 20);
                    decimal percentR = analyzer.GetPercentR(stock.Symbol, processDate, 14);
                    var closeLocationValue = analyzer.GetCloseLocationValue(stock.Symbol, processDate);

                    stock.NetChange = 0m;
                    if (priceHistory.Count() > 0 &&
                        priceHistory.Any(x => x.Date.Date == processDate.Date) &&
                        priceHistory.Any(x => x.Date.Date == processDate.AddDays(-1).Date)) {
                        decimal lastClosePrice = priceHistory.Where(x => x.Date.Date == processDate.AddDays(-1).Date).First().ClosePrice;
                        decimal openPrice = priceHistory.Where(x => x.Date.Date == processDate.Date).First().OpenPrice;

                        if (openPrice > 0m && lastClosePrice > 0m &&
                            openPrice - lastClosePrice > 0m)
                        {
                            var addToList = true;
                            for (int i = priceHistory.Count() - 2; i >= priceHistory.Count() - 10; i--)
                            {
                                if (openPrice <= priceHistory.ElementAt(i).HighPrice)
                                    addToList = false;
                            }

                            if (addToList)
                            {
                                stock.NetChange = openPrice - lastClosePrice;
                                increasedStocks.Add(stock);
                                Console.WriteLine("Symbol: {0}, Change: {1}, PctR: {2}, CLV: {3}, VWAP: {4}, Senoku: {5}", stock.Symbol, stock.NetChange, percentR, closeLocationValue, vwap, senokuSpanB);
                            }
                        }
                    }

                    /*Analyzer analyzer = new Analyzer(stocks, priceHistory);

                    var volumeAverage = analyzer.GetVolumeAverage(stock.Symbol, processDate, 20);
                    var senokuSpanB = analyzer.GetSenokuSpanB(stock.Symbol, processDate, 52);
                    var vwap = analyzer.GetVwap(stock.Symbol, processDate, 20);
                    decimal percentR = analyzer.GetPercentR(stock.Symbol, processDate, 14);
                    var closeLocationValue = analyzer.GetCloseLocationValue(stock.Symbol, processDate);

                    if (volumeAverage > 0 && senokuSpanB > 0 && vwap > 0)
                    {
                        var tradingDay = priceHistory.Where(x => x.Date.Date <= processDate.Date).OrderByDescending(x => x.Date).First();
                        var lastTypicalPrice = (tradingDay.HighPrice + tradingDay.LowPrice + tradingDay.ClosePrice) / 3;

                        if (lastTypicalPrice < senokuSpanB 
                            && tradingDay.Volume > (volumeAverage * 0.5) &&
                            tradingDay.OpenPrice > vwap && tradingDay.ClosePrice < vwap /*&&
                            closeLocationValue > 0)
                        {
                            stock.SpanBPercentage = ((lastTypicalPrice / senokuSpanB));
                            topStocks.Add(stock);
                            Console.WriteLine("Top Stock: {0}, Pct: {1}, CLV: {2}", stock.Symbol, stock.SpanBPercentage, percentR);
                        }                         
                    }*/
                }                
            }

            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }
}
