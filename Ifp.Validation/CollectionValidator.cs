using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    /// <summary>
    /// Takes one or more validation rules for <typeparamref name="T"/> and validates <see cref="IEnumerable{T}"/> against that rules.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public class CollectionValidator<T>
    {
        /// <summary>
        /// Constructs a <see cref="CollectionValidator{T}"/> and takes an enumeration of <see cref="IValidationRule{T}"/>.
        /// </summary>
        /// <param name="validationRules">The <see cref="IEnumerable{T}"/> of <see cref="IValidationRule{T}"/> objects.</param>
        public CollectionValidator(IEnumerable<IValidationRule<T>> validationRules) : this(validationRules.ToArray())
        {

        }

        /// <summary>
        /// Constructor for the <see cref="CollectionValidator{T}"/> that takes a variable number of <see cref="IValidationRule{T}"/> objects. 
        /// </summary>
        /// <param name="validationRules">A variable number of <see cref="IValidationRule{T}"/> objects.</param>
        public CollectionValidator(params IValidationRule<T>[] validationRules)
        {
            Validator = new RuleBasedValidator<T>(validationRules);
        }

        /// <summary>
        /// A <see cref="Validator{T}"/> that is applicable to an instance of <typeparamref name="T"/>. This validator is applied to all objects in the <see cref="IEnumerable{T}"/> in the <see cref="CollectionValidator{T}.ValidateCollection(IEnumerable{T})"/> method.
        /// </summary>
        /// <param name="validator"></param>
        public CollectionValidator(Validator<T> validator)
        {
            Validator = validator;
        }

        /// <summary>
        /// The <see cref="Validator{T}"/> that represents the rules given in the constructor.
        /// </summary>
        protected Validator<T> Validator { get; }

        /// <summary>
        /// Validates a <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> objects.
        /// </summary>
        /// <param name="collectionToValidate">The <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> objects.</param>
        /// <returns>A <see cref="ValidationSummary"/> representing the summary of the results of the application of all <see cref="IValidationRule{T}">Validation rules</see> to all objects in <paramref name="collectionToValidate"/>.</returns>
        public ValidationSummary ValidateCollection(IEnumerable<T> collectionToValidate) =>
            new ValidationSummary(ValidateCollectionLoop(collectionToValidate));

        /// <summary>
        /// Apply all rules, represented by <see cref="CollectionValidator{T}.Validator"/> to the <paramref name="collectionToValidate"/>. Override
        /// this method to change the order of application of the rules.
        /// </summary>
        /// <param name="collectionToValidate">The <see cref="IEnumerable{T}"/> of objects to validate.</param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> of <see cref="ValidationSummary"/> objects, where each <see cref="ValidationSummary"/> represents
        /// the result of the validation of a object of the <paramref name="collectionToValidate"/>.
        /// </returns>
        protected virtual IEnumerable<ValidationSummary> ValidateCollectionLoop(IEnumerable<T> collectionToValidate)
        {
            foreach (var objectToValidate in collectionToValidate)
            {
                yield return Validator.Validate(objectToValidate);
            }
        }
    }
}
