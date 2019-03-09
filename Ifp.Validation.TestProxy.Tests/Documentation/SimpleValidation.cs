using System;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    class SimpleValidation
    {
        bool IsRegisterNewUserModelValid(RegisterNewUserModel model)
        {
            // Use the ValidationSummaryBuilder to build a ValidationSummary step by step.
            ValidationSummaryBuilder vsBuilder = new ValidationSummaryBuilder();
            // Validate the object and append as much ValidationOutcomes as you like.
            if (String.IsNullOrWhiteSpace(model.EMail))
            {
                vsBuilder.Append("You must enter an email address".ToFailure(FailureSeverity.Error));
            }

            if (model.BithDate == null)
            {
                vsBuilder.Append("You did not enter a birth date.You will not be able to use some of our services.You can add this information later.".ToFailure(FailureSeverity.Information));
            }
            // Build the summary and use an IValidationSummaryPresentationService to present the summary to the user.
            var summary = vsBuilder.ToSummary();
            var presenter = new ValidationSummaryPresentationService();
            // If the user clicks on 'OK' the ShowValidationSummary method returns true.
            var userResponse = presenter.ShowValidationSummary(summary);
            return userResponse;
        }
    }
}
