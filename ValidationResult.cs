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
        public virtual bool IsAnError { get { return true; } }
        public static SuccessSeverity Success { get { return _Success; } }
        public static InformationSeverity Information { get { return _Information; } }
        public static WarningSeverity Warning { get { return _Warning; } }
        public static ErrorSeverity Error { get { return _Error; } }

        public sealed class SuccessSeverity : ValidationSeverity
        {
            public override bool AllowsCancel { get { return false; } }
            public override bool CausesCancel { get { return false; } }
            public override bool IsAnError { get { return false; } }
            protected override int SeverityAsNumber { get { return 0; } }
        }
        public class InformationSeverity : ValidationSeverity
        {
            public override bool AllowsCancel { get { return false; } }
            public override bool CausesCancel { get { return false; } }
            protected override int SeverityAsNumber { get { return 10; } }
        }
        public class WarningSeverity : ValidationSeverity
        {
            public override bool AllowsCancel { get { return true; } }
            public override bool CausesCancel { get { return false; } }
            protected override int SeverityAsNumber { get { return 20; } }
        }
        public class ErrorSeverity : ValidationSeverity
        {
            public override bool AllowsCancel { get { return true; } }
            public override bool CausesCancel { get { return true; } }
            protected override int SeverityAsNumber { get { return 30; } }
        }

        protected abstract int SeverityAsNumber { get; }

        public virtual int CompareTo(ValidationSeverity other)
        {
            return this.SeverityAsNumber.CompareTo(other.SeverityAsNumber);
        }
    }

    public abstract class ValidationOutcome
    {
        readonly static ValidationOutcomeSuccess _ValidationOutcomeSuccess = new ValidationOutcomeSuccess();
        public static ValidationOutcome Success { get { return _ValidationOutcomeSuccess; } }
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
        public override ValidationSeverity Severity
        {
            get { return ValidationSeverity.Success; }
        }
    }
    public abstract class ValidationOutcomeWithMessage : ValidationOutcome
    {
        readonly string _Message;
        public ValidationOutcomeWithMessage(string message)
        {
            _Message = message;
        }
        public string ErrorMessage { get { return _Message; } }
    }

    internal class ValidationOutcomeInformation : ValidationOutcomeWithMessage
    {
        public ValidationOutcomeInformation(string message) : base(message) { }
        public override ValidationSeverity Severity
        {
            get { return ValidationSeverity.Information; }
        }
    }

    internal class ValidationOutcomeWarning : ValidationOutcomeWithMessage
    {
        public ValidationOutcomeWarning(string message) : base(message) { }
        public override ValidationSeverity Severity
        {
            get { return ValidationSeverity.Warning; }
        }
    }

    internal class ValidationOutcomeError : ValidationOutcomeWithMessage
    {
        public ValidationOutcomeError(string message) : base(message) { }
        public override ValidationSeverity Severity
        {
            get { return ValidationSeverity.Error; }
        }
    }

    public class ValidationSummary
    {
        readonly ValidationOutcome[] _ValidationOutcomes;

        readonly ValidationSeverity _Severity;

        public ValidationSummary() : this(new ValidationOutcome[] { }) { }

        public ValidationSummary(IEnumerable<ValidationSummary> validationSummaries) : this(validationSummaries.ToArray()) { }

        public ValidationSummary(params ValidationSummary[] validationSummaries) : this(validationSummaries.SelectMany(s => s.ValidationOutcomes)) { }

        public ValidationSummary(IEnumerable<ValidationOutcome> validationOutcomes) : this(validationOutcomes.ToArray()) { }

        public ValidationSummary(params ValidationOutcome[] validationOutcomes)
        {
            _ValidationOutcomes = validationOutcomes.OrderByDescending(v => v.Severity).ToArray();
            _Severity = _ValidationOutcomes.Max(vr => vr.Severity) ?? ValidationSeverity.Success;
        }

        public IEnumerable<ValidationOutcomeWithMessage> ValidationFailures
        {
            get { return _ValidationOutcomes.OfType<ValidationOutcomeWithMessage>().Where(vr => vr.Severity.IsAnError); }
        }

        public IEnumerable<ValidationOutcome> ValidationOutcomes
        {
            get { return _ValidationOutcomes; }
        }

        public ValidationSeverity Severity
        {
            get
            {
                return _Severity;
            }
        }
    }

    public enum FailureSeverity
    {
        Information,
        Warning,
        Error,
    }

    public class ValidationSummaryBuilder
    {
        readonly List<ValidationOutcome> _ValidationOutcomes;

        public ValidationSummaryBuilder()
        {
            _ValidationOutcomes = new List<ValidationOutcome>();
        }

        protected List<ValidationOutcome> ValidationOutcomes
        {
            get { return _ValidationOutcomes; }
        }

        public ValidationSummaryBuilder Append(ValidationOutcome ValidationOutcome)
        {
            ValidationOutcomes.Add(ValidationOutcome);
            return this;
        }


        public ValidationSummaryBuilder Append(FailureSeverity severity, string message)
        {
            var validationResult = ValidationOutcome.Failure(severity, message);
            this.Append(validationResult);
            return this;
        }

        public ValidationSummary ToSummary()
        {
            return new ValidationSummary(ValidationOutcomes);
        }
    }

    public static class ValidationSummaryBuilderExtensions
    {
        public static ValidationSummary ToSummary(this ValidationOutcome validationOutcome)
        {
            return new ValidationSummary(validationOutcome);
        }
        public static ValidationSummaryBuilder ToSummaryBuilder(this ValidationOutcome validationOutcome)
        {
            var res = new ValidationSummaryBuilder();
            res.Append(validationOutcome);
            return res;
        }
        public static ValidationOutcome ToFailure(this string message, FailureSeverity severity)
        {
            return ValidationOutcome.Failure(severity, message);
        }
    }
}
