using FluentAssertions;
using Ifp.Validation.TestProxy.Tests.SupportClasses;
using Xunit;

namespace Ifp.Validation.TestProxy.Tests
{
    public class RuleBasedValidatorTests
    {
        [Fact]
        public void CovarianceTest()
        {
            // just needs to compile to show that an animal rule (super class) is compatible to an dog (sub class) rule.
            var validator = new RuleBasedValidator<Dog>(new AnimalMustBeMaleRule());
        }


        [Fact]
        public void ValidationSummaryIsOrderedBySeverityAsNumber()
        {
            var r0 = new AnimalTestValidationRule(new ValidationOutcomeSuccess());
            var r1 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)));
            var r2 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)));
            var r3 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)));
            var r4 = new AnimalTestValidationRule(new ValidationOutcomeInformation(nameof(ValidationOutcomeInformation)));
            var r5 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)));
            var r6 = new AnimalTestValidationRule(new ValidationOutcomeSuccess());
            var validator = new RuleBasedValidator<Animal>(r0, r1, r2, r3, r4, r5, r6);
            var result = validator.Validate(null);
            result.ValidationOutcomes.Should().Equal(
                r1.Outcome,
                r2.Outcome,
                r3.Outcome,
                r5.Outcome,
                r4.Outcome,
                r0.Outcome,
                r6.Outcome);
        }

        [Fact]
        public void RuleBasedValidatorCallsValidationInTheOrderGivenInConstructor()
        {
            int callCounter = 0;
            var r0 = new AnimalTestValidationRule(new ValidationOutcomeSuccess(), () => assert(0));
            var r1 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(1));
            var r2 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(2));
            var r3 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)), () => assert(3));
            var r4 = new AnimalTestValidationRule(new ValidationOutcomeInformation(nameof(ValidationOutcomeInformation)), () => assert(4));
            var r5 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)), () => assert(5));
            var r6 = new AnimalTestValidationRule(new ValidationOutcomeSuccess(), () => assert(6));
            var validator = new RuleBasedValidator<Animal>(r0, r1, r2, r3, r4, r5, r6);
            var result = validator.Validate(null);
            callCounter.Should().Be(7);

            void assert(int expectedCallCounter) { callCounter.Should().Be(expectedCallCounter); callCounter++; }
        }

        [Fact]
        public void RuleBasedValidatorBreaksIfCausesValidationProcessToStopIsTrue()
        {
            int callCounter = 0;
            var r0 = new AnimalTestValidationRule(new ValidationOutcomeSuccess(), () => assert(0));
            var r1 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(1));
            var r2 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(2));
            var r3 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)), () => assert(3), true);
            var r4 = new AnimalTestValidationRule(new ValidationOutcomeInformation(nameof(ValidationOutcomeInformation)), () => true.Should().Be(false, because: "Should not be called"));
            var validator = new RuleBasedValidator<Animal>(r0, r1, r2, r3, r4);
            var result = validator.Validate(null);
            callCounter.Should().Be(4);

            void assert(int expectedCallCounter) { callCounter.Should().Be(expectedCallCounter); callCounter++; }
        }
    }
}