namespace Semantic.StateMachines;

internal static class StateMachineRuntime
{
    public static void EnsureAllStatesHaveOutputs<TState, TInput>(
        IDictionary<TState, Func<TInput, StateMachine<TState>.Transition<TState>>> stateOutputs
    )
        where TState : struct, Enum
    {
        foreach (var state in Enum.GetValues<TState>())
        {
            if (!stateOutputs.ContainsKey(state))
            {
                throw new ArgumentException($"State {state} is not defined in the state outputs.");
            }
        }
    }

    public static void EnsureUniqueTransition<TState, TInput, TValue>(
        IDictionary<(TState State, TInput Input), TValue> transitions,
        TState fromState,
        TInput input
    )
        where TState : struct, Enum
        where TInput : notnull
    {
        if (transitions.ContainsKey((fromState, input)))
        {
            throw new InvalidOperationException(
                $"Transition already defined for state {fromState} and input {input}."
            );
        }
    }

    public static void ProcessMooreInput<TState, TInput>(
        TState currentState,
        TInput input,
        IDictionary<(TState State, TInput Input), TState> transitions,
        IDictionary<TState, Func<TInput, StateMachine<TState>.Transition<TState>>> stateOutputs,
        Action<TState> setCurrentState
    )
        where TState : struct, Enum
        where TInput : notnull
    {
        if (transitions.TryGetValue((currentState, input), out var nextState))
        {
            if (!EqualityComparer<TState>.Default.Equals(nextState, currentState))
            {
                setCurrentState(nextState);
            }
            return;
        }

        if (!stateOutputs.TryGetValue(currentState, out var outputFunc))
        {
            throw new InvalidOperationException(
                $"No transition defined from state {currentState} on input {input}."
            );
        }

        var transition = outputFunc(input)
            ?? throw new InvalidOperationException(
                $"Output function for state {currentState} returned null transition."
            );

        if (
            transition.State is not null &&
            !EqualityComparer<TState>.Default.Equals(transition.State.Value, currentState)
        )
        {
            setCurrentState(transition.State.Value);
        }
    }

    public static TOutput ProcessMealyInput<TState, TInput, TTransition, TOutput>(
        TState currentState,
        TInput input,
        IDictionary<(TState State, TInput Input), TTransition> transitions,
        Func<TTransition, TState> getToState,
        Func<TTransition, TOutput> getOutput,
        Action<TState> setCurrentState
    )
        where TState : struct, Enum
        where TInput : notnull
    {
        if (!transitions.TryGetValue((currentState, input), out var transition))
        {
            throw new InvalidOperationException(
                $"No transition defined from state {currentState} on input {input}."
            );
        }

        var nextState = getToState(transition);
        if (!EqualityComparer<TState>.Default.Equals(nextState, currentState))
        {
            setCurrentState(nextState);
        }

        return getOutput(transition);
    }
}
