using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ifp.Validation
{

    /// <summary>
    /// Base class for validators. Those validators usually don't perform validations on there own 
    /// but delegate the validation to one or more <see cref="IAsyncValidationRule{T}"/> objects. These <see cref="IAsyncValidationRule{T}"/> objects perform a single
    /// isolated validation and the <see cref="AsyncValidator{T}"/> collects the single <see cref="ValidationOutcome"/> and wrap them in a <see cref="ValidationSummary"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public abstract class AsyncValidator<T> : IAsyncValidator<T>
    {
        /// <summary>
        /// Validate an object and return the <see cref="ValidationOutcome"/> objects wrapped in a <see cref="ValidationOutcome"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>The <see cref="ValidationSummary"/> that wraps a several <see cref="ValidationOutcome"/>.
        /// The <see cref="ValidationSummary"/> can be used as a model that can be presented to the user.
        /// </returns>
        public abstract Task<ValidationSummary> ValidateAsync(T objectToValidate);
    }

    /// <summary>
    /// A validator that takes a set of objects that implement the <see cref="IAsyncValidationRule{T}"/> interface.
    /// Use the <see cref="AsyncValidationRule{T}"/> class or the <see cref="AsyncValidationRuleDelegate{T}"/> class as base for the implementation of rules.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate</typeparam>
    public class AsyncRuleBasedValidator<T> : AsyncValidator<T>
    {
        /// <summary>
        /// Constructs a validator that applies the given validation rules to an object in the given order.
        /// </summary>
        /// <param name="rules">The <see cref="IAsyncValidationRule{T}"/>s to apply to an object.</param>
        public AsyncRuleBasedValidator(IEnumerable<IAsyncValidationRule<T>> rules) : this(rules.ToArray()) { }

        /// <summary>
        /// Constructs a validator that applies the given validation rules to an object in the given order.
        /// </summary>
        /// <param name="rules">The <see cref="IAsyncValidationRule{T}"/>s to apply to an object.</param>
        public AsyncRuleBasedValidator(params IAsyncValidationRule<T>[] rules)
        {
            Rules = rules;
        }

        /// <summary>
        /// The rules passed in the constructor.
        /// </summary>
        protected IAsyncValidationRule<T>[] Rules { get; }

        /// <summary>
        /// Processes the <see cref="Rules"/> one after the other and stops if a rule returns a 
        /// <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/> and 
        /// <see cref="IAsyncValidationRule{T}.CausesValidationProcessToStop"/> both set to <c>true</c>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="ValidationOutcome"/>.</returns>
        protected virtual async Task<IEnumerable<ValidationOutcome>> ProcessValidationsAsync(T objectToValidate)
        {
            var results = new List<ValidationOutcome>(Rules.Length);
            foreach (var rule in Rules)
            {
                var ruleResult = await rule.ValidateObjectAsync(objectToValidate);
                results.Add(ruleResult);
                if (ruleResult.Severity.IsAnError && rule.CausesValidationProcessToStop)
                {
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// Validates an object by applying the rules given in the constructor <see cref="AsyncRuleBasedValidator{T}"/> to the object in the order specified in the constructor./> 
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationSummary"/> that contains the <see cref="ValidationOutcome"/> of every applied <see cref="IAsyncValidationRule{T}"/>.</returns>
        public override async Task<ValidationSummary> ValidateAsync(T objectToValidate) 
            => new ValidationSummary(await ProcessValidationsAsync(objectToValidate));
    }

    /// <summary>
    /// An <see cref="IAsyncValidator{T}"/> which combines the <see cref="ValidationSummary"/> of other <see cref="IAsyncValidator{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate</typeparam>
    public class AsyncValidatorCombiner<T> : AsyncValidator<T>, IAsyncValidator<T>
    {
        /// <summary>
        /// Constructs an <see cref="IAsyncValidator{T}"/> that combines the <see cref="ValidationSummary"/> of the <paramref name="validators"/> in the given order.
        /// </summary>
        /// <param name="validators">The <see cref="IAsyncValidator{T}"/>s to combine.</param>
        public AsyncValidatorCombiner(IEnumerable<IAsyncValidator<T>> validators) : this(validators.ToArray())
        {

        }

        /// <summary>
        /// Constructs an <see cref="IAsyncValidator{T}"/> that combines the <see cref="ValidationSummary"/> of the <paramref name="validators"/> in the given order.
        /// </summary>
        /// <param name="validators">The <see cref="IValidator{T}"/>s to combine.</param>
        public AsyncValidatorCombiner(params IAsyncValidator<T>[] validators)
        {
            Validators = validators;
        }

        /// <summary>
        /// The collection of validators that get combined.
        /// </summary>
        protected IEnumerable<IAsyncValidator<T>> Validators { get; }

        /// <summary>
        /// Validates <paramref name="objectToValidate"/> by calling <see cref="IAsyncValidator{T}.ValidateAsync(T)"/> of all <see cref="AsyncValidatorCombiner{T}.Validators"/> and combining
        /// their <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns></returns>
        public override async Task<ValidationSummary> ValidateAsync(T objectToValidate)
        {
            var resultList = new List<ValidationSummary>(Validators.Count());
            foreach (var validator in Validators)
            {
                var summary = await validator.ValidateAsync(objectToValidate);
                resultList.Add(summary);
            }

            return new ValidationSummary(resultList);
        }
    }

    /// <summary>
    /// A validator, that delegates validation to another validator. This can be useful if the objectToValidate needs to be transformed before validation.
    /// </summary>
    /// <typeparam name="T">The type to which the validation is delegated.</typeparam>
    /// <typeparam name="U">The type that this validator can validate.</typeparam>
    public class AsyncDelegateValidator<T, U> : IAsyncValidator<U>
    {
        /// <summary>
        /// Creates a <see cref="AsyncDelegateValidator{T, U}"/> that takes an <see cref="IAsyncValidator{T}"/> and a <paramref name="selector"/> from <typeparamref name="U"/>
        /// to <typeparamref name="T"/>. The <see cref="AsyncDelegateValidator{T, U}"/> implements <see cref="IAsyncValidator{U}"/>.
        /// </summary>
        /// <param name="validator">An existing <see cref="IAsyncValidator{T}"/> for <typeparamref name="T"/>.</param>
        /// <param name="selector">An <paramref name="selector"/> from <typeparamref name="U"/> to <typeparamref name="T"/>.</param>
        public AsyncDelegateValidator(IAsyncValidator<T> validator, Func<U, T> selector)
        {
            Validator = validator;
            Selector = selector;
        }

        /// <summary>
        /// The validator to which the validation is delegated
        /// </summary>
        protected IAsyncValidator<T> Validator { get; }
        
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
        public async Task<ValidationSummary> ValidateAsync(U objectToValidate)
        {
            return await Validator.ValidateAsync(Selector(objectToValidate));
        }
    }
}
