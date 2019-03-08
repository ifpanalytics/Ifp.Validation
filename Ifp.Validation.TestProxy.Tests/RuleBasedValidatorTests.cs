using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ifp.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ifp.Validation.TestProxy.Tests.SupportClasses;

namespace Ifp.Validation.Tests
{
    [TestClass()]
    public class RuleBasedValidatorTests
    {
        [TestMethod()]
        public void CovarianceTest()
        {
            // just needs to compile to show that an animal rule (super class) is compatible to an dog (sub class) rule.
            var validator = new RuleBasedValidator<Dog>(new AnimalMustBeMaleRule());
        }


        [TestMethod()]
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
            Assert.AreSame(r1.Outcome, result.ValidationOutcomes.ElementAt(0));
            Assert.AreSame(r2.Outcome, result.ValidationOutcomes.ElementAt(1));
            Assert.AreSame(r3.Outcome, result.ValidationOutcomes.ElementAt(2));
            Assert.AreSame(r5.Outcome, result.ValidationOutcomes.ElementAt(3));
            Assert.AreSame(r4.Outcome, result.ValidationOutcomes.ElementAt(4));
            Assert.AreSame(r0.Outcome, result.ValidationOutcomes.ElementAt(5));
            Assert.AreSame(r6.Outcome, result.ValidationOutcomes.ElementAt(6));
        }

        [TestMethod()]
        public void RuleBasedValidatorCallsValidationInTheOrderGivenInConstructor()
        {
            int callCounter = 0;
            Action<int> assert = expectedCallCounter => { Assert.AreEqual(expectedCallCounter, callCounter); callCounter++; };
            var r0 = new AnimalTestValidationRule(new ValidationOutcomeSuccess(), () => assert(0));
            var r1 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(1));
            var r2 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(2));
            var r3 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)), () => assert(3));
            var r4 = new AnimalTestValidationRule(new ValidationOutcomeInformation(nameof(ValidationOutcomeInformation)), () => assert(4));
            var r5 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)), () => assert(5));
            var r6 = new AnimalTestValidationRule(new ValidationOutcomeSuccess(), () => assert(6));
            var validator = new RuleBasedValidator<Animal>(r0, r1, r2, r3, r4, r5, r6);
            var result = validator.Validate(null);
            Assert.AreEqual(7, callCounter);
        }

        [TestMethod()]
        public void RuleBasedValidatorBreaksIfCausesValidationProcessToStopIsTrue()
        {
            int callCounter = 0;
            Action<int> assert = expectedCallCounter => { Assert.AreEqual(expectedCallCounter, callCounter); callCounter++; };
            var r0 = new AnimalTestValidationRule(new ValidationOutcomeSuccess(), () => assert(0));
            var r1 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(1));
            var r2 = new AnimalTestValidationRule(new ValidationOutcomeError(nameof(ValidationOutcomeError)), () => assert(2));
            var r3 = new AnimalTestValidationRule(new ValidationOutcomeWarning(nameof(ValidationOutcomeWarning)), () => assert(3), true);
            var r4 = new AnimalTestValidationRule(new ValidationOutcomeInformation(nameof(ValidationOutcomeInformation)), () => Assert.Fail("Should not be called"));
            var validator = new RuleBasedValidator<Animal>(r0, r1, r2, r3, r4);
            var result = validator.Validate(null);
            Assert.AreEqual(4, callCounter);
        }
    }
}