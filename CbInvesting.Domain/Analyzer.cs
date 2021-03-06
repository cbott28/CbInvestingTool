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
        private List<Stock> _stocks;
        private IEnumerable<TradingDay> _priceHistory;

        public Analyzer(List<Stock> stocks, IEnumerable<TradingDay> priceHistory)
        {
            _stocks = stocks;
            _priceHistory = priceHistory;
        }

        public int GetVolumeAverage(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            priceHistory = getPriceHistorySpan(numberOfDays);

            int volumeSum = 0;
            foreach (var tradingDay in priceHistory)
            {
                volumeSum += tradingDay.Volume;
            }

            if (priceHistory.Count() < numberOfDays) return 0;
            else return volumeSum / priceHistory.Count();
        }

        public decimal GetSenokuSpanB(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            priceHistory = getPriceHistorySpan(numberOfDays);

            decimal highestPrice = 0.00m;
            decimal lowestPrice = 0.00m;
            foreach (var tradingDay in priceHistory)
            {
                if (tradingDay.ClosePrice > highestPrice)
                    highestPrice = tradingDay.ClosePrice;

                if (lowestPrice == 0.00m || tradingDay.ClosePrice < lowestPrice)
                    lowestPrice = tradingDay.ClosePrice;
            }

            if (priceHistory.Count() < numberOfDays) return 0;
            else return (highestPrice + lowestPrice) / 2;
        }

        public decimal GetVwap(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date.Date <= processDate.Date);
            priceHistory = getPriceHistorySpan(numberOfDays);

            decimal cumulativeTpv = 0m;
            decimal cumulativeVolume = 0m;
            foreach (var tradingDay in priceHistory)
            {
                cumulativeTpv += ((tradingDay.HighPrice + tradingDay.LowPrice + tradingDay.ClosePrice) / 3) * tradingDay.Volume;
                cumulativeVolume += tradingDay.Volume;
            }

            if (priceHistory.Count() == 0) return 0m;
            else return cumulativeTpv / cumulativeVolume;
        }

        public decimal GetPercentR(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            priceHistory = getPriceHistorySpan(numberOfDays);

            if (priceHistory.Count() == numberOfDays)
            {
                decimal highestPrice = GetHighestPrice(symbol, processDate, numberOfDays);
                decimal lowestPrice = GetLowestPrice(symbol, processDate, numberOfDays);
                decimal closePrice = priceHistory.Last().ClosePrice;

                return ((highestPrice - closePrice) / (highestPrice - lowestPrice)) * 100;
            }
            else return 0m;
        }

        public decimal GetCloseLocationValue(string symbol, DateTime processDate)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            TradingDay tradingDay = priceHistory.Last();

            if (tradingDay.HighPrice - tradingDay.LowPrice == 0) return 0;
            else
                return ((tradingDay.ClosePrice - tradingDay.LowPrice) - (tradingDay.HighPrice - tradingDay.ClosePrice)) / (tradingDay.HighPrice - tradingDay.LowPrice);
        }

        public decimal GetAveragePrice(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            priceHistory = getPriceHistorySpan(numberOfDays);

            if (priceHistory.Count() == numberOfDays)
            {
                var cumumalitveAverage = 0m;
                foreach (var tradingDay in priceHistory)
                {
                    cumumalitveAverage += (tradingDay.HighPrice + tradingDay.LowPrice) / 2;
                }

                return cumumalitveAverage / numberOfDays;
            }
            else return 0m;            
        }

        public decimal GetHighestPrice(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            priceHistory = getPriceHistorySpan(numberOfDays);

            if (priceHistory.Count() == numberOfDays)
            {
                decimal highestPrice = 0.00m;
                foreach (var tradingDay in priceHistory)
                {
                    if (tradingDay.ClosePrice > highestPrice)
                        highestPrice = tradingDay.ClosePrice;
                }

                return highestPrice;
            }
            else return 0m;
        }

        public decimal GetLowestPrice(string symbol, DateTime processDate, int numberOfDays)
        {
            IEnumerable<TradingDay> priceHistory = _priceHistory.Where(x => x.Date <= processDate);
            priceHistory = getPriceHistorySpan(numberOfDays);

            if (priceHistory.Count() == numberOfDays)
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
            else return 0m;
        }

        private List<TradingDay> getPriceHistorySpan(int numberOfDays)
        {
            List<TradingDay> priceHistory = new List<TradingDay>();

            if (_priceHistory.Count() >= numberOfDays)
            {
                for (int i = _priceHistory.Count() - numberOfDays; i < _priceHistory.Count(); i++)
                {
                    priceHistory.Add(_priceHistory.ElementAt(i));
                }
            }

            return priceHistory;
        }

        /*public List<Stock> GetLowPointStocks(DateTime startDate, DateTime endDate)
        {
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
        }*/

        private DateTime formatDate(DateTime date)
        {
            var timeOfDay = new TimeSpan(5, 0, 0); //5AM EST
            return date.Date + timeOfDay;
        }
    }
}
