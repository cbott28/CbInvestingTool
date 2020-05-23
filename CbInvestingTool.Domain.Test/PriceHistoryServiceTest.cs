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
    public class PriceHistoryServiceTest
    {
        IPriceHistoryRepository _priceHistoryRepository;
        PriceHistoryService _priceHistoryService;
        private const string ValidSymbol = "VALIDSYMBOL";
        private const string InvalidSymbol = "INVALIDSYMBOL";
        private DateTime _ValidStartDate;
        private DateTime _ValidEndDate;
        private DateTime _InvalidStartDate;
        private DateTime _InvalidEndDate;

        [TestInitialize]
        public void Initialize()
        {
            _priceHistoryRepository = A.Fake<IPriceHistoryRepository>();
            _priceHistoryService = new PriceHistoryService(_priceHistoryRepository);
            _ValidStartDate = DateTime.Now.AddDays(-1);
            _ValidEndDate = DateTime.Now.AddDays(-1);
            _InvalidStartDate = DateTime.Now.AddDays(1);
            _InvalidEndDate = DateTime.Now.AddDays(1);

            A.CallTo(() => _priceHistoryRepository.GetPriceHistory(ValidSymbol, _ValidStartDate, _ValidEndDate)).Returns(new List<TradingDay>()
            {
                new TradingDay()
                {
                    OpenPrice = 5.00m,
                    HighPrice = 5.00m,
                    LowPrice = 1.00m,
                    ClosePrice = 5.00m,
                    Date = DateTime.Now
                }
            });

            A.CallTo(() => _priceHistoryRepository.GetPriceHistory(InvalidSymbol, _ValidStartDate, _ValidEndDate)).Throws<StockNotFoundException>();

            A.CallTo(() => _priceHistoryRepository.GetPriceHistory(ValidSymbol, _InvalidStartDate, _ValidEndDate)).Throws<InvalidDateException>();

            A.CallTo(() => _priceHistoryRepository.GetPriceHistory(ValidSymbol, _ValidStartDate, _InvalidEndDate)).Throws<InvalidDateException>();
        }

        [TestMethod]
        public void GetPriceHistory_ValidSymbolAndDates_ReturnsPriceHistory()
        {
            var priceHistory = _priceHistoryService.GetPriceHistory(ValidSymbol, _ValidStartDate, _ValidEndDate);

            Assert.IsInstanceOfType(priceHistory, typeof(List<TradingDay>));
        }

        [TestMethod, ExpectedException(typeof(StockNotFoundException))]
        public void GetPriceHistory_InvalidSymbol_ThrowsException()
        {
            var priceHistory = _priceHistoryService.GetPriceHistory(InvalidSymbol, _ValidStartDate, _ValidEndDate);
        }

        [TestMethod, ExpectedException(typeof(InvalidDateException))]
        public void GetPriceHistory_InvalidStartDate_ThrowsException()
        {
            var priceHistory = _priceHistoryService.GetPriceHistory(ValidSymbol, _InvalidStartDate, _ValidEndDate);
        }

        [TestMethod, ExpectedException(typeof(InvalidDateException))]
        public void GetPriceHistory_InvalidEndDate_ThrowsException()
        {
            var priceHistory = _priceHistoryService.GetPriceHistory(ValidSymbol, _ValidStartDate, _InvalidEndDate);
        }
    }
}
