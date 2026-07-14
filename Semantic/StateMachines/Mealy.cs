namespace Semantic.StateMachines;

/// <summary>
/// Represents a Mealy machine, a finite state machine where outputs depend on both the current state and the inputs.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public class Mealy<T> : StateMachine<T> where T : struct, Enum
{
    private readonly Dictionary<(T State, object Input), TransitionDefinition> _transitions;

    public static MealyBuilder<T> Builder(T initialState) => new(initialState);
    public static MealyBuilder<T, TInput, TOutput> Builder<TInput, TOutput>(T initialState)
        where TInput : notnull => new(initialState);

    internal Mealy(T initialState, Dictionary<(T State, object Input), TransitionDefinition> transitions)
    {
        ArgumentNullException.ThrowIfNull(transitions);
        _transitions = new Dictionary<(T State, object Input), TransitionDefinition>(transitions);
        CurrentState = initialState;
    }

    public object? ProcessInput(object input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return StateMachineRuntime.ProcessMealyInput(
            CurrentState,
            input,
            _transitions,
            transition => transition.ToState,
            transition => transition.Output,
            nextState => CurrentState = nextState
        );
    }

    internal sealed record TransitionDefinition(T ToState, object? Output);
}
