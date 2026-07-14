namespace Semantic.StateMachines;

/// <summary>
/// Provides a base class for implementing finite state machines.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public abstract class StateMachine<T> where T : struct, Enum
{
    private readonly object _stateLock = new();
    private T _currentState;

    /// <summary>
    /// Raised when the current state transitions to a different value.
    /// </summary>
    public event EventHandler<TransitionEventArgs>? OnTransition;

    /// <summary>
    /// Gets or sets the current state of the state machine.
    /// </summary>
    /// <remarks>This property is thread-safe and can be safely accessed from multiple threads.</remarks>
    public T CurrentState
    {
        get
        {
            lock (_stateLock)
            {
                return _currentState;
            }
        }
        protected set
        {
            T fromStatus = default;
            var hasChanged = false;
            EventHandler<TransitionEventArgs>? handler = null;

            lock (_stateLock)
            {
                if (EqualityComparer<T>.Default.Equals(_currentState, value))
                {
                    return;
                }

                fromStatus = _currentState;
                _currentState = value;
                hasChanged = true;
                handler = OnTransition;
            }

            if (hasChanged)
            {
                handler?.Invoke(this, new TransitionEventArgs(fromStatus, value));
            }
        }
    }

    public sealed class TransitionEventArgs : EventArgs
    {
        public TransitionEventArgs(T fromStatus, T toStatus)
        {
            FromStatus = fromStatus;
            ToStatus = toStatus;
        }

        public T FromStatus { get; }
        public T ToStatus { get; }
    }


    public static Transition<T> GoToState(T state) => StateMachine<T>.Transition<T>.To(state);
    public static Transition<T> NoTransition() => StateMachine<T>.Transition<T>.NoTransition();

    public record Transition<TState> where TState : struct, Enum
    {
        public required TState? State { get; init; }

        public static Transition<TState> To(TState state) => new Transition<TState> { State = state };
        public static Transition<TState> NoTransition() => new Transition<TState> { State = null };
    }
}
