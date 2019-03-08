using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    /// <summary>
    /// Base class for validators. Those validators usually don't perform validations on there own 
    /// but delegate the validation to one or more <see cref="IValidationRule{T}"/> objects. These <see cref="IValidationRule{T}"/> objects perform a single
    /// isolated validation and the <see cref="Validator{T}"/> collects the single <see cref="ValidationOutcome"/> and wrap them in a <see cref="ValidationSummary"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public abstract class Validator<T> : IValidator<T>
    {
        /// <summary>
        /// Validate an object and return the <see cref="ValidationOutcome"/> objects wrapped in a <see cref="ValidationOutcome"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>The <see cref="ValidationSummary"/> that wraps a several <see cref="ValidationOutcome"/>.
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
        /// <see cref="IValidationRule{T}.CausesValidationProcessToStop"/> both set to <c>true</c>.
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
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Validates an object by applying the rules given in the constructor <see cref="RuleBasedValidator{T}"/> to the object in the order specified in the constructor./> 
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationSummary"/> that contains the <see cref="ValidationOutcome"/> of every applied <see cref="IValidationRule{T}"/>.</returns>
        public override ValidationSummary Validate(T objectToValidate) => new ValidationSummary(ProcessValidations(objectToValidate));
    }

    /// <summary>
    /// An <see cref="IValidator{T}"/> which combines the <see cref="ValidationSummary"/> of other <see cref="IValidator{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate</typeparam>
    public class ValidatorCombiner<T> : Validator<T>, IValidator<T>
    {
        /// <summary>
        /// Constructs an <see cref="IValidator{T}"/> that combines the <see cref="ValidationSummary"/> of the <paramref name="validators"/> in the given order.
        /// </summary>
        /// <param name="validators">The <see cref="IValidator{T}"/>s to combine.</param>
        public ValidatorCombiner(IEnumerable<IValidator<T>> validators) : this(validators.ToArray())
        {

        }

        /// <summary>
        /// Constructs an <see cref="IValidator{T}"/> that combines the <see cref="ValidationSummary"/> of the <paramref name="validators"/> in the given order.
        /// </summary>
        /// <param name="validators">The <see cref="IValidator{T}"/>s to combine.</param>
        public ValidatorCombiner(params IValidator<T>[] validators)
        {
            Validators = validators;
        }

        /// <summary>
        /// The collection of validators that get combined.
        /// </summary>
        protected IValidator<T>[] Validators { get; }

        /// <summary>
        /// Validates <paramref name="objectToValidate"/> by calling <see cref="IValidator{T}.Validate(T)"/> of all <see cref="ValidatorCombiner{T}.Validators"/> and combining
        /// their <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns></returns>
        public override ValidationSummary Validate(T objectToValidate)
        {
            var validationSummaries = Validators.Select(v => v.Validate(objectToValidate));
            return new ValidationSummary(validationSummaries);

        }
    }
}
