using System;
using Semantic.StateMachines;

namespace SemanticTests.StateMachines;

public class MooreTests
{
    enum TestStates
    {
        StateA,
        StateB,
        StateC
    }

    [Fact]
    public void StateMachineTransitionEvent_ShouldBeRaisedOnlyWhenStateChanges()
    {
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .From(TestStates.StateA).On("toA").GoTo(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB)
            .Build();

        int eventCount = 0;
        TestStates from = default;
        TestStates to = default;
        machine.OnTransition += (_, args) =>
        {
            eventCount++;
            from = args.FromStatus;
            to = args.ToStatus;
        };

        machine.ProcessInput("toA");
        machine.ProcessInput("toB");

        Assert.Equal(1, eventCount);
        Assert.Equal(TestStates.StateA, from);
        Assert.Equal(TestStates.StateB, to);
    }

    [Fact]
    public void MooreStateMachine_ShouldUseStateOutputWhenNoExplicitTransitionMatches()
    {
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, input =>
                Equals(input, "toC")
                    ? StateMachine<TestStates>.GoToState(TestStates.StateC)
                    : StateMachine<TestStates>.NoTransition()
            )
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .Build();

        machine.ProcessInput("toC");

        Assert.Equal(TestStates.StateC, machine.CurrentState);
    }

    [Fact]
    public void MooreStateMachine_ShouldPreferExplicitTransitionOverStateOutputFallback()
    {
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, input =>
                Equals(input, "go")
                    ? StateMachine<TestStates>.GoToState(TestStates.StateC)
                    : StateMachine<TestStates>.NoTransition()
            )
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .From(TestStates.StateA).On("go").GoTo(TestStates.StateB)
            .Build();

        machine.ProcessInput("go");

        Assert.Equal(TestStates.StateB, machine.CurrentState);
    }

    [Fact]
    public void MooreStateMachine_ShouldHandleMultipleSequentialTransitions()
    {
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB)
            .From(TestStates.StateB).On("toC").GoTo(TestStates.StateC)
            .From(TestStates.StateC).On("toA").GoTo(TestStates.StateA)
            .Build();

        machine.ProcessInput("toB");
        machine.ProcessInput("toC");
        machine.ProcessInput("toA");

        Assert.Equal(TestStates.StateA, machine.CurrentState);
    }

    [Fact]
    public void MooreStateMachine_ShouldThrowWhenStateOutputReturnsNullTransition()
    {
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => null!)
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .Build();

        Assert.Throws<InvalidOperationException>(() => machine.ProcessInput("any"));
    }

    [Fact]
    public void MooreStateMachine_ShouldThrowOnNullInput()
    {
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .Build();

        Assert.Throws<ArgumentNullException>(() => machine.ProcessInput(null!));
    }

    [Fact]
    public void MooreBuilder_ShouldRejectInvalidConfiguration()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Moore<TestStates>.Builder(TestStates.StateA)
                .WithState(TestStates.StateA, null!)
        );

        Assert.Throws<ArgumentNullException>(() =>
            Moore<TestStates>.Builder(TestStates.StateA)
                .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
                .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
                .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
                .WithTransition(TestStates.StateA, null!, TestStates.StateB)
        );
    }

    [Fact]
    public void MooreBuilder_ShouldRejectDuplicateTransition()
    {
        var builder = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .WithTransition(TestStates.StateA, "toB", TestStates.StateB);

        Assert.Throws<InvalidOperationException>(() =>
            builder.WithTransition(TestStates.StateA, "toB", TestStates.StateC)
        );
    }

    [Fact]
    public void MooreBuilder_ShouldRequireAllStatesInConfiguration()
    {
        Assert.Throws<ArgumentException>(() =>
            Moore<TestStates>.Builder(TestStates.StateA)
                .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
                .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
                .Build()
        );
    }

    [Fact]
    public void TypedMooreBuilder_ShouldRejectDuplicateTransition()
    {
        var builder = Moore<TestStates, string>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .WithTransition(TestStates.StateA, "toB", TestStates.StateB);

        Assert.Throws<InvalidOperationException>(() =>
            builder.WithTransition(TestStates.StateA, "toB", TestStates.StateC)
        );
    }

    [Fact]
    public void TypedMooreStateMachine_ShouldThrowWhenStateOutputReturnsNullTransition()
    {
        var machine = Moore<TestStates, string>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => null!)
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .Build();

        Assert.Throws<InvalidOperationException>(() => machine.ProcessInput("any"));
    }

    [Fact]
    public void TypedMooreBuilder_ShouldRequireAllStatesInConfiguration()
    {
        Assert.Throws<ArgumentException>(() =>
            Moore<TestStates, string>.Builder(TestStates.StateA)
                .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
                .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
                .Build()
        );
    }


    [Fact]
    public void MooreStateMachine_ShouldHaveCurrentState()
    {
        // Arrange
        var mooreMachineBuilder = Moore<TestStates>.Builder(TestStates.StateB);

        mooreMachineBuilder.WithState(TestStates.StateA, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition());
        mooreMachineBuilder.WithState(TestStates.StateB, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition());
        mooreMachineBuilder.WithState(TestStates.StateC, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition());

        var mooreMachine = mooreMachineBuilder.Create();

        // Act
        var currentState = mooreMachine.CurrentState;

        // Assert
        Assert.Equal(TestStates.StateB, currentState);
    }

    [Fact]
    public void MooreStateMachine_ShouldAllowStateTransitions()
    {
        // Arrange
        var builder = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB)
            .From(TestStates.StateB).On("toA").GoTo(TestStates.StateA);

        var mooreMachine = builder.Create();

        // Act
        mooreMachine.ProcessInput("toB");
        var currentState = mooreMachine.CurrentState;

        // Assert
        Assert.Equal(TestStates.StateB, currentState);
    }

    [Fact]
    public void MooreStateMachine_ShouldKeepStateWhenNoTransitionIsDefined()
    {
        // Arrange
        var machine = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .Build();

        // Act
        machine.ProcessInput("missing");

        // Assert
        Assert.Equal(TestStates.StateA, machine.CurrentState);
    }

    [Fact]
    public void TypedMooreStateMachine_ShouldApplyTypedTransition()
    {
        var machine = Moore<TestStates, string>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB)
            .Build();

        machine.ProcessInput("toB");

        Assert.Equal(TestStates.StateB, machine.CurrentState);
    }

    [Fact]
    public void TypedMooreStateMachine_ShouldUseStateOutputWhenNoExplicitTransitionMatches()
    {
        var machine = Moore<TestStates, string>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, input =>
                input == "toC"
                    ? StateMachine<TestStates>.GoToState(TestStates.StateC)
                    : StateMachine<TestStates>.NoTransition()
            )
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .Build();

        machine.ProcessInput("toC");

        Assert.Equal(TestStates.StateC, machine.CurrentState);
    }

    [Fact]
    public void TypedMooreBuilderFromLegacyEntryPoint_ShouldWork()
    {
        var machine = Moore<TestStates>.Builder<string>(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.NoTransition())
            .WithState(TestStates.StateC, _ => StateMachine<TestStates>.NoTransition())
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB)
            .Build();

        machine.ProcessInput("toB");

        Assert.Equal(TestStates.StateB, machine.CurrentState);
    }
}
