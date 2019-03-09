namespace Ifp.Validation
{
    /// <summary>
    /// Extension methods for the Ifp.Validation library
    /// </summary>
    public static class ValidationSummaryBuilderExtensions
    {
        /// <summary>
        /// Converts a <see cref="ValidationOutcome"/> to a <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="validationOutcome">The <see cref="ValidationOutcome"/> to wrap into a <see cref="ValidationSummary"/>.</param>
        /// <returns>A <see cref="ValidationSummary"/> that contains the <see cref="ValidationOutcome"/>.</returns>
        public static ValidationSummary ToSummary(this ValidationOutcome validationOutcome) => new ValidationSummary(validationOutcome);

        /// <summary>
        /// Converts a <see cref="ValidationOutcome"/> to a <see cref="ValidationSummaryBuilder"/>.
        /// </summary>
        /// <param name="validationOutcome">A <see cref="ValidationOutcome"/> that is append to a new <see cref="ValidationSummaryBuilder"/>.</param>
        /// <returns>A <see cref="ValidationSummaryBuilder"/> with the <see cref="ValidationOutcome"/> appended.</returns>
        public static ValidationSummaryBuilder ToSummaryBuilder(this ValidationOutcome validationOutcome)
        {
            var res = new ValidationSummaryBuilder();
            res.Append(validationOutcome);
            return res;
        }

        /// <summary>
        /// Converts a <see cref="string"/> to a predefined <see cref="ValidationOutcome"/>.
        /// </summary>
        /// <param name="message">The message to append to a <see cref="ValidationOutcomeWithMessage"/>.</param>
        /// <param name="severity">The <see cref="FailureSeverity"/> that is used to construct one of the predefined <see cref="ValidationOutcome"/>.</param>
        /// <returns>Returns a <see cref="ValidationOutcome"/> that corresponds to the <paramref name="severity"/>.</returns>
        public static ValidationOutcome ToFailure(this string message, FailureSeverity severity) => ValidationOutcome.Failure(severity, message);

        /// <summary>
        /// Converts a <see cref="IValidationRule{T}"/> into an <see cref="IValidator{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to validate.</typeparam>
        /// <param name="rule">The rule that get's converted into an <see cref="IValidator{T}"/>.</param>
        /// <returns>An <see cref="IValidator{T}"/> that calls the <paramref name="rule"/> on <see cref="IValidator{T}.Validate(T)"/>.</returns>
        public static IValidator<T> ToValidator<T>(this IValidationRule<T> rule)
            => new RuleBasedValidator<T>(rule);
    }
}
