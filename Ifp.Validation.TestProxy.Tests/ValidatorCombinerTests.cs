using Ifp.Validation.TestProxy.Tests.SupportClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation.TestProxy.Tests
{
    [TestClass]
    public class ValidatorCombinerTests
    {
        [TestMethod()]
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
            Assert.AreEqual(result.Severity, ValidationOutcome.Success.Severity);
            Assert.AreEqual(callHistory.Count, 3);
            Assert.IsTrue(callHistory.Select((item, index) => item == index).All(t => t));
        }
    }
}
