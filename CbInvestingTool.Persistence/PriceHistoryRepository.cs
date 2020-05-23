using CbInvesting.Domain;
using CbInvesting.Domain.Exceptions;
using CbInvesting.Domain.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvestingTool.Persistence
{
    public class PriceHistoryRepository : IPriceHistoryRepository
    {
        public List<TradingDay> GetPriceHistory(string symbol, DateTime startdate, DateTime endDate)
        {
            if (startdate > DateTime.Now ||
                endDate > DateTime.Now) throw new InvalidDateException();

            string url = string.Format("https://api.tdameritrade.com/v1/marketdata/{0}/pricehistory", symbol.ToUpper());
            var client = new RestClient(url);
            RestRequest getRequest = new RestRequest(Method.GET);
            getRequest.AddHeader("cache-control", "no-cache");
            getRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            getRequest.AddParameter("apikey", ConfigurationManager.AppSettings.Get("apikey"));
            getRequest.AddParameter("periodType", "month");
            getRequest.AddParameter("period", "1");
            getRequest.AddParameter("frequencyType", "daily");
            getRequest.AddParameter("frequency", "1");
            getRequest.AddParameter("startDate", getEpochTime(startdate).ToString().PadRight(13, '0'));
            getRequest.AddParameter("endDate", getEpochTime(endDate).ToString().PadRight(13, '0'));

            var response = client.Get(getRequest);

            JObject priceHistorySearch = JObject.Parse(response.Content);
            IList<JToken> candles = new List<JToken>();

            try
            {
                candles = priceHistorySearch["candles"].Children().ToList();
            }
            catch (Exception)
            {
                return new List<TradingDay>();
            }

            List<TradingDay> priceHistory = new List<TradingDay>();
            foreach (var candle in candles)
            {
                TradingDay tradingDay = new TradingDay()
                {
                    Volume = (int)candle["volume"],
                    OpenPrice = (decimal)candle["open"],
                    HighPrice = (decimal)candle["high"],
                    LowPrice = (decimal)candle["low"],
                    ClosePrice = (decimal)candle["close"],
                    Date = convertFromEpoch((string)candle["datetime"])
            };

                priceHistory.Add(tradingDay);
            }

            return priceHistory;
        }

        private int getEpochTime(DateTime date)
        {
            TimeSpan span = date - new DateTime(1970, 1, 1);
            return (int)span.TotalSeconds;
        }

        private DateTime convertFromEpoch(string epochString)
        {
            int epochSeconds = int.Parse(epochString.Substring(0, 10));
            DateTime epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epochDate.AddSeconds(epochSeconds);
        }
    }
}
