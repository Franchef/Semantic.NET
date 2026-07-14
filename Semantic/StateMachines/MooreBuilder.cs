namespace Semantic.StateMachines;

public class MooreBuilder<T> where T : struct, Enum
{
    private readonly Dictionary<T, Func<object, StateMachine<T>.Transition<T>>> _stateOutputs = new();
    private readonly Dictionary<(T State, object Input), T> _transitions = new();
    private readonly T _initialState;

    public MooreBuilder(T initialState)
    {
        _initialState = initialState;
    }

    public MooreBuilder<T> WithState(T state, Func<object, StateMachine<T>.Transition<T>> output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _stateOutputs[state] = output;
        return this;
    }

    public MooreBuilder<T> WithTransition(T fromState, object input, T toState)
    {
        ArgumentNullException.ThrowIfNull(input);
        StateMachineRuntime.EnsureUniqueTransition(_transitions, fromState, input);
        _transitions[(fromState, input)] = toState;
        return this;
    }

    public FromStateBuilder From(T fromState) => new(this, fromState);

    public Moore<T> Build() => Create();

    public Moore<T> Create()
    {
        return new Moore<T>(_initialState, _stateOutputs, _transitions);
    }

    public sealed class FromStateBuilder
    {
        private readonly MooreBuilder<T> _builder;
        private readonly T _fromState;

        internal FromStateBuilder(MooreBuilder<T> builder, T fromState)
        {
            _builder = builder;
            _fromState = fromState;
        }

        public OnInputBuilder On(object input)
        {
            ArgumentNullException.ThrowIfNull(input);
            return new OnInputBuilder(_builder, _fromState, input);
        }
    }

    public sealed class OnInputBuilder
    {
        private readonly MooreBuilder<T> _builder;
        private readonly T _fromState;
        private readonly object _input;

        internal OnInputBuilder(MooreBuilder<T> builder, T fromState, object input)
        {
            _builder = builder;
            _fromState = fromState;
            _input = input;
        }

        public MooreBuilder<T> GoTo(T toState)
        {
            return _builder.WithTransition(_fromState, _input, toState);
        }
    }
}