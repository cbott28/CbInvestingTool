﻿using CbInvesting.Domain.Interfaces;
using CbInvesting.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain
{
    public class Analyzer
    {
        private StockService _stockService;
        private PriceHistoryService _priceHistoryService;

        public Analyzer(StockService stockService, PriceHistoryService priceHistoryService)
        {
            _stockService = stockService;
            _priceHistoryService = priceHistoryService;
        }

        public int GetVolumeAverage(string symbol, DateTime startDate, DateTime endDate)
        {
            startDate = formatDate(startDate);
            endDate = formatDate(endDate);

            List<TradingDay> priceHistory = _priceHistoryService.GetPriceHistory(symbol.ToUpper(), startDate, endDate);

            int volumeSum = 0;
            foreach (var tradingDay in priceHistory)
            {
                volumeSum += tradingDay.Volume;
            }

            if (priceHistory.Count == 0) return 0;
            else return volumeSum / priceHistory.Count;
        }

        public decimal GetSenokuSpanB(string symbol, DateTime processDate)
        {
            DateTime startDate = formatDate(processDate.AddDays(-52));
            DateTime endDate = formatDate(processDate);

            List<TradingDay> priceHistory = _priceHistoryService.GetPriceHistory(symbol.ToUpper(), startDate, endDate);

            decimal highestPrice = 0.00m;
            decimal lowestPrice = 0.00m;
            foreach (var tradingDay in priceHistory)
            {
                if (tradingDay.ClosePrice > highestPrice)
                    highestPrice = tradingDay.ClosePrice;

                if (lowestPrice == 0.00m || tradingDay.ClosePrice < lowestPrice)
                    lowestPrice = tradingDay.ClosePrice;
            }

            return (highestPrice + lowestPrice) / 2;
        }

        public decimal GetVwap(string symbol, DateTime processDate)
        {
            DateTime startDate = formatDate(processDate.AddDays(-20));
            DateTime endDate = formatDate(processDate);

            List<TradingDay> priceHistory = _priceHistoryService.GetPriceHistory(symbol.ToUpper(), startDate, endDate);

            decimal cumulativeTpv = 0m;
            decimal cumulativeVolume = 0m;
            foreach (var tradingDay in priceHistory)
            {
                cumulativeTpv += ((tradingDay.HighPrice + tradingDay.LowPrice + tradingDay.ClosePrice) / 3) * tradingDay.Volume;
                cumulativeVolume += tradingDay.Volume;
            }

            if (cumulativeVolume == 0m) return 0m;
            else return cumulativeTpv / cumulativeVolume;
        }

        public List<Stock> GetLowPointStocks(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Stock> stocks = _stockService.GetStocks().Where(s => s.LastPrice >= 1.00m && s.AskPrice <= 5.00m);;
            List<Stock> lowPointStocks = new List<Stock>();

            var timeOfDay = new TimeSpan(5, 0, 0); //5AM EST
            startDate = startDate.Date + timeOfDay;
            endDate = endDate.Date + timeOfDay;

            foreach (var stock in stocks)
            {
                List<TradingDay> priceHistory = _priceHistoryService.GetPriceHistory(stock.Symbol, startDate, endDate);

                if (priceHistory.Count == 0)
                {
                    System.Threading.Thread.Sleep(60000);
                    priceHistory = _priceHistoryService.GetPriceHistory(stock.Symbol, startDate, endDate);
                }
                else
                {
                    stock.LastChangePct = (double)((priceHistory.Last().ClosePrice - priceHistory.Last().OpenPrice) / priceHistory.Last().OpenPrice);
                    decimal lowestPrice = getLowestTradingPrice(priceHistory);

                    if (priceHistory.Count >= ((endDate - startDate).TotalDays / 2) && //has history for at least half the days
                        stock.LastPrice <= (lowestPrice += (lowestPrice * 0.10m))) //within 10% of lowest price
                        lowPointStocks.Add(stock);
                }
            }

            lowPointStocks = lowPointStocks.OrderBy(s => s.LastChangePct).ToList();
            return lowPointStocks;
        }

        private decimal getLowestTradingPrice(List<TradingDay> priceHistory)
        {
            DateTime worstTradingDay = new DateTime();
            decimal lowestPrice = 0.00m;

            foreach (var tradingDay in priceHistory)
            {
                if (worstTradingDay == DateTime.MinValue || tradingDay.ClosePrice < lowestPrice)
                {
                    worstTradingDay = tradingDay.Date;
                    lowestPrice = tradingDay.ClosePrice;
                }
            }

            return lowestPrice;
        }

        private DateTime formatDate(DateTime date)
        {
            var timeOfDay = new TimeSpan(5, 0, 0); //5AM EST
            return date.Date + timeOfDay;
        }
    }
}
