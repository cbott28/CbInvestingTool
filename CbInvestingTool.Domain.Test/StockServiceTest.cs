using System;
using System.Collections.Generic;
using CbInvesting.Domain;
using CbInvesting.Domain.Exceptions;
using CbInvesting.Domain.Interfaces;
using CbInvesting.Domain.Services;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CbInvestingTool.Domain.Test
{
    [TestClass]
    public class StockServiceTest
    {
        private IStockRepository _stockRepository;
        private StockService _stockService;
        private const string ValidSymbol = "AAPL";
        private const string InvalidSymbol = "INVALIDSYMBOL";
        private List<string> _ValidSymbols;

        [TestInitialize]
        public void Initialize()
        {
            _stockRepository = A.Fake<IStockRepository>();
            _stockService = new StockService(_stockRepository);

            _ValidSymbols = new List<string>()
            {
                "AAPL", "GOOG"
            };

            A.CallTo(() => _stockRepository.GetBySymbol(ValidSymbol)).Returns(new Stock()
            {
                Symbol = ValidSymbol
            });

            A.CallTo(() => _stockRepository.GetBySymbol(InvalidSymbol)).Throws<StockNotFoundException>();

            A.CallTo(() => _stockRepository.GetStocks(_ValidSymbols)).Returns(new List<Stock>()
            {
                new Stock()
                {
                    Symbol = ValidSymbol
                }
            });
        }

        [TestMethod]
        public void GetBySymbol_ValidSymbol_ReturnsStock()
        {
            var stock = _stockService.GetBySymbol(ValidSymbol);

            Assert.IsInstanceOfType(stock, typeof(Stock));
            Assert.IsNotNull(stock.Symbol);
        }

        [TestMethod, ExpectedException(typeof(StockNotFoundException))]
        public void GetBySymbol_InvalidSymbol_ThrowsException()
        {
            var stock = _stockRepository.GetBySymbol(InvalidSymbol);
        }

        [TestMethod]
        public void GetStocks_StockList_ReturnsStocks()
        {
            var stocks = _stockService.GetStocks();

            Assert.IsInstanceOfType(stocks, typeof(List<Stock>));
        }
    }
}
