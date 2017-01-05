using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public interface IRegisterNewUserService
    {
        bool ValidateAndStoreNewUser(RegisterNewUserModel model);
    }

    public class RegisterNewUserService: IRegisterNewUserService
    {
        public RegisterNewUserService(RegisterNewUserValidator validator, IValidationSummaryPresentationService validationSummaryPresentationService)
        {
            Validator = validator;
            ValidationSummaryPresentationService = validationSummaryPresentationService;
        }

        protected IValidationSummaryPresentationService ValidationSummaryPresentationService { get; }
        protected RegisterNewUserValidator Validator { get; }

        public bool ValidateAndStoreNewUser(RegisterNewUserModel model)
        {
            var summary = Validator.Validate(model);
            if (ValidationSummaryPresentationService.ShowValidationSummary(summary))
                // There was no error or the user pressed 'OK'.
                return false;
            // Logic to store the model to the database.
            return true;
        }
    }
}
