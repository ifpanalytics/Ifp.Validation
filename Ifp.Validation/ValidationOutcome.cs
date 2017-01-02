using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    public abstract class ValidationOutcome
    {
        readonly static ValidationOutcomeSuccess _ValidationOutcomeSuccess = new ValidationOutcomeSuccess();
        public static ValidationOutcome Success => _ValidationOutcomeSuccess;
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
    public abstract class ValidationOutcomeWithMessage : ValidationOutcome
    {
        public ValidationOutcomeWithMessage(string message)
        {
            ErrorMessage = message;
        }
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
