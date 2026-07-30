# OrderApp

A simple order matching engine implementing price-time-priority and pro-rata allocation algorithms.

## Overview

OrderApp is a .NET 10 console application that simulates order book matching. It supports two allocation strategies:

- **Price-Time-Priority (FIFO)** - orders at the best price are matched in the order they were received.
- **Pro-Rata** - orders at the best price are allocated proportionally based on order size.

## Prerequisites

- .NET 10 SDK

## Project Structure

- `OrderApp/` - console entry point; drives the matching engine and displays results
- `OrderService/` - core matching logic (order book, matching algorithms, order/trade models)

## Getting Started

### Clone
```bash
git clone https://github.com/zhanghpg/OrderApp
cd OrderApp
```

### Build
```bash
dotnet build
```

### Usage

```bash
OrderApp algo filename
```

| Argument   | Description                                                                                  |
|------------|------------------------------------------------------------------------------------------------|
| `algo`     | `1` for price-time-priority, `2` for pro-rata algorithm. Default is `1`.                       |
| `filename` | Filename containing a JSON string for order books. If not specified, uses the example from the user story. |

### Examples

```bash
# Run with price-time-priority (default), using the built-in example order book
dotnet run --project OrderApp

# Run with pro-rata algorithm, using the built-in example order book
dotnet run --project OrderApp -- 2

# Run with price-time-priority, using a custom order book file
dotnet run --project OrderApp -- 1 orders1.json

# Run with pro-rata algorithm, using a custom order book file
dotnet run --project OrderApp -- 2 orders2.json

## Testing

```bash
dotnet test
```

## License

MIT