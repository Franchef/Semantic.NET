namespace Semantic.StateMachines;

public class MooreBuilder<T> where T : struct, Enum
{
    private readonly Dictionary<T, Func<object, StateMachine<T>.Transition<T>>> _stateOutputs = new();
    private readonly Dictionary<(T State, object Input), T> _transitions = new();
    private readonly T initialState;

    public MooreBuilder(T initialState)
    {
        this.initialState = initialState;
    }
    public MooreBuilder<T> WithState(T state, Func<object, StateMachine<T>.Transition<T>> output)
    {
        _stateOutputs[state] = output;
        return this;
    }

    public MooreBuilder<T> WithTransition(T fromState, object input, T toState)
    {
        _transitions[(fromState, input)] = toState;
        return this;
    }

    public Moore<T> Create()
    {
        return new Moore<T>(initialState);
    }
}