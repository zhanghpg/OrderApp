# Answers

## 1. How much time did you spend on the engineering task?

The total time spent are around 6 hours.

## 2. What would you add to your solution if you'd had more time?

With more time, I would add:

- **Real unit tests wired into a test framework** (xUnit) rather than a standalone console runner, so they run as part of CI and produce standard test reports.
- **An audit trail for the rounding reconciliation step** pro-rata allocation involves rounding each order's share, which can leave the sum off by a unit or two versus the true matched volume. I'd log which order(s) received the +1/-1 adjustment and why, since that kind of traceability matters for trade support and reconciliation review.
- **A concurrency/thread-safety review**, in case this service is ever called against a shared, live order book from multiple threads rather than a single, isolated batch.

## 3. What do you think is the most useful feature added to the latest version of C#? Include a code snippet that shows how you've used it.

Pattern switch is the useful feature I used in my recent Reconciliation project.

```csharp
    public PatternDto CreateDto(PatternKind kind) => kind switch
    {
        PatternKind.TextReplaceXToY => new TextReplaceXToYPatternDto
        {
            ReplaceFrom = "",
            ReplaceTo = ""
        },
        PatternKind.DateInFormatOfXPatternDto => new DateInFormatOfXPatternDto { Format = "" },
        PatternKind.None => new NonePatternDto(),
        PatternKind.Const => new ConstPatternDto() { Text = "" },
        PatternKind.TextOfLeftX => new TextOfLeftXPatternDto { Length = 0 },
        PatternKind.TextOfRightX => new TextOfRightXPatternDto { Length = 0 },
        PatternKind.TextFromXToY => new TextFromXToYPatternDto
        {
            From = 0,
            To = 0
        },
        PatternKind.GroupInRegexPatternOfX => new GroupInRegexPatternOfXPatternDto { RegexPattern = "" },
        PatternKind.BuiltinFunction => new BuiltinFunctionPatternDto { Function = "" },
        _ => throw new NotSupportedException($"Unknown pattern kind: {kind}")
    };
```

I chose this over a long if/else chain or an old-style `switch` with fallthrough risk because it's an expression, not a statement every branch has to return a value, and the compiler can flag missing cases if `PatternKind` gets a new member later. That exhaustiveness check is what actually prevents bugs when the enum grows.

## 4. How would you track down a performance issue in production? Have you ever had to do this?

My approach to tracking down a production performance issue:

1. **Reproduce and confirm with data first** before touching any code, I look at metrics or APM data (response time, CPU, GC pressure, thread pool starvation) to confirm there's an actual regression and not just noise.
2. **Isolate the scope** narrow the problem to a specific endpoint, query, or service using distributed tracing or targeted logging, rather than guessing based on where the code "feels" slow.
3. **Profile, don't guess** attach a profiler or use `dotnet-trace` / `dotnet-counters` to get an actual call-stack or allocation picture of what's consuming time, instead of assuming based on code review alone.
