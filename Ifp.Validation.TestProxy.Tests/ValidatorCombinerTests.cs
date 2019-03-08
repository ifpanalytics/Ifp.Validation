using Ifp.Validation.TestProxy.Tests.SupportClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Ifp.Validation.TestProxy.Tests
{
    [TestClass]
    public class ValidatorCombinerTests
    {
        [TestMethod()]
        public void CombineRuleBasedValidationAndCollectionValidation()
        {
            var zoo = new Zoo();
            var ruleBasedValidator = new RuleBasedValidator<Zoo>(new ValidationRuleDelegate<Zoo>(_ => ValidationOutcome.Success));
            var collectionValidator = new SubCollectionValidator<Zoo, Animal>(z => z.Animals, new AnimalMustBeMaleRule());
            var simpleRule = new ZooTestValidationRule(ValidationOutcome.Success);
            var combined = new ValidatorCombiner<Zoo>(ruleBasedValidator, collectionValidator, simpleRule);
        }
    }
}
