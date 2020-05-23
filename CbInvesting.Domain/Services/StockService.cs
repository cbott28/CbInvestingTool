using CbInvesting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbInvesting.Domain.Services
{
    public class StockService
    {
        private IStockRepository _stockRepository;

        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public Stock GetBySymbol(string symbol)
        {
            Stock stock = _stockRepository.GetBySymbol(symbol);
            return stock;
        }

        public List<Stock> GetStocks()
        {
            List<string> symbols = getSymbols();
            List<Stock> stocks = new List<Stock>();

            while (symbols.Count > 0)
            {
                stocks.AddRange(_stockRepository.GetStocks(symbols.Take(500)));
                symbols.RemoveRange(0, (symbols.Count > 500) ? 500 : symbols.Count);
            }
            
            return stocks;
        }

        private List<string> getSymbols()
        {
            List<string> symbols = new List<string>();

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(@"C:\Users\cbott\AppData\CbInvesting\nasdaqlisted.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] columns = line.Split('|');
                            symbols.Add(columns[0]);
                        }
                    }

                    // Read the stream to a string, and write the string to the console.
                    
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return symbols;
        }
    }
}
