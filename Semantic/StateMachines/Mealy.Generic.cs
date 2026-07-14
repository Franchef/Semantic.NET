namespace Semantic.StateMachines;

/// <summary>
/// Represents a typed Mealy machine, a finite state machine where outputs depend on both state and input.
/// </summary>
/// <typeparam name="TState">Enumeration type representing states.</typeparam>
/// <typeparam name="TInput">Input type used to trigger transitions.</typeparam>
/// <typeparam name="TOutput">Output type emitted by transitions.</typeparam>
public sealed class Mealy<TState, TInput, TOutput> : StateMachine<TState>
    where TState : struct, Enum
    where TInput : notnull
{
    private readonly Dictionary<(TState State, TInput Input), TransitionDefinition> _transitions;

    public static MealyBuilder<TState, TInput, TOutput> Builder(TState initialState) => new(initialState);

    internal Mealy(
        TState initialState,
        Dictionary<(TState State, TInput Input), TransitionDefinition> transitions
    )
    {
        ArgumentNullException.ThrowIfNull(transitions);
        _transitions = new Dictionary<(TState State, TInput Input), TransitionDefinition>(transitions);
        CurrentState = initialState;
    }

    public TOutput ProcessInput(TInput input)
    {
        return StateMachineRuntime.ProcessMealyInput(
            CurrentState,
            input,
            _transitions,
            transition => transition.ToState,
            transition => transition.Output,
            nextState => CurrentState = nextState
        );
    }

    internal sealed record TransitionDefinition(TState ToState, TOutput Output);
}
