namespace Ifp.Validation
{
    /// <summary>
    /// A validator takes an object of type <typeparamref name="T"/> and performs a validation. The result of the validation is represented by a collection 
    /// of <see cref="ValidationOutcome"/> wrapped in a <see cref="ValidationSummary"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validate an object and return the <see cref="ValidationOutcome"/> objects wrapped in a <see cref="ValidationOutcome"/>.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>The <see cref="ValidationSummary"/> that wraps a several <see cref="ValidationOutcome"/>.
        /// The <see cref="ValidationSummary"/> can be used as a model that can be presented to the user.
        /// </returns>
        ValidationSummary Validate(T objectToValidate);
    }
}
