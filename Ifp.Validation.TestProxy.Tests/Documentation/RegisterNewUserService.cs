﻿using System;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public interface IRegisterNewUserService
    {
        bool ValidateAndStoreNewUser(RegisterNewUserModel model);
    }

    public class RegisterNewUserService : IRegisterNewUserService
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
            {
                // The user pressed 'Cancel'.
                return false;
            }
            // Logic to store the model to the database.
            return true;
        }
        public static IRegisterNewUserService TestFactoryMethod()
        {
            return new RegisterNewUserService(new RegisterNewUserValidator(new PasswordValidationRule(new PasswordPolicyVerifier()), new BirthdateValidationRule()), new ValidationSummaryPresentationService());
        }
    }

    internal class ValidationSummaryPresentationService : IValidationSummaryPresentationService
    {
        public bool ShowValidationSummary(ValidationSummary validationSummary)
        {
            throw new NotImplementedException();
        }

        public bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures)
        {
            throw new NotImplementedException();
        }

        public bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures, string headerText)
        {
            throw new NotImplementedException();
        }

        public bool ShowValidationSummary(ValidationSummary validationSummary, bool showOnlyOnFailures, string headerText, string howToProceedMessage)
        {
            throw new NotImplementedException();
        }
    }
}
