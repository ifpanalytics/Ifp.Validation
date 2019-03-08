using System;

namespace Ifp.Validation.TestProxy.Tests.SupportClasses
{
    class TestValidationRule<T> : ValidationRule<T>
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

        public override ValidationOutcome ValidateObject(T objectToValidate)
        {
            OnValidateObjectCalled?.Invoke();
            return Outcome;
        }
    }

    class AnimalTestValidationRule : TestValidationRule<Animal>
    {
        public AnimalTestValidationRule(ValidationOutcome outcome)
            : base(outcome)
        {
        }

        public AnimalTestValidationRule(ValidationOutcome outcome, Action onValidateObjectCalled)
            : base(outcome, onValidateObjectCalled)
        {
        }

        public AnimalTestValidationRule(ValidationOutcome outcome, Action onValidateObjectCalled, bool causesValidationProcessToStop)
            : base(outcome, onValidateObjectCalled, causesValidationProcessToStop)
        {
        }
    }

    class ZooTestValidationRule : TestValidationRule<Zoo>
    {
        public ZooTestValidationRule(ValidationOutcome outcome)
            : base(outcome)
        {
        }

        public ZooTestValidationRule(ValidationOutcome outcome, Action onValidateObjectCalled)
            : base(outcome, onValidateObjectCalled)
        {
        }

        public ZooTestValidationRule(ValidationOutcome outcome, Action onValidateObjectCalled, bool causesValidationProcessToStop)
            : base(outcome, onValidateObjectCalled, causesValidationProcessToStop)
        {
        }
    }
}
