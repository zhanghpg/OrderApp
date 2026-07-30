using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Tests.Models
{
    public class OrderTests
    {
        [Fact]
        public void Create_WhenPassZeroVolume_ShallThrowsArguementExption()
        {
            var ex = Assert.Throws<ArgumentException>(() => Order.Create("A", "A1", Direction.Buy, 0,5.0m, ToDateTime("09:55:00")));

            Assert.Equal("Volume must be greater than zero", ex.Message);
        }

        [Fact]
        public void Create_WhenPassNegetiveVolume_ShallThrowsArguementExption()
        {
            var ex = Assert.Throws<ArgumentException>(() => Order.Create("A", "A1", Direction.Buy, -1, 5.0m, ToDateTime("09:55:00")));

            Assert.Equal("Volume must be greater than zero", ex.Message);
        }

        [Fact]
        public void New_WhenSetNegativeVolumeLeft_Shall_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Order() { CompanyId = "A", OrderId = "A1", Volume = 10, Notional = 5m, Direction=Direction.Buy, OrderDateTime=ToDateTime("09:55:00"), VolumeLeft=-10 });
        }

        [Fact]
        public void Create_WhenPassValidateVolume_ShallSuccess()
        {
            var ord= Order.Create("A", "A1", Direction.Buy, 100, 5.0m, ToDateTime("09:55:00"));

            Assert.Equal("A",ord.CompanyId);
            Assert.Equal("A1", ord.OrderId);
            Assert.Equal(Direction.Buy, ord.Direction);
            Assert.Equal(100, ord.Volume);
            Assert.Equal(100, ord.VolumeLeft);
            Assert.Equal(5m, ord.Notional);
            Assert.Equal("09:55:00", ord.OrderDateTime.ToString("HH:mm:ss"));
            Assert.Equal(MatchedState.NoMatched, ord.OrderState);
        }

        private static DateTime ToDateTime(string hhmmss) => DateTime.ParseExact(hhmmss, "HH:mm:ss", null);
    }
}
