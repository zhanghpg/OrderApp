using OrderService.Models;
using OrderService.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Tests.Services
{
    public class ProRataExecutionServiceTests
    {
        private readonly ProRataExecutionService _sut = new();

        private static DateTime ToDateTime(string hhmmss) => DateTime.ParseExact(hhmmss, "HH:mm:ss", null);
     
        [Fact]
        public void Execute_EmptyOrders_ReturnsEmpty()
        {
            var result = _sut.Execute(new List<Order>());
            Assert.Empty(result);
        }

        [Fact]
        public void Execute_AllBuyOrders_ReturnsUnchangedNoFills()
        {
            var orders = new List<Order>
            {
                Order.Create("A", "A1", Direction.Buy, 100, 5.00m, ToDateTime("09:00:00")),
                Order.Create("B", "B1", Direction.Buy, 200, 5.00m, ToDateTime("09:01:00")),
            };

            var result = _sut.Execute(orders).ToList();

            Assert.Equal(orders, result);
            Assert.All(result, o => Assert.Null(o.Fills));
            Assert.All(result, o => Assert.Equal(MatchedState.NoMatched, o.OrderState));
        }

        [Fact]
        public void Execute_AllSellOrders_ReturnsUnchangedNoFills()
        {
            var orders = new List<Order>
            {
                Order.Create("A", "A1", Direction.Sell, 100, 5.00m, ToDateTime("09:00:00")),
                Order.Create("B", "B1", Direction.Sell, 200, 5.00m, ToDateTime("09:01:00")),
            };

            var result = _sut.Execute(orders).ToList();

            Assert.Equal(orders, result);
            Assert.All(result, o => Assert.Null(o.Fills));
            Assert.All(result, o => Assert.Equal(MatchedState.NoMatched, o.OrderState));
        }


        [Fact]
        public void Execute_Orders_With_Different_Notoinal_Throws_InvalidOperationException()
        {
            var orders = new List<Order>
            {
                Order.Create("A", "A1", Direction.Sell, 100, 5.00m, ToDateTime("09:00:00")),
                Order.Create("B", "B1", Direction.Buy, 200, 5.10m, ToDateTime("09:01:00")),
            };

            var ex=Assert.Throws<InvalidOperationException>(()=> _sut.Execute(orders).ToList());

            Assert.Equal("The notional must be same", ex.Message);
        }

        [Fact]
        public void Execute_UserStoryOrderBook_ReturnsAllExpectedResult()
        {
            var orders = new List<Order>() {
                    Order.Create("A", "A1", Direction.Buy, 50, 5.00m, ToDateTime("09:27:43")),
                    Order.Create("B", "B1", Direction.Buy, 200, 5.00m, ToDateTime("10:21:46")),
                    Order.Create("C", "C1", Direction.Sell, 200, 5.00m, ToDateTime("10:26:18")) };

            var result=_sut.Execute(orders).ToList();            

            Assert.Equal(orders, result);

            var ord = result[0];
            Assert.Equal("A1", ord.OrderId);
            Assert.Equal(MatchedState.PartialMatched, ord.OrderState);
            var fill = Assert.Single(ord.Fills!);
            Assert.Equal(10, ord.VolumeLeft);
            AssertFill(fill, "C1", 40, 5m);

            ord = result[1];
            Assert.Equal("B1", ord.OrderId);
            Assert.Equal(MatchedState.PartialMatched, ord.OrderState);
            fill = Assert.Single(ord.Fills!);
            Assert.Equal(40, ord.VolumeLeft);
            AssertFill(fill, "C1", 160, 5m);

            ord = result[2];
            Assert.Equal("C1", ord.OrderId);
            Assert.Equal(MatchedState.FullMatched, ord.OrderState);
            Assert.Equal(0, ord.VolumeLeft);
            Assert.Equal(2, ord.Fills!.Count);
            AssertFill(ord.Fills[0], "A1", 40, 5m);
            AssertFill(ord.Fills[1], "B1", 160, 5m);

        }

        private static void AssertFill(Fill fill, string expectedOppOrderId, int expectedVolume, decimal expectedNotional)
        {
            Assert.Equal(expectedOppOrderId, fill.OppOrderId);
            Assert.Equal(expectedVolume, fill.Volume);
            Assert.Equal(expectedNotional, fill.Notional);
        }
    }
}
