using Ifp.Validation.TestProxy.Tests.SupportClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ifp.Validation.TestProxy.Tests
{
    [TestClass]
    public class ValidatorCombinerTests
    {
        [TestMethod()]
        public void CombineRuleBasedValidationAndCollectionValidation()
        {
            var zoo = new Zoo();
            var r1WasCalled = false; var r2WasCalled = false; var r3WasCalled = false;
            var ruleBasedValidator = new RuleBasedValidator<Zoo>(new ValidationRuleDelegate<Zoo>(_ =>
            {
                r1WasCalled = true;
                return ValidationOutcome.Success;
            }));
            var collectionValidator = new SubCollectionValidator<Zoo, Animal>(z => z.Animals, new AnimalTestValidationRule(ValidationOutcome.Success, () => r2WasCalled = true));
            var simpleRule = new ZooTestValidationRule(ValidationOutcome.Success, () => r3WasCalled = true);
            var combined = new ValidatorCombiner<Zoo>(ruleBasedValidator, collectionValidator, simpleRule);
            var result = combined.Validate(new Zoo(new Dog()));
            Assert.AreEqual(result.Severity, ValidationOutcome.Success.Severity);
            Assert.AreEqual(r1WasCalled, true);
            Assert.AreEqual(r2WasCalled, true);
            Assert.AreEqual(r3WasCalled, true);
        }
    }
}
