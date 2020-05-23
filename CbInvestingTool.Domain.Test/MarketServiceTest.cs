using System;
using CbInvesting.Domain;
using CbInvesting.Domain.Exceptions;
using CbInvesting.Domain.Interfaces;
using CbInvesting.Domain.Services;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CbInvestingTool.Domain.Test
{
    [TestClass]
    public class MarketServiceTest
    {
        private IMarketRepository _marketRepository;
        private MarketService _marketService;
        private DateTime _ValidDate;
        private DateTime _InvalidDate;

        [TestInitialize]
        public void Initialize()
        {
            _marketRepository = A.Fake<IMarketRepository>();
            _marketService = new MarketService(_marketRepository);
            _ValidDate = DateTime.Now;
            _InvalidDate = DateTime.Now.AddDays(-1);

            A.CallTo(() => _marketRepository.GetMarketHours(_ValidDate)).Returns(new MarketHours()
            {
                Description = "EQUITY",
                IsOpen = false
            });

            A.CallTo(() => _marketRepository.GetMarketHours(_InvalidDate)).Throws<InvalidDateException>();
        }

        [TestMethod]
        public void GetMarketHours_ValidDate_RetrurnsMarketHours()
        {
            var marketHours = _marketService.GetMarketHours(_ValidDate);

            Assert.IsInstanceOfType(marketHours, typeof(MarketHours));
        }

        [TestMethod, ExpectedException(typeof(InvalidDateException))]
        public void GetMarketHours_InvalidDate_ThrowsException()
        {
            var marketHours = _marketService.GetMarketHours(_InvalidDate);
        }
    }
}
