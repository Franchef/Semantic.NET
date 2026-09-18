# .NET 10 High-Performance Engineering Guide
### Purpose: Technical Architecture Guardrails for Software Engineers & AI Code Agents
### Target Runtime: .NET 10 / C# 14
### Focus: Zero-Allocation Memory Layouts, CPU Efficiency, and Non-Blocking Async/Concurrency

---

## 1. Low-Level Memory Layout & Value Type Optimization

### 1.1 `readonly struct` for Immutable Value Types
* **Rule:** Any struct representing immutable data must be declared as `readonly struct`. This prevents the compiler from injecting hidden, performance-degrading defensive copies during field access and method calls.
* **JIT Context:** .NET 10 applies advanced physical promotion to small `readonly struct` variables, passing them directly via CPU registers rather than pushing them onto the stack.

```csharp
// CORRECT: Immutability enforced; optimized via CPU registers
public readonly struct Vertex3D
{
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public Vertex3D(double x, double y, double z) => (X, Y, Z) = (x, y, z);
}

// INCORRECT: Incurs implicit defensive copying overhead when passed or stored
public struct BadVertex3D
{
    public double X;
    public double Y;
    public double Z;
}
```

### 1.2 `ref struct` & `ReadOnlySpan<T>` for Zero-Allocation Parsers
* **Rule:** Use `ref struct` for short-lived operational objects (like tokenizers, serialization state context, or request buffers). 
* **Constraint Validation:** `ref struct` types are guaranteed by the compiler to live solely on the **stack**. They cannot be boxed, cannot be stored in standard arrays, cannot cross `await` boundaries, and cannot be captured in closures.

```csharp
// CORRECT: Zero heap allocation parsing context
public ref struct TokenReader
{
    private ReadOnlySpan<char> _buffer;
    private int _position;

    public TokenReader(ReadOnlySpan<char> source)
    {
        _buffer = source;
        _position = 0;
    }

    public ReadOnlySpan<char> ReadNextToken()
    {
        // Slice logic avoiding string.Substring allocations
        return _buffer.Slice(_position); 
    }
}
```

### 1.3 `in`, `out`, and `ref` Parameter Semantics
* **`in` Parameter Rule:** Use for large, immutable structs to pass them by reference instead of copying their entire payload. **Never** use `in` on small primitives like `int` or `bool`, as the reference pointer overhead degrades CPU cycles.
* **`ref` Parameter Rule:** Use when a struct must be updated in place by the invoked method.
* **`out` Parameter Rule:** Use for lightweight multi-value returns to bypass tuple heap/stack instantiation overhead.

```csharp
// CORRECT: High-efficiency data pipeline
public static class PhysicsEngine
{
    // Passed by reference; zero struct data cloning
    public static void ComputeVelocity(in Vertex3D initial, ref Vertex3D acceleration, out double speed)
    {
        // Acceleration modified in place
        // Speed assigned without allocating a tuple return
        speed = 42.0; 
    }
}
```

### 1.4 Native JIT Escape Analysis
* **Rule:** Write short, self-contained methods for array initialization or lightweight local object processing.
* **Under the Hood:** .NET 10 features sophisticated escape analysis. If the runtime detects that a local array or object does not "escape" the scope of its enclosing method, the JIT automatically optimizes the reference out and stack-allocates it, bypassing the managed heap entirely.

---

## 2. Advanced Collection Handling & Core CPU Efficiency

### 2.1 Collection De-Abstraction over Generic Interfaces
* **Rule:** In hot paths (loops executing >1,000 times), always instantiate and loop through concrete collections (e.g., `List<T>`, `T[]`) directly, rather than storing them as `IEnumerable<T>`.
* **Mechanics:** While .NET 10 aggressively attempts de-abstraction automatically, passing concrete types ensures loop cloning, automatic SIMD vectorization, and devirtualization are applied reliably by the JIT.

```csharp
// CORRECT: Concrete iterations enable devirtualization & JIT loop cloning
public void ProcessData(List<int> items)
```

### 2.2 Chained LINQ Optimization Fusion
* **Rule:** Keep LINQ expressions short and rely on native .NET 10 pattern fusion. Combos like `.OrderBy().First()` or `.Where().Select()` are compiled into fused structural loops.
* **Caution:** For latency-critical paths, completely replace LINQ expressions with predictable, zero-allocation `for` loops or explicit `Span<T>` manipulation.

### 2.3 Compile-Time Regex Generation
* **Rule:** Never instantiate `new Regex(...)` inside loop iterations or API endpoints. Use the static `[GeneratedRegex]` source generator attribute.
* **Mechanics:** .NET 10 transforms greedy regex loops into highly efficient, atomic machine-code matching networks at compile-time.

```csharp
public static partial class InputValidator
{
    [GeneratedRegex(@"^[a-zA-Z0-9]+$", RegexOptions.Compiled)]
    public static partial Regex AlphaNumericMatcher();
}
```

---

## 3. High-Performance Concurrency, Async/Await, & Thread Safety

### 3.1 `ValueTask<T>` for Hybrid Async Code Paths
* **Rule:** If an asynchronous method returns a value and completes synchronously in more than 25% of cases (e.g., read hits from a cache), declare it using `ValueTask<T>`.
* **Allocation Save:** Bypasses creating a heap-allocated `System.Threading.Tasks.Task` object when the result is immediately available.

```csharp
public sealed class DataProvider
{
    private int _cachedValue = 100;
    private bool _isDataFresh = true;

    public ValueTask<int> FetchMetricAsync()
    {
        // Zero allocations on cache hit
        if (_isDataFresh) 
            return new ValueTask<int>(_cachedValue);

        // Allocates task only when real external network I/O is triggered
        return new ValueTask<int>(FetchFromRemoteServerAsync());
    }

    private Task<int> FetchFromRemoteServerAsync() => Task.FromResult(200);
}
```

### 3.2 Non-Blocking Thread Safety & Lock Primitives
* **Rule 1 (Modern Locks):** For standard exclusive blocks, use the dedicated native `System.Threading.Lock` type introduced in .NET 9/10 instead of standard `object` instances. The compiler optimizes this into lightweight, fast-path spinning.
* **Rule 2 (Atomic Operations):** For incrementing, swapping, or checking individual values across multiple threads, use `Interlocked` methods. Do not invoke heavier structural locks.
* **Rule 3 (Async-Safe Locks):** Never use standard `lock` blocks around an `await` statement. Implement `SemaphoreSlim(1, 1)` using a `try/finally` pattern for async synchronization boundaries.

```csharp
public sealed class ConcurrentPipeline
{
    // Native .NET 10 optimized lock type
    private readonly Lock _syncObject = new(); 
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private long _processedCount;

    public void ProcessSynchronously(ReadOnlySpan<byte> payload)
    {
        // Lock object pattern
        lock (_syncObject)
        {
            // Mutex-protected operations
        }
        
        // Lock-free atomic increment
        Interlocked.Increment(ref _processedCount);
    }

    public async Task ProcessAsynchronouslyAsync()
    {
        await _asyncLock.WaitAsync();
        try
        {
            // Thread-safe code block containing await operations
            await Task.Delay(10); 
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}
```

### 3.3 Eradicating Sync-Over-Async Execution (Thread Starvation)
* **Rule:** Code agents and developers must never write blocking invocations (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`) on asynchronous contexts.
* **Consequence:** This practice triggers severe ThreadPool starvation and high CPU context-switching churn under heavy traffic.
* **Library Rule:** Append `.ConfigureAwait(false)` to all internal library operations where access to the UI or request-specific synchronization context is not required.

```csharp
// CORRECT: Pure async execution flowing continuously 
public async Task EnqueuePayloadAsync(byte[] data)
{
    await OutboundSocket.WriteAsync(data).ConfigureAwait(false);
}

// CRITICAL VIOLATION: Deadlocks and starves the .NET ThreadPool
public void BadEnqueue(byte[] data)
{
    OutboundSocket.WriteAsync(data).GetAwaiter().GetResult(); 
}
```

---

## 4. Summary Matrix for Code Agents

| Context / Objective | Recommended Architecture Strategy | Keywords to Emit |
| :--- | :--- | :--- |
| **Parsing & String Slicing** | Stack allocated views, zero heap allocations | `ReadOnlySpan<char>`, `ref struct` |
| **Micro-DTOs / Math Points** | Avoid reference tracking, leverage registers | `readonly struct`, `in` |
| **High Frequency API Hits** | Drop task instantiation on quick returns | `ValueTask<T>` |
| **Asynchronous Locks** | Non-blocking execution state management | `SemaphoreSlim`, `try / finally` |
| **Atomic Counters** | Hardware level fast-path execution | `Interlocked.Increment` |
| **Text Parsing Execution** | Compile-time computed regex state machine | `[GeneratedRegex]` |
