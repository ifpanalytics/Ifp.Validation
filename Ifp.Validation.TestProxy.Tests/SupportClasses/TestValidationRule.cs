using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.SupportClasses
{
    class TestValidationRule : ValidationRule<Animal>
    {
        bool _CausesValidationProcessToStop = false;
        public TestValidationRule(ValidationOutcome outcome) : this(outcome, null)
        {
        }
        public TestValidationRule(ValidationOutcome outcome, Action onValidateObjectCalled) : this(outcome, onValidateObjectCalled, false)
        {
        }

        public TestValidationRule(ValidationOutcome outcome, Action onValidateObjectCalled, bool causesValidationProcessToStop)
        {
            Outcome = outcome;
            OnValidateObjectCalled = onValidateObjectCalled;
            _CausesValidationProcessToStop = causesValidationProcessToStop;
        }

        public override bool CausesValidationProcessToStop => _CausesValidationProcessToStop;
        public ValidationOutcome Outcome { get; }
        public Action OnValidateObjectCalled { get; }

        public override ValidationOutcome ValidateObject(Animal objectToValidate)
        {
            OnValidateObjectCalled?.Invoke();
            return Outcome;
        }
    }
}
