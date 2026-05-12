namespace Semantic.StateMachines;

/// <summary>
/// Represents a Mealy machine, a finite state machine where outputs depend on both the current state and the inputs.
/// </summary>
/// <typeparam name="T">An enumeration type representing the states of the state machine.</typeparam>
public class Mealy<T> : StateMachine<T> where T : struct, Enum
{

}
