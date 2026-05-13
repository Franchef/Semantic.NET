using System;

namespace Semantic.StateMachines;

/// <summary>
/// Represents a Moore machine, a finite state machine where outputs depend only on the current state.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public class Moore<T> : StateMachine<T> where T : struct, Enum
{
    public static MooreBuilder<T> Builder(T initialState) => new MooreBuilder<T>(initialState);

    internal Moore(T initialState)
    {
        CurrentState = initialState;
    }
}