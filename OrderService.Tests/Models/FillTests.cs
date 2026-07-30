using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Tests.Models
{
    public class FillTests
    {
        [Fact]
        public void New_WhenSetZeroVolume_Shall_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Fill() { ExecutionId = 1, OppOrderId = "B1", OrderId = "A1", Volume = 0, Notional = 5m });
        }

        [Fact]
        public void New_WhenSetNegativeVolume_Shall_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Fill() { ExecutionId = 1, OppOrderId = "B1", OrderId = "A1", Volume =-10, Notional = 5m });
        }
    }
}
