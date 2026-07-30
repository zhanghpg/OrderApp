using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Services
{
    public enum Algorithm
    {
        PRICE_TIME_PRIORITRYT=1,
        PRO_RATA=2

    }
    public class ExecutionServiceFactory
    {
        public IExecutionService CreateExecutionService(Algorithm algo)
        {
            return algo switch
            {
                Algorithm.PRICE_TIME_PRIORITRYT => new PtpExecutionService(),
                Algorithm.PRO_RATA => new ProRataExecutionService(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
