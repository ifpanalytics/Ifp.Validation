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

        public virtual bool CausesValidationProcessToStop => false;
    }

    public class ValidationRuleDelegate<T> : IValidationRule<T>
    {

        public ValidationRuleDelegate(ValidationFunction<T> validationFunction)
            : this(validationFunction, false)
        {
        }
        public ValidationRuleDelegate(ValidationFunction<T> validationFunction, bool causesValidationProcessToStop)
        {
            ValidationFunction = validationFunction;
            CausesValidationProcessToStop = causesValidationProcessToStop;
        }

        protected ValidationFunction<T> ValidationFunction { get; }

        public ValidationOutcome ValidateObject(T objectToValidate) => ValidationFunction(objectToValidate);

        public bool CausesValidationProcessToStop { get; }
    }

    public class RuleBasedValidator<T> : Validator<T>
    {

        public RuleBasedValidator(IEnumerable<IValidationRule<T>> rules) : this(rules.ToArray()) { }
        public RuleBasedValidator(params IValidationRule<T>[] rules)
        {
            Rules = rules;
        }

        protected IValidationRule<T>[] Rules { get; }

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

        public override ValidationSummary Validate(T objectToValidate) => new ValidationSummary(ProcessValidations(objectToValidate));
    }
}
