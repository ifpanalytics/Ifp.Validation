namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public class RegisterNewUserValidator : RuleBasedValidator<RegisterNewUserModel>
    {
        public RegisterNewUserValidator(PasswordValidationRule passwordValidationRule, BirthdateValidationRule birthdateValidationRule)
            : base(passwordValidationRule, birthdateValidationRule)
        {

        }
    }
}
