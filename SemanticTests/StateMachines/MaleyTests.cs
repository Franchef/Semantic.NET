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
    public void MealyStateMachine_ShouldHaveCurrentState()
    {
        // Arrange
        var mealyMachine = new Mealy<TestStates>();
        // Act
        var currentState = mealyMachine.CurrentState;
        // Assert
        Assert.Equal(default(TestStates), currentState);
    }

}
