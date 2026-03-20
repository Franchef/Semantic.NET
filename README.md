# Semantic.NET

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

---
This project is in active development. More semantic utilities will be added to cover additional common scenarios.
