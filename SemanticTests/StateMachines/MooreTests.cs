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
        var mooreMachine = new Moore<TestStates>();

        // Act
        var currentState = mooreMachine.CurrentState;

        // Assert
        Assert.Equal(default(TestStates), currentState);
    }

}
