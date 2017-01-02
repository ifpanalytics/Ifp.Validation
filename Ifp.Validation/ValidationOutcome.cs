using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{

    /// <summary>
    /// Abstract base class for Validation outcomes returned by <see cref="IValidationRule{T}.ValidateObject(T)"/>.
    /// </summary>
    public abstract class ValidationOutcome
    {
        readonly static ValidationOutcomeSuccess _ValidationOutcomeSuccess = new ValidationOutcomeSuccess();

        /// <summary>
        /// The static property to indicate a successful passing of a validation rule. Use this value to return 
        /// success from <see cref="ValidationRule{T}.ValidateObject(T)"/>.
        /// </summary>
        public static ValidationOutcome Success => _ValidationOutcomeSuccess;

        /// <summary>
        /// A constant <see cref="ValidationSeverity"/> for the concrete <see cref="ValidationOutcome"/>. 
        /// </summary>
        public abstract ValidationSeverity Severity { get; }

        static Func<string, ValidationOutcome> FailureSeverityToValidationOutcome(FailureSeverity severity)
        {
            switch (severity)
            {
                case FailureSeverity.Information:
                    return message => new ValidationOutcomeInformation(message);
                case FailureSeverity.Warning:
                    return message => new ValidationOutcomeWarning(message);
                case FailureSeverity.Error:
                    return message => new ValidationOutcomeError(message);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Static factory method for predefined <see cref="ValidationOutcome"/> types. The predefined outcomes are
        /// described by the <see cref="FailureSeverity"/> enum.
        /// </summary>
        /// <param name="severity">One of the predefined <see cref="FailureSeverity"/> values.</param>
        /// <param name="message">A message, describing the failure.</param>
        /// <returns>Returns a <see cref="ValidationOutcome"/> that corresponds to the <paramref name="severity"/>.</returns>
        public static ValidationOutcome Failure(FailureSeverity severity, string message)
        {
            var validationOutcomeFunc = FailureSeverityToValidationOutcome(severity);
            var validationOutcome = validationOutcomeFunc(message);
            return validationOutcome;
        }
    }

    internal sealed class ValidationOutcomeSuccess : ValidationOutcome
    {
        public override ValidationSeverity Severity => ValidationSeverity.Success;
    }

    /// <summary>
    /// A base class for <see cref="ValidationOutcome"/> types with an error message.
    /// </summary>
    public abstract class ValidationOutcomeWithMessage : ValidationOutcome
    {
        /// <summary>
        /// Constructor taking the <see cref="ErrorMessage"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ValidationOutcomeWithMessage(string message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// The <see cref="ErrorMessage"/> given by the constructor <see cref="ValidationOutcomeWithMessage.ValidationOutcomeWithMessage(string)"/>.
        /// </summary>
        public string ErrorMessage { get; }
    }

    internal class ValidationOutcomeInformation : ValidationOutcomeWithMessage
    {
        public ValidationOutcomeInformation(string message) : base(message) { }
        public override ValidationSeverity Severity => ValidationSeverity.Information;
    }

    internal class ValidationOutcomeWarning : ValidationOutcomeWithMessage
    {
        public ValidationOutcomeWarning(string message) : base(message) { }
        public override ValidationSeverity Severity => ValidationSeverity.Warning;
    }

    internal class ValidationOutcomeError : ValidationOutcomeWithMessage
    {
        public ValidationOutcomeError(string message) : base(message) { }
        public override ValidationSeverity Severity => ValidationSeverity.Error;
    }
}
