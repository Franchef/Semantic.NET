using System.ComponentModel;
using Semantic.Sequences;

namespace SemanticTests.Sequences
{
    public class PatternMatchesTests
    {
        [Fact]
        public void TestPatternMatches()
        {
            var pattern = PatternMatchesBuilder.StartsWith(1)
                .ContinuesWith(2)
                .EndsWith(3);

            Assert.False(pattern.HasMatch());

            pattern.Next(1);
            Assert.False(pattern.HasMatch());

            pattern.Next(2);
            Assert.False(pattern.HasMatch());

            pattern.Next(3);
            Assert.True(pattern.HasMatch());

            // Test that it resets after a match
            pattern.Next(1);
            Assert.False(pattern.HasMatch());
        }
        
        [Fact]
        public void TestPatternMatches_ResetOnMismatch()
        {
            var pattern = PatternMatchesBuilder.StartsWith(1)
                .ContinuesWith(2)
                .EndsWith(3);

            // Feed a wrong value at the start
            pattern.Next(99); // Should reset
            Assert.False(pattern.HasMatch());

            // Feed a correct value, then a wrong one
            pattern.Next(1);
            pattern.Next(99); // Should reset
            Assert.False(pattern.HasMatch());

            // Feed a partial correct sequence, then a wrong one
            pattern.Next(1);
            pattern.Next(2);
            pattern.Next(99); // Should reset
            Assert.False(pattern.HasMatch());
        }

        [Fact]
        public void TestPatternMatches_ResetAfterMatchAndMismatch()
        {
            var pattern = PatternMatchesBuilder.StartsWith(1)
                .ContinuesWith(2)
                .EndsWith(3);

            // Complete a match
            pattern.Next(1);
            pattern.Next(2);
            pattern.Next(3);
            Assert.True(pattern.HasMatch());

            // Now feed a wrong value after reset
            pattern.Next(99); // Should hit the else branch after reset
            Assert.False(pattern.HasMatch());
        }
    }
}