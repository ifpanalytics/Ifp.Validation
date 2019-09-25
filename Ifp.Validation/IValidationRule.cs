namespace Ifp.Validation
{
    /// <summary>
    /// Classes that validate other objects should implement this interface.
    /// Classes that implement this interface can be combined to rules sets by the <see cref="RuleBasedValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    /// <remarks>
    /// You should not implement this interface directly. Use <see cref="ValidationRule{T}"/> as base class instead.
    /// </remarks>
    /// <seealso cref="ValidationRule{T}"/>
    /// <seealso cref="ValidationRuleDelegate{T}"/>
    public interface IValidationRule<in T>
    {
        /// <summary>
        /// Checks the <paramref name="objectToValidate"/> and returns a <see cref="ValidationOutcome"/>. 
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationOutcome"/> that represents the result of the validation.</returns>
        ValidationOutcome ValidateObject(T objectToValidate);

        /// <summary>
        /// Returns true if the <see cref="RuleBasedValidator{T}"/> should not proceed validation tests in case the validation returns a <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/>. 
        /// This is useful to prevent further processing of rules in cases where all the following rules will also fail. A typical use case is a check for <c>objectToValidate == null</c>.
        /// </summary>
        bool CausesValidationProcessToStop { get; }
    }
}
