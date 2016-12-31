using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    /// <summary>
    /// A container for one or more <see cref="ValidationOutcome"/> objects.
    /// The <see cref="ValidationSummary"/> is usually produced by a <see cref="RuleBasedValidator{T}"/>.
    /// </summary>
    /// <seealso cref="ValidationSummaryBuilder"/>
    /// <seealso cref="ValidationSummaryBuilderExtensions.ToSummary"/>
    public class ValidationSummary
    {
        readonly ValidationOutcome[] _ValidationOutcomes;

        /// <summary>
        /// Constructs an empty <see cref="ValidationSummary"/>.
        /// </summary>
        /// <seealso cref="ValidationSummaryBuilder"/>
        /// <seealso cref="ValidationSummaryBuilderExtensions.ToSummary"/>
        public ValidationSummary() : this(new ValidationOutcome[] { }) { }

        /// <summary>
        /// Constructs a new <see cref="ValidationSummary"/> out of other <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="validationSummaries">Other <see cref="ValidationSummary"/> that get combined to a new <see cref="ValidationSummary"/></param>
        /// <seealso cref="ValidationSummaryBuilder"/>
        /// <seealso cref="ValidationSummaryBuilderExtensions.ToSummary"/>
        public ValidationSummary(IEnumerable<ValidationSummary> validationSummaries) : this(validationSummaries.ToArray()) { }

        /// <summary>
        /// Constructs a new <see cref="ValidationSummary"/> out of other <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="validationSummaries">Other <see cref="ValidationSummary"/> that get combined to a new <see cref="ValidationSummary"/></param>
        /// <seealso cref="ValidationSummaryBuilder"/>
        /// <seealso cref="ValidationSummaryBuilderExtensions.ToSummary"/>
        public ValidationSummary(params ValidationSummary[] validationSummaries) : this(validationSummaries.SelectMany(s => s.ValidationOutcomes)) { }

        /// <summary>
        /// Constructs a new <see cref="ValidationSummary"/> out of other <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="validationOutcomes">A set of <see cref="ValidationOutcome"/> objects that are summarized.</param>
        /// <seealso cref="ValidationSummaryBuilder"/>
        /// <seealso cref="ValidationSummaryBuilderExtensions.ToSummary"/>
        public ValidationSummary(IEnumerable<ValidationOutcome> validationOutcomes) : this(validationOutcomes.ToArray()) { }

        /// <summary>
        /// Constructs a new <see cref="ValidationSummary"/> out of other <see cref="ValidationSummary"/>.
        /// </summary>
        /// <param name="validationOutcomes">A set of <see cref="ValidationOutcome"/> objects that are summarized.</param>
        /// <seealso cref="ValidationSummaryBuilder"/>
        /// <seealso cref="ValidationSummaryBuilderExtensions.ToSummary"/>
        public ValidationSummary(params ValidationOutcome[] validationOutcomes)
        {
            _ValidationOutcomes = validationOutcomes.OrderByDescending(v => v.Severity).ToArray();
            Severity = _ValidationOutcomes.Max(vr => vr.Severity) ?? ValidationSeverity.Success;
        }

        /// <summary>
        /// Returns all <see cref="ValidationOutcome"/> objects that are errors (ValidationOutcomes with <see cref="ValidationSeverity.IsAnError"/>) 
        /// </summary>
        public IEnumerable<ValidationOutcomeWithMessage> ValidationFailures => _ValidationOutcomes.OfType<ValidationOutcomeWithMessage>().Where(vr => vr.Severity.IsAnError);

        /// <summary>
        /// Returns all <see cref="ValidationOutcome"/> objects.
        /// </summary>
        public IEnumerable<ValidationOutcome> ValidationOutcomes => _ValidationOutcomes;

        /// <summary>
        /// Return the most severe of all <see cref="ValidationOutcome.Severity"/>. The severity is ordered by <see cref="ValidationSeverity.SeverityAsNumber"/>.
        /// </summary>
        public ValidationSeverity Severity { get; }
    }

    /// <summary>
    /// A helper class that allows to build a validation summary step by step.
    /// The <see cref="ValidationSummary"/> can not be changed after construction.
    /// The ValidationSummaryBuilder can be used to collect several <see cref="ValidationOutcome"/> or <see cref="ValidationSummary"/> objects and append them to a new <see cref="ValidationSummary"/>.
    /// </summary>
    public class ValidationSummaryBuilder
    {
        /// <summary>
        /// Constructs the ValidationSummaryBuilder.
        /// </summary>
        public ValidationSummaryBuilder()
        {
            ValidationOutcomes = new List<ValidationOutcome>();
        }

        /// <summary>
        /// The list of <see cref="ValidationOutcome"/>.
        /// </summary>
        protected List<ValidationOutcome> ValidationOutcomes { get; }

        /// <summary>
        /// Appends a <see cref="ValidationOutcome"/> to the builder.
        /// </summary>
        /// <param name="ValidationOutcome">The <see cref="ValidationOutcome"/> to append.</param>
        /// <returns>The builder itself.</returns>
        public ValidationSummaryBuilder Append(ValidationOutcome ValidationOutcome)
        {
            ValidationOutcomes.Add(ValidationOutcome);
            return this;
        }

        /// <summary>
        /// Appends a <see cref="ValidationOutcome"/>, that will be build from the parameters.
        /// </summary>
        /// <param name="severity">The <see cref="FailureSeverity"/> of the <see cref="ValidationOutcome"/>.</param>
        /// <param name="message">The message to pass to a <see cref="ValidationOutcomeWithMessage"/>.</param>
        /// <returns>The builder itself.</returns>
        public ValidationSummaryBuilder Append(FailureSeverity severity, string message)
        {
            var validationResult = ValidationOutcome.Failure(severity, message);
            this.Append(validationResult);
            return this;
        }

        /// <summary>
        /// Returns the <see cref="ValidationSummary"/> out of the appended <see cref="ValidationOutcome"/> objects. 
        /// </summary>
        /// <returns></returns>
        public ValidationSummary ToSummary() => new ValidationSummary(ValidationOutcomes);
    }
}
