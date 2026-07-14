using System;
using Semantic.StateMachines;

namespace SemanticTests.StateMachines;

public class MaleyTests
{
    enum TestStates
    {
        StateA,
        StateB,
        StateC
    }

    [Fact]
    public void MealyStateMachine_ShouldThrowOnNullInput()
    {
        var machine = Mealy<TestStates>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits("A->B")
            .Build();

        Assert.Throws<ArgumentNullException>(() => machine.ProcessInput(null!));
    }

    [Fact]
    public void MealyBuilder_ShouldRejectInvalidConfiguration()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Mealy<TestStates>.Builder(TestStates.StateA).From(TestStates.StateA).On(null!)
        );
    }

    [Fact]
    public void MealyBuilder_ShouldRejectDuplicateTransition()
    {
        var builder = Mealy<TestStates>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits("A->B");

        Assert.Throws<InvalidOperationException>(() =>
            builder.From(TestStates.StateA).On("toB").GoTo(TestStates.StateC).Emits("A->C")
        );
    }

    [Fact]
    public void MealyStateMachine_ShouldAllowNullOutput()
    {
        var machine = Mealy<TestStates>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("stay").GoTo(TestStates.StateA).Emits(null)
            .Build();

        var output = machine.ProcessInput("stay");

        Assert.Null(output);
        Assert.Equal(TestStates.StateA, machine.CurrentState);
    }

    [Fact]
    public void TypedMealyBuilder_ShouldRejectDuplicateTransition()
    {
        var builder = Mealy<TestStates, string, int>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits(1);

        Assert.Throws<InvalidOperationException>(() =>
            builder.From(TestStates.StateA).On("toB").GoTo(TestStates.StateC).Emits(2)
        );
    }

    [Fact]
    public void TypedMealyStateMachine_ShouldThrowWhenTransitionIsMissing()
    {
        var machine = Mealy<TestStates, string, int>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits(1)
            .Build();

        Assert.Throws<InvalidOperationException>(() => machine.ProcessInput("missing"));
    }

    [Fact]
    public void MealyStateMachine_ShouldHaveCurrentState()
    {
        // Arrange
        var mealyMachine = Mealy<TestStates>.Builder(TestStates.StateB)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits("A->B")
            .Build();
        // Act
        var currentState = mealyMachine.CurrentState;
        // Assert
        Assert.Equal(TestStates.StateB, currentState);
    }

    [Fact]
    public void MealyStateMachine_ShouldTransitionAndEmitOutput()
    {
        var mealyMachine = Mealy<TestStates>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits("A->B")
            .From(TestStates.StateB).On("toC").GoTo(TestStates.StateC).Emits("B->C")
            .Build();

        var output = mealyMachine.ProcessInput("toB");

        Assert.Equal("A->B", output);
        Assert.Equal(TestStates.StateB, mealyMachine.CurrentState);
    }

    [Fact]
    public void MealyStateMachine_ShouldHandleMultipleSequentialTransitions()
    {
        var mealyMachine = Mealy<TestStates>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits("A->B")
            .From(TestStates.StateB).On("toC").GoTo(TestStates.StateC).Emits("B->C")
            .From(TestStates.StateC).On("toA").GoTo(TestStates.StateA).Emits("C->A")
            .Build();

        var output1 = mealyMachine.ProcessInput("toB");
        var output2 = mealyMachine.ProcessInput("toC");
        var output3 = mealyMachine.ProcessInput("toA");

        Assert.Equal("A->B", output1);
        Assert.Equal("B->C", output2);
        Assert.Equal("C->A", output3);
        Assert.Equal(TestStates.StateA, mealyMachine.CurrentState);
    }

    [Fact]
    public void MealyStateMachine_ShouldThrowWhenTransitionIsMissing()
    {
        var mealyMachine = Mealy<TestStates>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits("A->B")
            .Build();

        Assert.Throws<InvalidOperationException>(() => mealyMachine.ProcessInput("missing"));
    }

    [Fact]
    public void TypedMealyStateMachine_ShouldTransitionAndEmitTypedOutput()
    {
        var mealyMachine = Mealy<TestStates, string, int>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits(10)
            .From(TestStates.StateB).On("toC").GoTo(TestStates.StateC).Emits(20)
            .Build();

        var output = mealyMachine.ProcessInput("toB");

        Assert.Equal(10, output);
        Assert.Equal(TestStates.StateB, mealyMachine.CurrentState);
    }

    [Fact]
    public void TypedMealyStateMachine_ShouldHandleSelfTransitionAndEmitOutput()
    {
        var mealyMachine = Mealy<TestStates, string, int>.Builder(TestStates.StateA)
            .From(TestStates.StateA).On("stay").GoTo(TestStates.StateA).Emits(7)
            .Build();

        var output = mealyMachine.ProcessInput("stay");

        Assert.Equal(7, output);
        Assert.Equal(TestStates.StateA, mealyMachine.CurrentState);
    }

    [Fact]
    public void TypedMealyBuilderFromLegacyEntryPoint_ShouldWork()
    {
        var mealyMachine = Mealy<TestStates>.Builder<string, int>(TestStates.StateA)
            .From(TestStates.StateA).On("toB").GoTo(TestStates.StateB).Emits(99)
            .Build();

        var output = mealyMachine.ProcessInput("toB");

        Assert.Equal(99, output);
        Assert.Equal(TestStates.StateB, mealyMachine.CurrentState);
    }
}
