using OrderService.Models;
using OrderService.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Tests.Services
{
    public class PtpExecutionServiceTests
    {
        private readonly PtpExecutionService _sut = new();

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
                Order.Create("B", "B1", Direction.Buy, 200, 5.10m, ToDateTime("09:01:00")),
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
                Order.Create("B", "B1", Direction.Sell, 200, 5.10m, ToDateTime("09:01:00")),
            };

            var result = _sut.Execute(orders).ToList();

            Assert.Equal(orders, result);
            Assert.All(result, o => Assert.Null(o.Fills));
            Assert.All(result, o => Assert.Equal(MatchedState.NoMatched, o.OrderState));
        }

        [Fact]
        public void Execute_UserStoryOrderBook_ReturnsAllExpectedResult()
        {
            var orders = new List<Order>() {
                    Order.Create("A", "A1", Direction.Buy, 100, 4.99m, ToDateTime("09:27:43")),
                    Order.Create("B", "B1", Direction.Buy, 200, 5.00m, ToDateTime("10:21:46")),
                    Order.Create("C", "C1", Direction.Buy, 150, 5.00m, ToDateTime("10:26:18")),
                    Order.Create("D", "D1", Direction.Sell, 150, 5.00m, ToDateTime("10:32:41")),
                    Order.Create("E", "E1", Direction.Sell, 100, 5.00m, ToDateTime("10:33:07")) };

            var result=_sut.Execute(orders).ToList();            

            Assert.Equal(orders, result);

            var ord = result[0];
            Assert.Equal("A1", ord.OrderId);
            Assert.Equal(MatchedState.NoMatched, ord.OrderState);
            Assert.Null(ord.Fills);

            ord = result[1];
            Assert.Equal("B1", ord.OrderId);
            Assert.Equal(MatchedState.FullMatched, ord.OrderState);
            Assert.Equal(0, ord.VolumeLeft);
            Assert.Equal(2, ord.Fills!.Count);
            AssertFill(ord.Fills[0], "D1", 150, 5m);
            AssertFill(ord.Fills[1], "E1", 50, 5m);

            ord = result[2];
            Assert.Equal("C1", ord.OrderId);
            Assert.Equal(MatchedState.PartialMatched, ord.OrderState);
            Assert.Equal(100, ord.VolumeLeft);
            var fill=Assert.Single(ord.Fills!);
            AssertFill(fill, "E1", 50, 5m);

            ord = result[3];
            Assert.Equal("D1", ord.OrderId);
            Assert.Equal(MatchedState.FullMatched, ord.OrderState);
            Assert.Equal(0, ord.VolumeLeft);
            fill = Assert.Single(ord.Fills!);
            AssertFill(fill, "B1", 150, 5m);

            ord = result[4];
            Assert.Equal("E1", ord.OrderId);
            Assert.Equal(MatchedState.FullMatched, ord.OrderState);
            Assert.Equal(0, ord.VolumeLeft);
            Assert.Equal(2, ord.Fills!.Count);
            AssertFill(ord.Fills[0], "B1", 50, 5m);
            AssertFill(ord.Fills[1], "C1", 50, 5m);
        }

        [Fact]
        public void Execute_WhenRestBuyOrder_Exists_Then_Filled_With_Buy_Notional()
        {
            var orders = new List<Order>
            {
                Order.Create("A", "A1", Direction.Buy, 100, 5.20m, ToDateTime("09:00:00")),
                Order.Create("B", "B1", Direction.Sell, 200, 5.10m, ToDateTime("09:01:00")),
            };

            var result = _sut.Execute(orders).ToList();

            Assert.Equal(orders, result);
            var ord = result[0];
            var fill=Assert.Single(ord.Fills!);
            Assert.Equal(5.20m, fill.Notional);
        }

        [Fact]
        public void Execute_WhenRestSellOrder_Exists_Then_Filled_With_Buy_Notional()
        {
            var orders = new List<Order>
            {
                Order.Create("A", "A1", Direction.Sell, 100, 5.10m, ToDateTime("09:00:00")),
                Order.Create("B", "B1", Direction.Buy, 200, 5.20m, ToDateTime("09:01:00")),
            };

            var result = _sut.Execute(orders).ToList();

            Assert.Equal(orders, result);
            var ord = result[0];
            var fill = Assert.Single(ord.Fills!);
            Assert.Equal(5.10m, fill.Notional);
        }


        private static void AssertFill(Fill fill, string expOppOrderId, int expVolume, decimal expNotional)
        {
            Assert.Equal( expOppOrderId, fill.OppOrderId);
            Assert.Equal(expVolume, fill.Volume);
            Assert.Equal(expNotional, fill.Notional);
        }
    }
}
