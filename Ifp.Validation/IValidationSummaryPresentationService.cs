using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifp.Validation
{
    /// <summary>
    /// Interface for services that can show a <see cref="ValidationSummary"/> as a dialog to the user. This library does not include an implementation of such a service.
    /// A WPF specific implementation can be found in the <see href="https://github.com/ifpanalytics/Ifp.Validation.WPF">Ifp.Validation.WPF</see> package.
    /// </summary>
    /// <note type="implement">
    /// Implementers of this interface should respect the <see cref="ValidationSummary.Severity"/> property by
    /// <list type="definition">
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Information"/>
    ///         </term>
    ///         <description>
    ///             Present the outcome to the user and let the user proceed (e.g. by displaying a dialog with an OK button).
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Warning"/>    
    ///         </term>
    ///         <description>
    ///             Present the outcome to the user and let the user decide whether to proceed or to cancel. (e.g. by displaying a dialog with an OK and a Cancel button).
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///             <see cref="ValidationSeverity.Error"/>
    ///         </term>
    ///         <description>
    ///             Present the outcome to the user and only offer the option to cancel the process (e.g. by displaying a dialog with a disabled OK and an enabled Cancel button).
    ///         </description>
    ///     </item>
    /// </list>
    /// </note>
    public interface IValidationSummaryPresentationService
    {
        /// <summary>
        /// Presents the outcome of a validation to the user if the <see cref="ValidationSummary.Severity"/> is <see cref="ValidationSeverity.Warning"/> or 
        /// <see cref="ValidationSeverity.Error"/> 
        /// </summary>
        /// <param name="validationSummary">The <see cref="ValidationSummary"/> to present. A <see cref="ValidationSummary"/> is usually created by a <see cref="Validator{T}"/>.</param>
        /// <returns>Returns <c>true</c> if the user want's to proceed.</returns>
        bool ShowValidationSummary(ValidationSummary validationSummary);
        bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures);
        bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures, string headerText);
        bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures, string headerText, string howToProceedMessage);
    }
}
