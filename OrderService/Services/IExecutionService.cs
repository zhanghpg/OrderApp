using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Services
{
    /// <summary>
    /// Matches orders with certain algorithm and applies fills directly to the input orders.
    /// Returns the same collection, mutated in place.
    /// </summary>
    public interface IExecutionService
    {
        public IEnumerable<Order> Execute(IReadOnlyCollection<Order> orders);
    }
}
