namespace Semantic.StateMachines;

/// <summary>
/// Provides a base class for implementing finite state machines.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public abstract class StateMachine<T> where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the current state of the state machine.
    /// </summary>
    public T CurrentState { get; protected set; }

    public record Transition<TState> where TState : struct, Enum
    {
        public required TState? State { get; init; }

        public static Transition<TState> To(TState state) => new Transition<TState> { State = state };
        public static Transition<TState> NoTransition() => new Transition<TState> { State = null };
    }
}
