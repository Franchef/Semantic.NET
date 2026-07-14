namespace Semantic.StateMachines;

public sealed class MealyBuilder<T> where T : struct, Enum
{
    private readonly T _initialState;
    private readonly Dictionary<(T State, object Input), Mealy<T>.TransitionDefinition> _transitions = new();

    public MealyBuilder(T initialState)
    {
        _initialState = initialState;
    }

    public FromStateBuilder From(T fromState) => new(this, fromState);

    public Mealy<T> Build() => Create();

    public Mealy<T> Create()
    {
        return new Mealy<T>(_initialState, _transitions);
    }

    internal MealyBuilder<T> AddTransition(T fromState, object input, T toState, object? output)
    {
        ArgumentNullException.ThrowIfNull(input);
        StateMachineRuntime.EnsureUniqueTransition(_transitions, fromState, input);

        _transitions[(fromState, input)] = new Mealy<T>.TransitionDefinition(toState, output);
        return this;
    }

    public sealed class FromStateBuilder
    {
        private readonly MealyBuilder<T> _builder;
        private readonly T _fromState;

        internal FromStateBuilder(MealyBuilder<T> builder, T fromState)
        {
            _builder = builder;
            _fromState = fromState;
        }

        public InputBuilder On(object input)
        {
            ArgumentNullException.ThrowIfNull(input);
            return new InputBuilder(_builder, _fromState, input);
        }
    }

    public sealed class InputBuilder
    {
        private readonly MealyBuilder<T> _builder;
        private readonly T _fromState;
        private readonly object _input;

        internal InputBuilder(MealyBuilder<T> builder, T fromState, object input)
        {
            _builder = builder;
            _fromState = fromState;
            _input = input;
        }

        public ToStateBuilder GoTo(T toState)
        {
            return new ToStateBuilder(_builder, _fromState, _input, toState);
        }
    }

    public sealed class ToStateBuilder
    {
        private readonly MealyBuilder<T> _builder;
        private readonly T _fromState;
        private readonly object _input;
        private readonly T _toState;

        internal ToStateBuilder(MealyBuilder<T> builder, T fromState, object input, T toState)
        {
            _builder = builder;
            _fromState = fromState;
            _input = input;
            _toState = toState;
        }

        public MealyBuilder<T> Emits(object? output)
        {
            return _builder.AddTransition(_fromState, _input, _toState, output);
        }
    }
}
