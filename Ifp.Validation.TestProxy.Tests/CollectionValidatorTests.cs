using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ifp.Validation;


namespace Ifp.Validation.TestProxy.Tests
{
    [TestClass]
    public class CollectionValidatorTests
    {

        [TestMethod]
        public void CollectionValidatorMustPerformObjectsFirstThenRules()
        {
            int ruleCallCounter = 0;
            Func<int, int, ValidationOutcome> assertFunction = (i, offset) => { Assert.AreEqual(i + offset, ruleCallCounter); ruleCallCounter++; return "error".ToFailure(FailureSeverity.Error); };
            var rule1 = new ValidationRuleDelegate<int>(i => assertFunction(i, 0));
            var rule2 = new ValidationRuleDelegate<int>(i => assertFunction(i, 4));
            var rule3 = new ValidationRuleDelegate<int>(i => assertFunction(i, 8));
            var col = new int[] { 0, 1, 2, 3 };
            var sut = new CollectionValidator<int>(rule1, rule2, rule3);
            sut.OutterLoopOverRulesInnerLoopOverObjects = true;
            var result = sut.ValidateCollection(col).ToList();
            Assert.AreEqual(12, ruleCallCounter);
        }

        [TestMethod]
        public void CollectionValidatorMustPerformObjectsFirstThenRulesAndBreakOnError()
        {
            int ruleCallCounter = 0;
            Func<int, int, ValidationOutcome> assertFunction = (i, offset) => { Assert.AreEqual(i + offset, ruleCallCounter); ruleCallCounter++; return "error".ToFailure(FailureSeverity.Error); };
            var rule1 = new ValidationRuleDelegate<int>(i => assertFunction(i, 0));
            var rule2 = new ValidationRuleDelegate<int>(i => assertFunction(i, 4), true);
            var rule3 = new ValidationRuleDelegate<int>(i => assertFunction(i, 8));
            var col = new int[] { 0, 1, 2, 3 };
            var sut = new CollectionValidator<int>(rule1, rule2, rule3);
            sut.OutterLoopOverRulesInnerLoopOverObjects = true;
            var result = sut.ValidateCollection(col).ToList();
            Assert.AreEqual(5, ruleCallCounter);
        }

        [TestMethod]
        public void CollectionValidatorMustPerformRulesFirstThenObjects()
        {
            int ruleCallCounter = 0;
            Func<int, int, ValidationOutcome> assertFunction = (i, offset) => { Assert.AreEqual((i * 3) + offset, ruleCallCounter); ruleCallCounter++; return "error".ToFailure(FailureSeverity.Error); };
            var rule1 = new ValidationRuleDelegate<int>(i => assertFunction(i, 0));
            var rule2 = new ValidationRuleDelegate<int>(i => assertFunction(i, 1));
            var rule3 = new ValidationRuleDelegate<int>(i => assertFunction(i, 2));
            var col = new int[] { 0, 1, 2, 3 };
            var sut = new CollectionValidator<int>(rule1, rule2, rule3);
            sut.OutterLoopOverRulesInnerLoopOverObjects = false;
            var result = sut.ValidateCollection(col).ToList();
            Assert.AreEqual(12, ruleCallCounter);
        }

        [TestMethod]
        public void CollectionValidatorMustPerformRulesFirstThenObjectsAndBreakOnError()
        {
            int ruleCallCounter = 0;
            Func<int, int, ValidationOutcome> assertFunction = (i, offset) => { Assert.AreEqual((i * 2) + offset, ruleCallCounter); ruleCallCounter++; return "error".ToFailure(FailureSeverity.Error); };
            var rule1 = new ValidationRuleDelegate<int>(i => assertFunction(i, 0));
            var rule2 = new ValidationRuleDelegate<int>(i => assertFunction(i, 1), true);
            var rule3 = new ValidationRuleDelegate<int>(i => assertFunction(i, 2));
            var col = new int[] { 0, 1, 2, 3 };
            var sut = new CollectionValidator<int>(rule1, rule2, rule3);
            sut.OutterLoopOverRulesInnerLoopOverObjects = false;
            var result = sut.ValidateCollection(col).ToList();
            Assert.AreEqual(8, ruleCallCounter);
        }
    }
}
