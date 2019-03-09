using FluentAssertions;
using Ifp.Validation.TestProxy.Tests.SupportClasses;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ifp.Validation.TestProxy.Tests
{
    public class ValidatorCombinerTests
    {
        [Fact]
        public void CombineRuleBasedValidationAndCollectionValidation()
        {
            var callHistory = new List<int>();
            var ruleBasedValidator = new RuleBasedValidator<Zoo>(new ValidationRuleDelegate<Zoo>(_ =>
            {
                callHistory.Add(0);
                return ValidationOutcome.Success;
            }));
            var collectionValidator = new SubCollectionValidator<Zoo, Animal>(z => z.Animals, new AnimalTestValidationRule(ValidationOutcome.Success, () => callHistory.Add(1)));
            var simpleRule = new ZooTestValidationRule(ValidationOutcome.Success, () => callHistory.Add(2));
            var combined = new ValidatorCombiner<Zoo>(ruleBasedValidator, collectionValidator, simpleRule);
            var result = combined.Validate(new Zoo(new Dog()));
            result.Severity.Should().Be(ValidationOutcome.Success.Severity);
            callHistory.Should().HaveCount(3);
            callHistory.Should().Equal(0, 1, 2);
        }
    }
}
