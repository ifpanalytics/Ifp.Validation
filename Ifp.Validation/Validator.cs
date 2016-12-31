using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifp.Validation
{
    /// <summary>
    /// Base class for validators. Those validators usually don't perform validations on there own 
    /// but delegate the validation to one or more <see cref="IValidationRule{T}"/> objects. These <see cref="IValidationRule{T}"/> objects perform a single
    /// isolated validation and the <see cref="Validator{T}"/> collects the single <see cref="ValidationOutcome"/> and wrap them in a <see cref="ValidationSummary"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public abstract class Validator<T>
    {
        /// <summary>
        /// Validate a object and return the <see cref="ValidationOutcome"/> objects wrapped in a <see cref="ValidationOutcome"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>The <see cref="ValidationSummary"/> that wraps a several <see cref="ValidationOutcome"/>.#
        /// The <see cref="ValidationSummary"/> can be used as a model that can be presented to the user.
        /// </returns>
        public abstract ValidationSummary Validate(T objectToValidate);
    }

    /// <summary>
    /// A validator that takes a set of objects that implement the <see cref="IValidationRule{T}"/> interface.
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

        /// <summary>
        /// The rules passed in the constructor.
        /// </summary>
        protected IValidationRule<T>[] Rules { get; }

        /// <summary>
        /// Processes the <see cref="Rules"/> one after the other and stops if a rule returns a 
        /// <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/> and 
        /// <see cref="IValidationRule{T}.CausesValidationProcessToStop"/> both set to <code>true</code>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="ValidationOutcome"/>.</returns>
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
