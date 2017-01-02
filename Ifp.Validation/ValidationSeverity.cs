using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifp.Validation
{
    /// <summary>
    /// <see cref="ValidationSeverity"/> is the base class for the description of the <see cref="ValidationOutcome.Severity"/> of a <see cref="ValidationOutcome"/>.
    /// The severity describes how fatal the result of a validation is. There are four predefined severity levels, but you can also create your own by inheriting from this class.
    /// The predefined severity levels are
    /// <list type="definition">
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Success"/>
    ///         </term>
    ///         <description>
    ///             The validation was successful (e.g. A required field was filled).
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Information"/>
    ///         </term>
    ///         <description>
    ///             The validation was successful but the user should be informed about problems that were found during the validation (e.g. 
    ///             The birth date you entered indicates that you are under age. You will not be able to use our payed services.)  
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Warning"/>    
    ///         </term>
    ///         <description>
    ///             The validation found a violation that might be a problem. The user should decide whether he wants to continue or cancel the process (e.g.
    ///             The item you purchased are overweight. Additional shipping charges apply.)
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Error"/>
    ///         </term>
    ///         <description>
    ///             The validation failed. The error is reported to the user and the process is canceled.
    ///             (e.g. The email address you entered is invalid.)
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public abstract class ValidationSeverity : IComparable<ValidationSeverity>
    {
        static readonly SuccessSeverity _Success = new SuccessSeverity();
        static readonly InformationSeverity _Information = new InformationSeverity();
        static readonly WarningSeverity _Warning = new WarningSeverity();
        static readonly ErrorSeverity _Error = new ErrorSeverity();

        /// <summary>
        /// If <c>true</c> the process will be canceled in any case.
        /// </summary>
        public abstract bool CausesCancel { get; }

        /// <summary>
        /// If <c>true</c> the process can be canceled by the user.
        /// </summary>
        public abstract bool AllowsCancel { get; }

        /// <summary>
        /// Returns a number to allow comparison between <see cref="ValidationSeverity">validation severities</see>. This number is used by the
        /// <see cref="ValidationSummary"/> to calculate the <see cref="ValidationSummary.Severity"/> by looking for the highest severity of all <see cref="ValidationOutcome"/>.
        /// </summary>
        protected abstract int SeverityAsNumber { get; }

        /// <summary>
        /// If <c>true</c> the severity will be treated as an error and will be presented to the user.
        /// This is true for all severities except <see cref="ValidationSeverity.Success"/>.
        /// </summary>
        public virtual bool IsAnError => true;

        /// <summary>
        /// One of the default severities. Returns a instance of <see cref="SuccessSeverity"/>.
        /// </summary>
        public static SuccessSeverity Success => _Success;

        /// <summary>
        /// One of the default severities. Returns a instance of <see cref="InformationSeverity"/>.
        /// </summary>
        public static InformationSeverity Information => _Information;

        /// <summary>
        /// One of the default severities. Returns a instance of <see cref="WarningSeverity"/>.
        /// </summary>
        public static WarningSeverity Warning => _Warning;

        /// <summary>
        /// One of the default severities. Returns a instance of <see cref="ErrorSeverity"/>.
        /// </summary>
        public static ErrorSeverity Error => _Error;

        /// <summary>
        /// A default severity. The <see cref="SuccessSeverity"/> indicates that an object conforms to a validation rule.
        /// </summary>
        public sealed class SuccessSeverity : ValidationSeverity
        {
            /// <summary>
            /// Returns <c>false</c>. Should be ignored because <see cref="IsAnError"/> is <c>false</c>.
            /// </summary>
            public override bool AllowsCancel => false;

            /// <summary>
            /// Returns <c>false</c>. Should be ignored because <see cref="IsAnError"/> is <c>false</c>.
            /// </summary>
            public override bool CausesCancel => false;

            /// <summary>
            /// Returns <c>false</c> indicating a validation success.
            /// </summary>
            public override bool IsAnError => false;

            /// <summary>
            /// Returns <c>0</c>. Lowest severity.
            /// </summary>
            protected override int SeverityAsNumber => 0;
        }

        /// <summary>
        /// A default severity. The <see cref="InformationSeverity"/> indicates that an object conforms to a validation rule, but the user should be 
        /// informed about something.
        /// </summary>
        public class InformationSeverity : ValidationSeverity
        {
            /// <summary>
            /// Returns <c>false</c>. The user should only be informed. To allow cancellation use <see cref="WarningSeverity"/> instead.
            /// </summary>
            public override bool AllowsCancel => false;
            
            /// <summary>
            /// Returns <c>false</c>. Information allows continuation of the process.
            /// </summary>
            public override bool CausesCancel => false;
            
            /// <summary>
            /// Returns <c>10</c>. 
            /// </summary>
            protected override int SeverityAsNumber => 10;
        }

        /// <summary>
        /// A default severity. The <see cref="WarningSeverity"/> indicates that an object violates a validation rule, but the user should #
        /// decide whether the process should be continued or canceled.
        /// </summary>
        public class WarningSeverity : ValidationSeverity
        {

            /// <summary>
            /// Returns <c>true</c> to enable cancellation.
            /// </summary>
            public override bool AllowsCancel => true;
            
            /// <summary>
            /// Returns <c>false</c> to allow continuation of the process.
            /// </summary>
            public override bool CausesCancel => false;

            /// <summary>
            /// Returns <c>20</c>.
            /// </summary>
            protected override int SeverityAsNumber => 20;
        }

        /// <summary>
        /// A default severity. The <see cref="ErrorSeverity"/> indicates that an object violates a validation rule and the
        /// process should be canceled.
        /// </summary>
        public class ErrorSeverity : ValidationSeverity
        {
            /// <summary>
            /// Returns <c>true</c>.
            /// </summary>
            public override bool AllowsCancel => true;

            /// <summary>
            /// Returns <c>true</c>. This disallows the continuation of the process.
            /// </summary>
            public override bool CausesCancel => true;

            /// <summary>
            /// Returns <c>30</c>.
            /// </summary>
            protected override int SeverityAsNumber => 30;
        }

        /// <summary>
        /// Implements <see cref="IComparable{T}"/>. Compares two <see cref="ValidationSeverity"/> objects by delegating to <see cref="SeverityAsNumber"/>.
        /// </summary>
        /// <param name="other">The other <see cref="ValidationSeverity"/> to compare to.</param>
        /// <returns>Returns a number indicating smaller, greater or equals as described by <see cref="IComparable{T}"/>.</returns>
        public virtual int CompareTo(ValidationSeverity other) => this.SeverityAsNumber.CompareTo(other.SeverityAsNumber);
    }
}
