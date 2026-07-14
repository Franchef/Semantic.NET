using System;

namespace Semantic.StateMachines;

/// <summary>
/// Represents a Moore machine, a finite state machine where outputs depend only on the current state.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public class Moore<T> : StateMachine<T> where T : struct, Enum
{
    private readonly Dictionary<T, Func<object, Transition<T>>> _stateOutputs;
    private readonly Dictionary<(T State, object Input), T> _transitions;

    public static MooreBuilder<T> Builder(T initialState) => new MooreBuilder<T>(initialState);
    public static MooreBuilder<T, TInput> Builder<TInput>(T initialState)
        where TInput : notnull => new(initialState);

    internal Moore(
        T initialState,
        Dictionary<T, Func<object, Transition<T>>> stateOutputs,
        Dictionary<(T State, object Input), T> transitions
    )
    {
        ArgumentNullException.ThrowIfNull(stateOutputs);
        ArgumentNullException.ThrowIfNull(transitions);

        StateMachineRuntime.EnsureAllStatesHaveOutputs(stateOutputs);
        _stateOutputs = new Dictionary<T, Func<object, Transition<T>>>(stateOutputs);
        _transitions = new Dictionary<(T State, object Input), T>(transitions);
        CurrentState = initialState;
    }

    public void ProcessInput(object input)
    {
        ArgumentNullException.ThrowIfNull(input);

        StateMachineRuntime.ProcessMooreInput(
            CurrentState,
            input,
            _transitions,
            _stateOutputs,
            nextState => CurrentState = nextState
        );
    }
}