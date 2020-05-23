using System.Configuration;
using System.Collections.Specialized;
using CbInvesting.Domain;
using CbInvesting.Domain.Exceptions;
using CbInvesting.Domain.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace CbInvestingTool.Persistence
{
    public class StockRepository : IStockRepository
    {
        public Stock GetBySymbol(string symbol)
        {
            string url = string.Format("https://api.tdameritrade.com/v1/marketdata/{0}/quotes", symbol);
            var client = new RestClient(url);
            RestRequest getRequest = new RestRequest(Method.GET);
            getRequest.AddHeader("cache-control", "no-cache");
            getRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            getRequest.AddParameter("apikey", ConfigurationManager.AppSettings.Get("apikey"));
            var response = client.Get(getRequest);

            JObject stockSearch = JObject.Parse(response.Content);
            var result = stockSearch[symbol];
            Stock stock = result.ToObject<Stock>();

            if (String.IsNullOrEmpty(stock.Symbol))
                throw new StockNotFoundException();
            else
                return stock;
        }

        public List<Stock> GetStocks(IEnumerable<string> symbols)
        {
            string url = string.Format("https://api.tdameritrade.com/v1/marketdata/quotes");
            var client = new RestClient(url);
            RestRequest getRequest = new RestRequest(Method.GET);
            getRequest.AddHeader("cache-control", "no-cache");
            getRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            getRequest.AddParameter("apikey", ConfigurationManager.AppSettings.Get("apikey"));
            getRequest.AddParameter("symbol", String.Join(",",symbols));
            var response = client.Get(getRequest);

            JObject stockSearch = JObject.Parse(response.Content);
            IList<JToken> results = stockSearch.Children().ToList();
            List<Stock> stocks = new List<Stock>();

            foreach (var result in results)
            {
                var data = result.Children().ToList();
                Stock stock = data.First().ToObject<Stock>();
                stocks.Add(stock);
            }

            return stocks;
        }
    }
}
