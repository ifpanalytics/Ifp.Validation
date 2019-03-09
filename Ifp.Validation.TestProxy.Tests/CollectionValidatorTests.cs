using FluentAssertions;
using System;
using Xunit;

namespace Ifp.Validation.TestProxy.Tests
{
    public class CollectionValidatorTests
    {

        [Fact]
        public void CollectionValidatorMustPerformRulesFirstThenObjects()
        {
            int ruleCallCounter = 0;
            var rule1 = new ValidationRuleDelegate<int>(i => assertFunction(i, 0));
            var rule2 = new ValidationRuleDelegate<int>(i => assertFunction(i, 1));
            var rule3 = new ValidationRuleDelegate<int>(i => assertFunction(i, 2));
            var col = new int[] { 0, 1, 2, 3 };
            var sut = new CollectionValidator<int>(rule1, rule2, rule3);
            var result = sut.ValidateCollection(col);
            ruleCallCounter.Should().Be(12);

            ValidationOutcome assertFunction(int i, int offset)
            {
                ruleCallCounter.Should().Be((i * 3) + offset);
                ruleCallCounter++;
                return "error".ToFailure(FailureSeverity.Error);
            }
        }

        [Fact]
        public void CollectionValidatorMustPerformRulesFirstThenObjectsAndBreakOnError()
        {
            int ruleCallCounter = 0;
            var rule1 = new ValidationRuleDelegate<int>(i => assertFunction(i, 0));
            var rule2 = new ValidationRuleDelegate<int>(i => assertFunction(i, 1), true);
            var rule3 = new ValidationRuleDelegate<int>(i => { true.Should().Be(false, because: "This rule should never be called."); throw new NotSupportedException("Unreachable code"); });
            var col = new int[] { 0, 1, 2, 3 };
            var sut = new CollectionValidator<int>(rule1, rule2, rule3);
            var result = sut.ValidateCollection(col);
            ruleCallCounter.Should().Be(8);

            ValidationOutcome assertFunction(int i, int offset)
            {
                ruleCallCounter.Should().Be((i * 2) + offset);
                ruleCallCounter++;
                return "error".ToFailure(FailureSeverity.Error);
            }
        }
    }
}
