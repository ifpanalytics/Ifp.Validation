namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public class PasswordValidationRule : ValidationRule<RegisterNewUserModel>
    {
        // Import other services per constructor injection if needed
        public PasswordValidationRule(IPasswordPolicyVerifier passwordPolicyVerifier)
        {
            PasswordPolicyVerifier = passwordPolicyVerifier;
        }

        protected IPasswordPolicyVerifier PasswordPolicyVerifier { get; }

        public override ValidationOutcome ValidateObject(RegisterNewUserModel objectToValidate)
        {
            if (objectToValidate.Password != objectToValidate.PasswordRepeated)
            {
                return "The two passwords you entered are not the same.".ToFailure(FailureSeverity.Error);
            }

            if (!PasswordPolicyVerifier.ConformsToPolicy(objectToValidate.Password))
            {
                return "The password you entered does not conform to the password policy.".ToFailure(FailureSeverity.Error);
            }

            if (PasswordPolicyVerifier.IsWeakPassword(objectToValidate.Password))
            {
                return "The password you entered is valid but weak. Do you want to use it anyway?".ToFailure(FailureSeverity.Warning);
            }

            return ValidationOutcome.Success;
        }
    }
}
