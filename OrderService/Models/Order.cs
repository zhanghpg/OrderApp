using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Models
{
    public class Order
    {
        public required string CompanyId { get; set; }
        public required string OrderId { get; set; }
        public required Direction Direction { get; set; }
        public required int Volume
        {
            get;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Volume must be greater than zero");

                field = value;
                VolumeLeft = value;
            }
        }
        public int VolumeLeft
        {
            get;
            set
            {
                if (value < 0)
                    throw new ArgumentException("VolumeLeft cannot be negative");

                field = value;
            }
        }
        public required decimal Notional { get; set; }
        public required DateTime OrderDateTime { get; set; }
        public MatchedState OrderState { get; set; } = MatchedState.NoMatched;
        public List<Fill>? Fills { get; set; }

        public void AddFill(Fill fill)
        {
            if (fill.Volume > VolumeLeft)
                throw new InvalidOperationException("The fill volume cannot be greater than the volume left");

            if( Fills == null) 
                Fills = new List<Fill>();

            Fills.Add(fill);
            VolumeLeft -= fill.Volume;
            OrderState = VolumeLeft==0? MatchedState.FullMatched: MatchedState.PartialMatched;
        }


        public static Order Create(string companyId,string orderId,Direction direction,int volume,decimal notional,
                DateTime orderDateTime,MatchedState state=MatchedState.NoMatched)
        {
            return new Order() { CompanyId = companyId, OrderId = orderId, Direction = direction, Volume = volume, 
                Notional = notional, OrderDateTime = orderDateTime, OrderState = state };
        }
    }
}
