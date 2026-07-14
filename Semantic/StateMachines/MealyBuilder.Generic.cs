namespace Semantic.StateMachines;

public sealed class MealyBuilder<TState, TInput, TOutput>
    where TState : struct, Enum
    where TInput : notnull
{
    private readonly TState _initialState;
    private readonly Dictionary<(TState State, TInput Input), Mealy<TState, TInput, TOutput>.TransitionDefinition>
        _transitions = new();

    public MealyBuilder(TState initialState)
    {
        _initialState = initialState;
    }

    public FromStateBuilder From(TState fromState) => new(this, fromState);

    public Mealy<TState, TInput, TOutput> Build() => Create();

    public Mealy<TState, TInput, TOutput> Create()
    {
        return new Mealy<TState, TInput, TOutput>(_initialState, _transitions);
    }

    internal MealyBuilder<TState, TInput, TOutput> AddTransition(
        TState fromState,
        TInput input,
        TState toState,
        TOutput output
    )
    {
        StateMachineRuntime.EnsureUniqueTransition(_transitions, fromState, input);

        _transitions[(fromState, input)] = new Mealy<TState, TInput, TOutput>.TransitionDefinition(
            toState,
            output
        );
        return this;
    }

    public sealed class FromStateBuilder
    {
        private readonly MealyBuilder<TState, TInput, TOutput> _builder;
        private readonly TState _fromState;

        internal FromStateBuilder(MealyBuilder<TState, TInput, TOutput> builder, TState fromState)
        {
            _builder = builder;
            _fromState = fromState;
        }

        public InputBuilder On(TInput input)
        {
            return new InputBuilder(_builder, _fromState, input);
        }
    }

    public sealed class InputBuilder
    {
        private readonly MealyBuilder<TState, TInput, TOutput> _builder;
        private readonly TState _fromState;
        private readonly TInput _input;

        internal InputBuilder(
            MealyBuilder<TState, TInput, TOutput> builder,
            TState fromState,
            TInput input
        )
        {
            _builder = builder;
            _fromState = fromState;
            _input = input;
        }

        public ToStateBuilder GoTo(TState toState)
        {
            return new ToStateBuilder(_builder, _fromState, _input, toState);
        }
    }

    public sealed class ToStateBuilder
    {
        private readonly MealyBuilder<TState, TInput, TOutput> _builder;
        private readonly TState _fromState;
        private readonly TInput _input;
        private readonly TState _toState;

        internal ToStateBuilder(
            MealyBuilder<TState, TInput, TOutput> builder,
            TState fromState,
            TInput input,
            TState toState
        )
        {
            _builder = builder;
            _fromState = fromState;
            _input = input;
            _toState = toState;
        }

        public MealyBuilder<TState, TInput, TOutput> Emits(TOutput output)
        {
            return _builder.AddTransition(_fromState, _input, _toState, output);
        }
    }
}
