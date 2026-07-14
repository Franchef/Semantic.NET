namespace Semantic.StateMachines;

public sealed class MooreBuilder<TState, TInput>
    where TState : struct, Enum
    where TInput : notnull
{
    private readonly Dictionary<TState, Func<TInput, StateMachine<TState>.Transition<TState>>> _stateOutputs =
        new();
    private readonly Dictionary<(TState State, TInput Input), TState> _transitions = new();
    private readonly TState _initialState;

    public MooreBuilder(TState initialState)
    {
        _initialState = initialState;
    }

    public MooreBuilder<TState, TInput> WithState(
        TState state,
        Func<TInput, StateMachine<TState>.Transition<TState>> output
    )
    {
        ArgumentNullException.ThrowIfNull(output);
        _stateOutputs[state] = output;
        return this;
    }

    public MooreBuilder<TState, TInput> WithTransition(TState fromState, TInput input, TState toState)
    {
        StateMachineRuntime.EnsureUniqueTransition(_transitions, fromState, input);

        _transitions[(fromState, input)] = toState;
        return this;
    }

    public FromStateBuilder From(TState fromState) => new(this, fromState);

    public Moore<TState, TInput> Build() => Create();

    public Moore<TState, TInput> Create()
    {
        return new Moore<TState, TInput>(_initialState, _stateOutputs, _transitions);
    }

    public sealed class FromStateBuilder
    {
        private readonly MooreBuilder<TState, TInput> _builder;
        private readonly TState _fromState;

        internal FromStateBuilder(MooreBuilder<TState, TInput> builder, TState fromState)
        {
            _builder = builder;
            _fromState = fromState;
        }

        public OnInputBuilder On(TInput input)
        {
            return new OnInputBuilder(_builder, _fromState, input);
        }
    }

    public sealed class OnInputBuilder
    {
        private readonly MooreBuilder<TState, TInput> _builder;
        private readonly TState _fromState;
        private readonly TInput _input;

        internal OnInputBuilder(MooreBuilder<TState, TInput> builder, TState fromState, TInput input)
        {
            _builder = builder;
            _fromState = fromState;
            _input = input;
        }

        public MooreBuilder<TState, TInput> GoTo(TState toState)
        {
            return _builder.WithTransition(_fromState, _input, toState);
        }
    }
}
