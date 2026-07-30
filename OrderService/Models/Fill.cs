using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Models
{
    public class Fill
    {
        public required int ExecutionId { get; set; }
        public required string OrderId { get; set; }

        public required string OppOrderId { get; set; }
        public required int Volume
        {
            get;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Volume must be greater than zero");

                field = value;
            }
        }
        public required decimal Notional { get; set; }
    }
}
