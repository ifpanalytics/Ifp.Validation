using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifp.Validation
{
    public abstract class ValidationSeverity : IComparable<ValidationSeverity>
    {
        static readonly SuccessSeverity _Success = new SuccessSeverity();
        static readonly InformationSeverity _Information = new InformationSeverity();
        static readonly WarningSeverity _Warning = new WarningSeverity();
        static readonly ErrorSeverity _Error = new ErrorSeverity();

        public abstract bool CausesCancel { get; }
        public abstract bool AllowsCancel { get; }
        public virtual bool IsAnError => true;
        public static SuccessSeverity Success => _Success;
        public static InformationSeverity Information => _Information;
        public static WarningSeverity Warning => _Warning;
        public static ErrorSeverity Error => _Error;

        public sealed class SuccessSeverity : ValidationSeverity
        {
            public override bool AllowsCancel => false;
            public override bool CausesCancel => false;
            public override bool IsAnError => false;
            protected override int SeverityAsNumber => 0;
        }
        public class InformationSeverity : ValidationSeverity
        {
            public override bool AllowsCancel => false;
            public override bool CausesCancel => false;
            protected override int SeverityAsNumber => 10;
        }
        public class WarningSeverity : ValidationSeverity
        {
            public override bool AllowsCancel => true;
            public override bool CausesCancel => false;
            protected override int SeverityAsNumber => 20;
        }
        public class ErrorSeverity : ValidationSeverity
        {
            public override bool AllowsCancel => true;
            public override bool CausesCancel => true;
            protected override int SeverityAsNumber => 30;
        }

        protected abstract int SeverityAsNumber { get; }

        public virtual int CompareTo(ValidationSeverity other) => this.SeverityAsNumber.CompareTo(other.SeverityAsNumber);
    }

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


    /// <summary>
    /// enumeration of predefined failure severities.
    /// </summary>
    public enum FailureSeverity
    {
        /// <summary>
        /// Failure severity 'information' indicates that there is a minor rule violation. This violation might be shown to the user but should never prevent the user from proceeding. 
        /// </summary>
        Information,

        /// <summary>
        /// Failure severity 'warning' indicates a rule violation that the user must confirm. An user should be able to proceed after he confirmed that he read the warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Failure severity 'error' indicates a rule violation that the user must confirm. An user should not be able to proceed.
        /// </summary>
        Error,
    }
}
