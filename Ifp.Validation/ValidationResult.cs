﻿using System;
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

    public class ValidationSummary
    {
        readonly ValidationOutcome[] _ValidationOutcomes;

        public ValidationSummary() : this(new ValidationOutcome[] { }) { }

        public ValidationSummary(IEnumerable<ValidationSummary> validationSummaries) : this(validationSummaries.ToArray()) { }

        public ValidationSummary(params ValidationSummary[] validationSummaries) : this(validationSummaries.SelectMany(s => s.ValidationOutcomes)) { }

        public ValidationSummary(IEnumerable<ValidationOutcome> validationOutcomes) : this(validationOutcomes.ToArray()) { }

        public ValidationSummary(params ValidationOutcome[] validationOutcomes)
        {
            _ValidationOutcomes = validationOutcomes.OrderByDescending(v => v.Severity).ToArray();
            Severity = _ValidationOutcomes.Max(vr => vr.Severity) ?? ValidationSeverity.Success;
        }

        public IEnumerable<ValidationOutcomeWithMessage> ValidationFailures => _ValidationOutcomes.OfType<ValidationOutcomeWithMessage>().Where(vr => vr.Severity.IsAnError);

        public IEnumerable<ValidationOutcome> ValidationOutcomes => _ValidationOutcomes;

        public ValidationSeverity Severity { get; }
    }

    public enum FailureSeverity
    {
        Information,
        Warning,
        Error,
    }

    public class ValidationSummaryBuilder
    {

        public ValidationSummaryBuilder()
        {
            ValidationOutcomes = new List<ValidationOutcome>();
        }

        protected List<ValidationOutcome> ValidationOutcomes { get; }

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

        public ValidationSummary ToSummary() => new ValidationSummary(ValidationOutcomes);
    }

    public static class ValidationSummaryBuilderExtensions
    {
        public static ValidationSummary ToSummary(this ValidationOutcome validationOutcome) => new ValidationSummary(validationOutcome);
        public static ValidationSummaryBuilder ToSummaryBuilder(this ValidationOutcome validationOutcome)
        {
            var res = new ValidationSummaryBuilder();
            res.Append(validationOutcome);
            return res;
        }
        public static ValidationOutcome ToFailure(this string message, FailureSeverity severity) => ValidationOutcome.Failure(severity, message);
    }
}