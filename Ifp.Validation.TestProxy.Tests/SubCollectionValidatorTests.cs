using FluentAssertions;
using Ifp.Validation.TestProxy.Tests.SupportClasses;
using System.Collections.Generic;
using Xunit;

namespace Ifp.Validation.TestProxy.Tests
{
    public class SubCollectionValidatorTests
    {
        [Fact]
        public void AllItemsOfACollectionAreporcessedInTheRightOrder()
        {
            var callHistory = new List<int>();
            var r0 = new AnimalTestValidationRule(ValidationOutcome.Success, () => callHistory.Add(0));
            var r1 = new AnimalTestValidationRule(ValidationOutcome.Failure(FailureSeverity.Error, "Error1"), () => callHistory.Add(1));
            var r2 = new AnimalTestValidationRule(ValidationOutcome.Failure(FailureSeverity.Warning, "Warning2"), () => callHistory.Add(2), true);
            var r3 = new AnimalTestValidationRule(ValidationOutcome.Failure(FailureSeverity.Warning, "Warning3"), () => callHistory.Add(3));
            var sut = new SubCollectionValidator<Zoo, Animal>(z => z.Animals, new RuleBasedValidator<Animal>(r0, r1, r2, r3));
            var summary = sut.Validate(new Zoo(new Dog(), new Dog()));

            summary.ValidationOutcomes.Should().Equal(r1.Outcome, r1.Outcome, r2.Outcome, r2.Outcome, r0.Outcome, r0.Outcome);
            callHistory.Should().Equal(0, 1, 2, 0, 1, 2);
        }
    }
}
