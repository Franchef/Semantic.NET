using System;

namespace Semantic.StateMachines;

/// <summary>
/// Represents a Moore machine, a finite state machine where outputs depend only on the current state.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public class Moore<T> : StateMachine<T> where T : struct, Enum
{
    private readonly Dictionary<T, Func<object, Transition<T>>> _stateOutputs;

    public static MooreBuilder<T> Builder(T initialState) => new MooreBuilder<T>(initialState);

    internal Moore(T initialState, 
    Dictionary<T, Func<object, Transition<T>>> stateOutputs)
    {
        foreach (var state in Enum.GetValues<T>())
        {
            if(stateOutputs.ContainsKey(state)) continue;
                throw new ArgumentException($"State {state} is not defined in the state outputs.");
        }
        _stateOutputs = stateOutputs;
        CurrentState = initialState;
    }

    public void ProcessInput(object input)
    {
        if (_stateOutputs.TryGetValue(CurrentState, out var outputFunc))
        {
            var transition = outputFunc(input);
            if(transition.State is not null && !EqualityComparer<T>.Default.Equals(transition.State.Value, CurrentState))
            {
                CurrentState = transition.State.Value;
            }
        }
        else
        {
            throw new InvalidOperationException($"No transition defined from state {CurrentState} on input {input}");
        }
    }
}