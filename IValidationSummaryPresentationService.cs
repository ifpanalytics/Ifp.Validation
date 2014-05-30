using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifp.Validation
{
    public interface IValidationSummaryPresentationService
    {
        bool ShowValidationSummary(ValidationSummary validationSummary);
        bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures);
        bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures, string headerText);
        bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures, string headerText, string howToProceedMessage);
    }
}
