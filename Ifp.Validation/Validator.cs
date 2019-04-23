using System;
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
        protected IEnumerable<IValidator<T>> Validators { get; }

        /// <summary>
        /// Validates <paramref name="objectToValidate"/> by calling <see cref="IValidator{T}.Validate(T)"/> of all <see cref="ValidatorCombiner{T}.Validators"/> and combining
        /// their <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns></returns>
        public override ValidationSummary Validate(T objectToValidate)
            => new ValidationSummary(Validators.Select(v => v.Validate(objectToValidate)));
    }

    /// <summary>
    /// Validates an object of type <typeparamref name="T"/> by applying <see cref="IValidationRule{T}"/>s to a
    /// collection that is accessible from <typeparamref name="T"/>. The rules that get applied are of the underlying type
    /// of the collection accessed.
    /// </summary>
    /// <typeparam name="T">The type to validate</typeparam>
    /// <typeparam name="U">The underlying type of the collection that is accessible from <typeparamref name="T"/></typeparam>
    public class SubCollectionValidator<T, U> : Validator<T>, IValidator<T>
    {
        /// <summary>
        /// Constructs a <see cref="SubCollectionValidator{T, U}"/> by taking a <paramref name="selector"/> the returns a 
        /// collection from an object of type <typeparamref name="T"/> and some <see cref="IValidationRule{T}"/>s for the underlying type
        /// of the collection.
        /// </summary>
        /// <param name="selector">A <paramref name="selector"/> that takes an object of type <typeparamref name="T"/> and returns a collection of 
        /// type <typeparamref name="U"/>.</param>
        /// <param name="validators">The <see cref="IValidator{T}"/>s to apply to <typeparamref name="U"/>.</param>
        public SubCollectionValidator(Func<T, IEnumerable<U>> selector, params IValidator<U>[] validators)
        {
            Selector = selector;
            Validator = new ValidatorCombiner<U>(validators);
        }

        /// <summary>
        /// A <see cref="SubCollectionValidator{T, U}.Selector"/> function that takes an object <typeparamref name="T"/> and returns a collection <typeparamref name="U"/>.
        /// </summary>
        protected Func<T, IEnumerable<U>> Selector { get; }

        /// <summary>
        /// The validator that is applied to each element of the collection returned by <see cref="SubCollectionValidator{T, U}.Selector"/>.
        /// </summary>
        protected IValidator<U> Validator { get; }

        /// <summary>
        /// The validation that applies the <see cref="SubCollectionValidator{T, U}.Validator"/> to each element returned by <see cref="SubCollectionValidator{T, U}.Selector"/>.
        /// </summary>
        /// <param name="objectToValidate"></param>
        /// <returns></returns>
        public override ValidationSummary Validate(T objectToValidate)
            => new ValidationSummary(Selector(objectToValidate).Select(Validator.Validate));
    }

    /// <summary>
    /// A validator, that delegates validation to another validator. This can be useful if the objectToValidate needs to be transformed before validation.
    /// </summary>
    /// <typeparam name="T">The type to which the validation is delegated.</typeparam>
    /// <typeparam name="U">The type that this validator can validate.</typeparam>
    public class DelegateValidator<T, U> : IValidator<U>
    {
        /// <summary>
        /// Creates a <see cref="DelegateValidator{T, U}"/> that takes an <see cref="IValidator{T}"/> and a <paramref name="selector"/> from <typeparamref name="U"/>
        /// to <typeparamref name="T"/>. The <see cref="DelegateValidator{T, U}"/> implements <see cref="IValidator{U}"/>.
        /// </summary>
        /// <param name="validator">An existing <see cref="IValidator{T}"/> for <typeparamref name="T"/>.</param>
        /// <param name="selector">An <paramref name="selector"/> from <typeparamref name="U"/> to <typeparamref name="T"/>.</param>
        public DelegateValidator(IValidator<T> validator, Func<U, T> selector)
        {
            Validator = validator;
            Selector = selector;
        }

        /// <summary>
        /// The validator to which the validation is delegated
        /// </summary>
        protected IValidator<T> Validator { get; }
        
        /// <summary>
        /// The <see cref="Selector"/> function that transforms objectToValidate from <typeparamref name="U"/> to <typeparamref name="T"/>.
        /// </summary>
        protected Func<U, T> Selector { get; }
        
        /// <summary>
        /// Validates an <paramref name="objectToValidate"/> of type <typeparamref name="U"/> by transforming it to <typeparamref name="T"/> and delegating 
        /// validation to <see cref="Validator"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationSummary"/> representing the result of a validation.</returns>
        public ValidationSummary Validate(U objectToValidate)
        {
            return Validator.Validate(Selector(objectToValidate));
        }
    }
}
