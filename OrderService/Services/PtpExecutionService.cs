using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Services
{
    /// <summary>
    /// Matches orders with price-time-priority algorithm and applies fills directly to the input orders.
    /// Returns the same collection, mutated in place.
    /// </summary>
    public sealed class PtpExecutionService : IExecutionService
    {
        public IEnumerable<Order> Execute(IReadOnlyCollection<Order> orders)
        {
            if (orders.Count == 0) 
                return Enumerable.Empty<Order>();

            if(orders.All(o => o.Direction == Direction.Buy)
                    || orders.All(o => o.Direction == Direction.Sell))
                return orders;

            var buyOrders = orders.Where(o => o.Direction == Direction.Buy && o.VolumeLeft>0)
                .OrderByDescending(o => o.Notional).ThenBy(o => o.OrderDateTime).ToList();

            var sellOrders = orders.Where(o => o.Direction == Direction.Sell && o.VolumeLeft > 0)
               .OrderBy(o => o.Notional).ThenBy(o => o.OrderDateTime).ToList();

            int execId = 0;           
            foreach (var buyOrder in buyOrders)
            {
                foreach (var sellOrder in sellOrders)
                {
                    if (buyOrder.VolumeLeft == 0) break;
                    if (sellOrder.VolumeLeft == 0 ) continue;
                    if (sellOrder.Notional > buyOrder.Notional) break;

                    var matchedQty=Math.Min(buyOrder.VolumeLeft, sellOrder.VolumeLeft);
                    execId++;
                    var fillNotional= buyOrder.OrderDateTime.CompareTo(sellOrder.OrderDateTime)<=0?buyOrder.Notional:sellOrder.Notional;                 
                    var bFill = new Fill() { ExecutionId = execId, OrderId = buyOrder.OrderId,OppOrderId=sellOrder.OrderId, Notional = fillNotional, Volume = matchedQty };
                    buyOrder.AddFill(bFill);

                    var sFill = new Fill() { ExecutionId = execId, OrderId = sellOrder.OrderId, OppOrderId=buyOrder.OrderId, Notional = fillNotional, Volume = matchedQty };
                    sellOrder.AddFill(sFill);
                }
            }
            return orders;
        }
    }
}
