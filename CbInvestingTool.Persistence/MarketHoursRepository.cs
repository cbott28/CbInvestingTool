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
    public class MarketHoursRepository : IMarketRepository
    {
        public MarketHours GetMarketHours(DateTime date)
        {
            if (date < DateTime.Now) throw new InvalidDateException();

            string url = string.Format("https://api.tdameritrade.com/v1/marketdata/EQUITY/hours");
            var client = new RestClient(url);
            RestRequest getRequest = new RestRequest(Method.GET);
            getRequest.AddHeader("cache-control", "no-cache");
            getRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            getRequest.AddParameter("apikey", ConfigurationManager.AppSettings.Get("apikey"));
            getRequest.AddParameter("date", date.ToString("yyyy-MM-dd"));
            var response = client.Get(getRequest);

            JObject marketSearch = JObject.Parse(response.Content);
            MarketHours marketHours = new MarketHours()
            {
                Description = (string)marketSearch["equity"]["EQ"]["marketType"],
                IsOpen = (bool)marketSearch["equity"]["EQ"]["isOpen"],
                PreMarketStart = DateTime.Parse((string)marketSearch["equity"]["EQ"]["sessionHours"]["preMarket"][0]["start"]),
                PreMarketEnd = DateTime.Parse((string)marketSearch["equity"]["EQ"]["sessionHours"]["preMarket"][0]["end"]),
                RegularMarketStart = DateTime.Parse((string)marketSearch["equity"]["EQ"]["sessionHours"]["regularMarket"][0]["start"]),
                RegularMarketEnd = DateTime.Parse((string)marketSearch["equity"]["EQ"]["sessionHours"]["regularMarket"][0]["end"]),
                PostMarketStart = DateTime.Parse((string)marketSearch["equity"]["EQ"]["sessionHours"]["postMarket"][0]["start"]),
                PostMarketEnd = DateTime.Parse((string)marketSearch["equity"]["EQ"]["sessionHours"]["preMarket"][0]["end"])
            };

            return marketHours;
        }
    }
}
