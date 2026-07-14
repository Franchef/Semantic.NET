namespace Semantic.StateMachines;

/// <summary>
/// Represents a typed Moore machine, a finite state machine where behavior depends on the current state.
/// </summary>
/// <typeparam name="TState">Enumeration type representing states.</typeparam>
/// <typeparam name="TInput">Input type used to trigger transitions.</typeparam>
public sealed class Moore<TState, TInput> : StateMachine<TState>
    where TState : struct, Enum
    where TInput : notnull
{
    private readonly Dictionary<TState, Func<TInput, Transition<TState>>> _stateOutputs;
    private readonly Dictionary<(TState State, TInput Input), TState> _transitions;

    public static MooreBuilder<TState, TInput> Builder(TState initialState) => new(initialState);

    internal Moore(
        TState initialState,
        Dictionary<TState, Func<TInput, Transition<TState>>> stateOutputs,
        Dictionary<(TState State, TInput Input), TState> transitions
    )
    {
        ArgumentNullException.ThrowIfNull(stateOutputs);
        ArgumentNullException.ThrowIfNull(transitions);

        StateMachineRuntime.EnsureAllStatesHaveOutputs(stateOutputs);

        _stateOutputs = new Dictionary<TState, Func<TInput, Transition<TState>>>(stateOutputs);
        _transitions = new Dictionary<(TState State, TInput Input), TState>(transitions);
        CurrentState = initialState;
    }

    public void ProcessInput(TInput input)
    {
        StateMachineRuntime.ProcessMooreInput(
            CurrentState,
            input,
            _transitions,
            _stateOutputs,
            nextState => CurrentState = nextState
        );
    }
}
