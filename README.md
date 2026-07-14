# Semantic.NET

[![CI](https://github.com/Franchef/Semantic.NET/actions/workflows/ci.yml/badge.svg)](https://github.com/Franchef/Semantic.NET/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/Franchef/Semantic.NET/branch/main/graph/badge.svg)](https://codecov.io/gh/Franchef/Semantic.NET)

Semantic.NET provides semantic utilities for common programming scenarios, enabling developers to write expressive and intention-revealing code.

## Project Scope

The goal of this project is to create reusable semantic utilities that simplify and clarify common patterns in .NET development. These utilities are designed to:

- Make code more readable and maintainable
- Express intent clearly
- Reduce boilerplate for common scenarios

## Example: Pattern Matching Utility

One of the utilities in this project is a pattern matching builder for sequences. It allows you to define and check for specific patterns in a sequence of values in a fluent, semantic way.

### Usage Example

```csharp
var pattern = PatternMatchesBuilder.StartsWith(1)
	.ContinuesWith(2)
	.EndsWith(3);

pattern.Next(1);
pattern.Next(2);
pattern.Next(3);
if (pattern.HasMatch())
{
	// Pattern 1,2,3 matched!
}
```

See the unit tests in `SemanticTests/Sequences/PatternMatchesTests.cs` for more usage details.

## Example: Sliding Window Utility

```csharp
var window = SlidingWindowBuilder.Create<int>(3);

window.Add(10);
window.Add(20);
window.Add(30);
// [10, 20, 30]

window.Add(40);
// [20, 30, 40]

var items = window.GetItems();
```

## Example: Semantic State Machine Builders

State machines can also be configured with a fluent API that guides transition definitions:

```csharp
var machine = Moore<MyState>.Builder(MyState.Idle)
    .WithState(MyState.Idle, _ => StateMachine<MyState>.NoTransition())
    .WithState(MyState.Running, _ => StateMachine<MyState>.NoTransition())
    .WithState(MyState.Stopped, _ => StateMachine<MyState>.NoTransition())
    .From(MyState.Idle).On("start").GoTo(MyState.Running)
    .From(MyState.Running).On("stop").GoTo(MyState.Stopped)
    .Build();

machine.ProcessInput("start");
// machine.CurrentState == MyState.Running
```

For Mealy machines, each transition concludes with an explicit output:

```csharp
var mealy = Mealy<MyState>.Builder(MyState.Idle)
    .From(MyState.Idle).On("start").GoTo(MyState.Running).Emits("started")
    .From(MyState.Running).On("stop").GoTo(MyState.Idle).Emits("stopped")
    .Build();

var output = mealy.ProcessInput("start");
// output == "started"
```

Typed variants are available when you want compile-time safety on inputs and outputs:

```csharp
var typedMoore = Moore<MyState, string>.Builder(MyState.Idle)
    .WithState(MyState.Idle, _ => StateMachine<MyState>.NoTransition())
    .WithState(MyState.Running, _ => StateMachine<MyState>.NoTransition())
    .WithState(MyState.Stopped, _ => StateMachine<MyState>.NoTransition())
    .From(MyState.Idle).On("start").GoTo(MyState.Running)
    .Build();

var typedMealy = Mealy<MyState, string, int>.Builder(MyState.Idle)
    .From(MyState.Idle).On("start").GoTo(MyState.Running).Emits(1)
    .From(MyState.Running).On("stop").GoTo(MyState.Idle).Emits(0)
    .Build();

int signal = typedMealy.ProcessInput("start");
// signal == 1
```

See `SemanticTests/StateMachines/MooreTests.cs` and `SemanticTests/StateMachines/MaleyTests.cs` for complete scenarios and edge cases.

---
This project is in active development. More semantic utilities will be added to cover additional common scenarios.
