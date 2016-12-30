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

    /// <summary>
    /// A validator that takes a set of objects that implement the <see cref="IValidationRule{T}"/> intereface.
    /// Use the <see cref="ValidationRule{T}"/> class or the <see cref="ValidationRuleDelegate{T}"/> class as base for the implementation of rules.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate</typeparam>
    public class RuleBasedValidator<T> : Validator<T>
    {
        /// <summary>
        /// Constructs a validator that applies the given validation rules to an object in the given order.
        /// </summary>
        /// <param name="rules">The <see cref="IValidationRule{T}"/>s to apply to an object.</param>
        public RuleBasedValidator(IEnumerable<IValidationRule<T>> rules) : this(rules.ToArray()) { }

        /// <summary>
        /// Constructs a validator that applies the given validation rules to an object in the given order.
        /// </summary>
        /// <param name="rules">The <see cref="IValidationRule{T}"/>s to apply to an object.</param>
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

        /// <summary>
        /// Validates an object by applying the rules given in the constructor <see cref="RuleBasedValidator{T}"/> to the object in the order specified in the constructor./> 
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationSummary"/> that contains the <see cref="ValidationOutcome"/> of every applied <see cref="IValidationRule{T}"/>.</returns>
        public override ValidationSummary Validate(T objectToValidate) => new ValidationSummary(ProcessValidations(objectToValidate));
    }
}
