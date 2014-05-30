using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifp.Validation
{
    public delegate ValidationOutcome ValidationFunction<T>(T objectToValidate);

    public abstract class Validator<T>
    {
        public abstract ValidationSummary Validate(T objectToValidate);
    }

    public interface IValidationRule<in T>
    {
        ValidationOutcome ValidateObject(T objectToValidate);
        bool CausesValidationProcessToStop { get; }
    }

    public abstract class ValidationRule<T> : IValidationRule<T>
    {

        public abstract ValidationOutcome ValidateObject(T objectToValidate);

        public virtual bool CausesValidationProcessToStop { get { return false; } }
    }

    public class ValidationRuleDelegate<T> : IValidationRule<T>
    {
        readonly ValidationFunction<T> _ValidationFunction;
        readonly bool _CausesValidationProcessToStop;

        public ValidationRuleDelegate(ValidationFunction<T> validationFunction)
            : this(validationFunction, false)
        {
        }
        public ValidationRuleDelegate(ValidationFunction<T> validationFunction, bool causesValidationProcessToStop)
        {
            _ValidationFunction = validationFunction;
            _CausesValidationProcessToStop = causesValidationProcessToStop;
        }

        protected ValidationFunction<T> ValidationFunction
        {
            get { return _ValidationFunction; }
        }

        public ValidationOutcome ValidateObject(T objectToValidate)
        {
            return ValidationFunction(objectToValidate);
        }

        public bool CausesValidationProcessToStop
        {
            get
            {
                return _CausesValidationProcessToStop;
            }
        }
    }

    public class RuleBasedValidator<T> : Validator<T>
    {
        readonly IValidationRule<T>[] _Rules;

        public RuleBasedValidator(IEnumerable<IValidationRule<T>> rules) : this(rules.ToArray()) { }
        public RuleBasedValidator(params IValidationRule<T>[] rules)
        {
            _Rules = rules;
        }

        protected IValidationRule<T>[] Rules
        {
            get { return _Rules; }
        }

        protected virtual IEnumerable<ValidationOutcome> ProcessValidations(T objectToValidate)
        {
            foreach (var rule in Rules)
            {
                var ruleResult = rule.ValidateObject(objectToValidate);
                yield return ruleResult;
                if (ruleResult.Severity.IsAnError && rule.CausesValidationProcessToStop)
                    yield break;
            }
        }

        public override ValidationSummary Validate(T objectToValidate)
        {
            return new ValidationSummary(ProcessValidations(objectToValidate));
        }
    }
}
