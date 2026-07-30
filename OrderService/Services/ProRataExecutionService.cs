using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Services
{
    /// <summary>
    /// Matches orders with pro-rata algorithm and applies fills directly to the input orders.
    /// Returns the same collection, mutated in place.
    /// </summary>
    public sealed class ProRataExecutionService : IExecutionService
    {
        public IEnumerable<Order> Execute(IReadOnlyCollection<Order> orders)
        {
            if (orders.Count == 0) 
                return Enumerable.Empty<Order>();

            if(orders.All(o => o.Direction == Direction.Buy)
                    || orders.All(o => o.Direction == Direction.Sell))
                return orders;

            if (orders.Select(o => o.Notional).Distinct().Count() > 1)
                throw new InvalidOperationException("The notional must be same");

            var buyVolume= orders.Where(o => o.Direction == Direction.Buy).Sum(o=> o.VolumeLeft);
            var sellVolume = orders.Where(o => o.Direction == Direction.Sell).Sum(o => o.VolumeLeft);

            if (buyVolume == 0 || sellVolume == 0)
                return orders;

            var matchedVolume=Math.Min(buyVolume, sellVolume);
            
            var buyOrders = orders.Where(o => o.Direction == Direction.Buy)
                .OrderBy(o => o.OrderDateTime);

            var sellOrders = orders.Where(o => o.Direction == Direction.Sell)
               .OrderBy(o => o.OrderDateTime);

            int execId = 0, matchedVolumeleft = matchedVolume;
            int buyAmtTobeClosedLeft = 0, sellAmtToBeClosed = 0;
            foreach (var buyOrder in buyOrders)
            {
                buyAmtTobeClosedLeft = (int)Math.Round((double)matchedVolume / (double)buyVolume * (double)buyOrder.Volume, 0);
                
                foreach (var sellOrder in sellOrders)
                {
                    if (buyAmtTobeClosedLeft == 0) break;

                    sellAmtToBeClosed = (int)Math.Round((double)matchedVolume / (double)sellVolume * (double)sellOrder.Volume, 0)
                        -(sellOrder.Volume-sellOrder.VolumeLeft);                   
                    
                    int matchedQty = 0;      
                    matchedQty = Math.Min(buyAmtTobeClosedLeft, sellAmtToBeClosed);

                    if (matchedQty <= 0) continue;

                    buyAmtTobeClosedLeft -= matchedQty;
                    execId++;

                    var bFill = new Fill() { ExecutionId = execId, OrderId = buyOrder.OrderId, OppOrderId = sellOrder.OrderId, Notional = buyOrder.Notional, Volume = matchedQty };
                    buyOrder.AddFill(bFill);

                    var sFill = new Fill() { ExecutionId = execId, OrderId = sellOrder.OrderId, OppOrderId = buyOrder.OrderId, Notional = buyOrder.Notional, Volume = matchedQty };
                    sellOrder.AddFill(sFill);
                }
            }
            return orders;
        }
    }
}
