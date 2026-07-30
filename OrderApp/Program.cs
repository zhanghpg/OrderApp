using OrderService.Models;
using OrderService.Services;
using System.Runtime.CompilerServices;
using System.Text.Json;

Console.WriteLine("=============");
Console.WriteLine("It's a simple order matching app that accepts price-time-priority & pro-rata algorithm");
Console.WriteLine("OrderApp algo filename");
Console.WriteLine("algo         1 for price-time-priority, 2 for pro-rata algorithm, default is 1");
Console.WriteLine("filename     filename contains josn string for order books, if not specified, it uses example of user story");
Console.WriteLine("=============");

Algorithm algo = Algorithm.PRICE_TIME_PRIORITRYT;
if (args.Length>=1)
{    
    if (!Enum.TryParse<Algorithm>(args[0], out algo))
    {
        Console.WriteLine("Invailid algorithm");
        return;
    }
}

var orders =new List<Order>();
if (args.Length ==2)
{
    if (!File.Exists(args[1]))
    {
        Console.WriteLine($"File {args[1]} doesn't exist");
        return;
    }
    var data=File.ReadAllText(args[1]);
    orders = JsonSerializer.Deserialize<List<Order>>(data); 
}
else
{
    if (algo == Algorithm.PRICE_TIME_PRIORITRYT)
    {
        orders = new List<Order>() {
                    Order.Create("A", "A1", Direction.Buy, 100, 4.99m, ToDateTime("09:27:43")),
                    Order.Create("B", "B1", Direction.Buy, 200, 5.00m, ToDateTime("10:21:46")),
                    Order.Create("C", "C1", Direction.Buy, 150, 5.00m, ToDateTime("10:26:18")),
                    Order.Create("D", "D1", Direction.Sell, 150, 5.00m, ToDateTime("10:32:41")),
                    Order.Create("E", "E1", Direction.Sell, 100, 5.00m, ToDateTime("10:33:07")) };
    }
    else
    {
        orders = new List<Order>() {
                    Order.Create("A", "A1", Direction.Buy, 50, 5.00m, ToDateTime("09:27:43")),
                    Order.Create("B", "B1", Direction.Buy, 200, 5.00m, ToDateTime("10:21:46")),
                    Order.Create("C", "C1", Direction.Sell, 200, 5.00m, ToDateTime("10:26:18")) };
    }
}


var service=new ExecutionServiceFactory().CreateExecutionService(algo);
var newOrders= service.Execute(orders);
foreach (var order in newOrders)
{
    Console.WriteLine($"OrderId:{order.OrderId}, Matched state: {order.OrderState}");
    if (order.Fills != null)
    {
        foreach (var fill in order.Fills)
            Console.WriteLine($"==>Opp orderId:{fill.OppOrderId}, filled volume: {fill.Volume}, filled Price: {fill.Notional}");
    }
}

static DateTime ToDateTime(string hhmmss) => DateTime.ParseExact(hhmmss, "HH:mm:ss", null);