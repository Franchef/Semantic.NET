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
    public void MooreStateMachine_ShouldHaveCurrentState()
    {
        // Arrange
        var mooreMachine = Moore<TestStates>.Builder(TestStates.StateA)
            .Create();

        // Act
        var currentState = mooreMachine.CurrentState;

        // Assert
        Assert.Equal(default(TestStates), currentState);
    }

    [Fact]
    public void MooreStateMachine_ShouldAllowStateTransitions()
    {
        // Arrange
        var builder = Moore<TestStates>.Builder(TestStates.StateA)
            .WithState(TestStates.StateA, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .WithState(TestStates.StateB, _ => StateMachine<TestStates>.Transition<TestStates>.NoTransition())
            .WithTransition(TestStates.StateA, "toB", TestStates.StateB)
            .WithTransition(TestStates.StateB, "toA", TestStates.StateA);

        var mooreMachine = builder.Create();

        // Act
        var currentState = mooreMachine.CurrentState;

        // Assert
        Assert.Equal(TestStates.StateA, currentState);
    }

}
